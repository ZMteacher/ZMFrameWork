using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class GridViewSimpleDiagonalDemoScript : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;   
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
            /*LoopGridViewSettingParam settingParam = new LoopGridViewSettingParam();
            settingParam.mItemSize = new Vector2(500, 500);
            settingParam.mItemPadding = new Vector2(40, 40);
            settingParam.mPadding = new RectOffset(10, 20, 30, 40);
            settingParam.mGridFixedType = GridFixedType.RowCountFixed;
            settingParam.mFixedRowOrColumnCount = 6;
            mLoopGridView.InitGridView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex, settingParam);
            */

            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            mLoopGridView.InitGridView(mDataSourceMgr.TotalItemCount, OnGetItemByRowColumn);
            InitButtonPanel();
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
       
        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index,int row,int column)
        {
            if (index < 0)
            {
                return null;
            }
            
            //get the data to showing
            SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            /*
            get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopGridView Inspector Setting
            */
            LoopGridViewItem item = gridView.NewListViewItem("ItemPrefab");

            SimpleItem itemScript = item.GetComponent<SimpleItem>();//get your own component
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData);
            itemScript.SetItemSelected(mCurrentSelectItemId == itemData.mId);
            return item;
        }    

        void OnItemClicked(int itemId)
        {
            mCurrentSelectItemId = itemId;
            mLoopGridView.RefreshAllShownItem();
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
            if (count <= mCurrentSelectItemId)
            {
                mCurrentSelectItemId = -1;
            }
            mDataSourceMgr.SetDataTotalCount(count);
            mLoopGridView.SetListItemCount(count, false);
            mLoopGridView.RefreshAllShownItem();
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
            mLoopGridView.MovePanelToItemByIndex(itemIndex, 0);
        }

        void OnAddButtonClicked()
        {
            SimpleItemData newData = mDataSourceMgr.InsertData(mDataSourceMgr.TotalItemCount);
            mLoopGridView.SetListItemCount(mDataSourceMgr.TotalItemCount, false);
            mLoopGridView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }  
    }
}
