using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class NestedSimpleSpecialGridViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;
        const int mItemCountPerRow = 30;// how many items in one row
        float mItemPadding = 10;
        float mItemOffsetX = 0;
        float mItemOffsetY = 0;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        Button mBackButton;
        int mCurrentSelectItemId = -1;        

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            int row = mDataSourceMgr.TotalItemCount / mItemCountPerRow;
            if(mDataSourceMgr.TotalItemCount % mItemCountPerRow > 0)
            {
                row++;
            }
            UpdateItemPrefab();//create all the items in a row and update the content width.
            //count is the total row count
            mLoopListView.InitListView(row, OnGetItemByIndex);
            InitButtonPanel(); 
        }

        void UpdateItemPrefab()
        {
            ItemPrefabConfData tData = mLoopListView.GetItemPrefabConfData("ItemPrefab");
            GameObject prefabObj = tData.mItemPrefab;
            SimpleItemList itemScript = prefabObj.GetComponent<SimpleItemList>();
            GameObject simpleItemGameObj = itemScript.mItemList[0].gameObject;
            RectTransform rf0 = simpleItemGameObj.GetComponent<RectTransform>();
            mItemOffsetX = rf0.anchoredPosition.x;
            mItemOffsetY = rf0.anchoredPosition.y;
            float itemWidth = rf0.rect.width;
            float curX = mItemOffsetX + itemWidth + mItemPadding;
            for (int i = 1;i < mItemCountPerRow; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(simpleItemGameObj, Vector3.zero, Quaternion.identity, prefabObj.transform);
                go.SetActive(true);
                RectTransform rf = go.GetComponent<RectTransform>();
                rf.localScale = Vector3.one;
                rf.localEulerAngles = Vector3.zero;
                rf.anchoredPosition3D = new Vector3(curX, mItemOffsetY, 0);
                curX = curX + itemWidth + mItemPadding;
                itemScript.mItemList.Add(go.GetComponent<SimpleItem>());
            }
            float contentWidth = curX - mItemPadding + mItemOffsetX;
            prefabObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
            ScrollRect scrollRect = mLoopListView.gameObject.GetComponent<ScrollRect>();
            scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
        }

        void InitButtonPanel()
        {
            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);
            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);
            
            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }     

        /*when a row is getting show in the scrollrect viewport, 
        this method will be called with the row’ rowIndex as a parameter, 
        to let you create the row  and update its content.

        SuperScrollView uses single items with subitems that make up the columns in the row.
        so in fact, the GridView is ListView.
        if one row is make up with 3 subitems, then the GridView looks like:

            row0:  subitem0 subitem1 subitem2
            row1:  subitem3 subitem4 subitem5
            row2:  subitem6 subitem7 subitem8
            row3:  subitem9 subitem10 subitem11
            ...
        */
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0 )
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab"); 
            SimpleItemList itemScript = item.GetComponent<SimpleItemList>();
            itemScript.mNameList.text = "Item"+rowIndex;
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                for (int i = 0; i < mItemCountPerRow; ++i)
                {
                    itemScript.mItemList[i].Init(OnItemClicked);
                }
            }
            //update all items in the row
            for (int i = 0;i < mItemCountPerRow; ++i)
            {
                int itemIndex = rowIndex * mItemCountPerRow + i;
                if(itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                //update the subitem content.
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemData.mName = "Item"+ rowIndex + "_" + i;
                    itemScript.mItemList[i].SetItemData(itemData);
                    itemScript.mItemList[i].SetItemSelected(mCurrentSelectItemId == itemData.mId);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }
            return item;
        }    

        void OnItemClicked(int itemId)
        {
            mCurrentSelectItemId = itemId;
            mLoopListView.RefreshAllShownItem();
        }

        void OnSetCountButtonClicked()
        {
            int rowCount = 0;
            if (int.TryParse(mSetCountInput.text, out rowCount) == false)
            {
                return;
            }
            if (rowCount < 0)
            {
                return;
            }
            int dataCount = rowCount * mItemCountPerRow;           
            mDataSourceMgr.SetDataTotalCount(dataCount);
            mLoopListView.SetListItemCount(rowCount, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnScrollToButtonClicked()
        {
            int itemRowIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemRowIndex) == false)
            {
                return;
            }
            int row = mDataSourceMgr.TotalItemCount / mItemCountPerRow;
            if ((itemRowIndex < 0) || (itemRowIndex >= row))
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemRowIndex, 0);
        }

        void OnAddButtonClicked()
        {
            int count = mDataSourceMgr.TotalItemCount;
            for(int i = 0; i < mItemCountPerRow; i++)
            {
                mDataSourceMgr.InsertData(count+i);
            }
            int tmpCount = mDataSourceMgr.TotalItemCount;
            int tmpRow = tmpCount / mItemCountPerRow;
            if (tmpCount % mItemCountPerRow > 0)
            {
                tmpRow++;
            }
            mLoopListView.SetListItemCount( tmpRow, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
