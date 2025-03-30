using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class GridViewClickLoadMoreDemoScript : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        LoadingTipStatus mLoadingTipStatus = LoadingTipStatus.None;
        public int mLoadCount = 20;  
        ButtonPanelGridViewLoad mButtonPanel;     

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
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            mLoopGridView.InitGridView(mDataSourceMgr.TotalItemCount+1, OnGetItemByRowColumn);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelGridViewLoad();
            mButtonPanel.mLoopGridView = mLoopGridView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mExtraHeaderItemCount = 0;
            mButtonPanel.mExtraFooterItemCount = 1;
            mButtonPanel.Start();
        }         
       
        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index,int row,int column)
        {
            if (index < 0)
            {
                return null;
            }

            /*
            get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopGridView Inspector Setting
            */
            LoopGridViewItem item = null;
            if (index == mDataSourceMgr.TotalItemCount)
            {
                item = gridView.NewListViewItem("ItemPrefab0");
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    LoadClickItem itemScript0 = item.GetComponent<LoadClickItem>();
                    itemScript0.mRootButton.onClick.AddListener(OnLoadMoreButtonClicked);
                }
                UpdateLoadingTip(item);
                return item;
            }

            //get the data to showing
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            item = gridView.NewListViewItem("ItemPrefab1");
            BaseHorizontalItem itemScript = item.GetComponent<BaseHorizontalItem>();//get your own component
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData, index);
            return item;
        }

        void UpdateLoadingTip(LoopGridViewItem item)
        {
            if (item == null)
            {
                return;
            }
            LoadClickItem itemScript0 = item.GetComponent<LoadClickItem>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.None)
            {
                itemScript0.mText.text = "More";
                itemScript0.mWaitingIcon.SetActive(false);
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading...";
            }
        }

        void Update()
        {
            mDataSourceMgr.Update();
        }

        void OnLoadMoreButtonClicked()
        {
            if (mLoadingTipStatus != LoadingTipStatus.None)
            {
                return;
            }
            LoopGridViewItem item = mLoopGridView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount);
            if (item == null)
            {
                return;
            }
            if(mLoadCount < 0)
            {
                mLoadCount = 0;
            }
            mLoadingTipStatus = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip(item);
            mDataSourceMgr.RequestLoadMoreDataList(mLoadCount, OnDataSourceLoadMoreFinished);
        }

        void OnDataSourceLoadMoreFinished()
        {
            if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatus = LoadingTipStatus.None;
                mLoopGridView.SetListItemCount(mDataSourceMgr.TotalItemCount + 1, false);
                mLoopGridView.RefreshAllShownItem();
            }
        }       
    }
}
