using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewClickLoadMoreDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        LoadingTipStatus mLoadingTipStatus = LoadingTipStatus.None;
        public int mLoadCount = 20;   
        ButtonPanelLoad mButtonPanel;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            // totalItemCount +1 because the last "load more" button is also a item.
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount+1, OnGetItemByIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelLoad();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mExtraHeaderItemCount = 0;// the 0'th item is the "pull to refresh" banner, not a real item.
            mButtonPanel.mExtraFooterItemCount = 1;    
            mButtonPanel.Start();
        }   

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (index == mDataSourceMgr.TotalItemCount)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    LoadClickItem itemScript0 = item.GetComponent<LoadClickItem>();
                    itemScript0.mRootButton.onClick.AddListener(OnLoadMoreButtonClicked);
                }
                UpdateLoadingTip(item);
                return item;
            }
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab1");
            BaseVerticalItem itemScript = item.GetComponent<BaseVerticalItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData, index);
            return item;
        }

        void UpdateLoadingTip(LoopListViewItem2 item)
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
                itemScript0.mText.text = "Click to Load More";
                itemScript0.mWaitingIcon.SetActive(false);
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
            }
        }

        void Update()
        {
            mDataSourceMgr.Update();
        }

        void OnLoadMoreButtonClicked()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus != LoadingTipStatus.None)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount);
            if (item == null)
            {
                return;
            }
            mLoadingTipStatus = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip(item);
            if(mLoadCount < 0)
            {
                mLoadCount = 0;
            }
            mDataSourceMgr.RequestLoadMoreDataList(mLoadCount, OnDataSourceLoadMoreFinished);
        }

        void OnDataSourceLoadMoreFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatus = LoadingTipStatus.None;
                mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount + 1, false);
                mLoopListView.RefreshAllShownItem();
            }
        }        
    }
}
