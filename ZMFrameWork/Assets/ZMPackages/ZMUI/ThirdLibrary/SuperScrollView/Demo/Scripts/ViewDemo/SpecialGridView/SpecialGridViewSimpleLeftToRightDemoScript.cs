using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpecialGridViewSimpleLeftToRightDemoScript : MonoBehaviour
    {        
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;
        const int mItemCountPerRow = 3;// how many items in one row
        int mItemPadding = 10;
        int mItemMinHight = 45;
        int mItemMaxHeight = 150;
        int[][] mItemHeightArray = null;
        int mItemHeightCount = 100;
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
            InitItemHeightArrayForDemo();
            //count is the total row count
            mLoopListView.InitListView(row, OnGetItemByIndex);
            InitButtonPanel(); 
        }

        void InitItemHeightArrayForDemo()
        {
            mItemHeightArray = new int[2][];
            for (int i = 0; i < mItemHeightArray.Length; ++i)
            {
                mItemHeightArray[i] = new int[mItemHeightCount];
                for (int j = 0; j < mItemHeightCount; ++j)
                {
                    mItemHeightArray[i][j] = Random.Range(mItemMinHight, mItemMaxHeight);
                }
            }
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
            UpdateItemHeightAndPos(itemScript,rowIndex);
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                for (int i = 0; i < mItemCountPerRow; ++i)
                {
                    itemScript.mItemList[i].Init(OnItemClicked);
                }
            }
            //update all items in the row
            for (int i = 0;i< mItemCountPerRow; ++i)
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

        void UpdateItemHeightAndPos(SimpleItemList itemScript,int rowIndex)
        {
            float heightViewport = mLoopListView.ViewPortHeight;
            int count = itemScript.mItemList.Count;
            float preHeight = 0;
            for(int i = 0; i < count-1; i++)
            {
                GameObject item = itemScript.mItemList[i].gameObject;            
                RectTransform rectItem = item.GetComponent<RectTransform>();
                float heightItem = mItemHeightArray[i][rowIndex % mItemHeightCount];
                rectItem.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightItem);
                preHeight = preHeight + heightItem + mItemPadding;
            }

            GameObject itemLast = itemScript.mItemList[count-1].gameObject;            
            RectTransform rectItemLast = itemLast.GetComponent<RectTransform>();
            float heightItemLast = heightViewport - preHeight;//random the item's height
            rectItemLast.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightItemLast);
        
            float curY = 0;
            for (int j = 0; j < itemScript.mItemList.Count; ++j)
            {
                GameObject item = itemScript.mItemList[j].gameObject;
                item.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, curY, 0);
                RectTransform rectItem = item.GetComponent<RectTransform>();
                float height = rectItem.rect.height;
                curY = curY - height - mItemPadding;
            }  
        }

        void OnItemClicked(int itemId)
        {
            mCurrentSelectItemId = itemId;
            mLoopListView.RefreshAllShownItem();
        }

        void OnSetCountButtonClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count < 0)
            {
                return;
            }

            int tmpRow = count / mItemCountPerRow;
            if (count % mItemCountPerRow > 0)
            {
                tmpRow++;
            }
            mDataSourceMgr.SetDataTotalCount(count);
            mLoopListView.SetListItemCount(tmpRow, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if ((itemIndex < 0) || (itemIndex >= mDataSourceMgr.TotalItemCount))
            {
                return;
            }
            int tmpCount = itemIndex + 1;
            int tmpRow = tmpCount / mItemCountPerRow;
            if (tmpCount % mItemCountPerRow > 0)
            {
                tmpRow++;
            }
            if (tmpRow > 0)
            {
                tmpRow--;
            }
            mLoopListView.MovePanelToItemIndex(tmpRow, 0);
        }

        void OnAddButtonClicked()
        {
            SimpleItemData newData = mDataSourceMgr.InsertData(mDataSourceMgr.TotalItemCount);
            int tmpCount = mDataSourceMgr.TotalItemCount;
            int tmpRow = tmpCount / mItemCountPerRow;
            if (tmpCount % mItemCountPerRow > 0)
            {
                tmpRow++;
            }
            mLoopListView.SetListItemCount(tmpRow, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
