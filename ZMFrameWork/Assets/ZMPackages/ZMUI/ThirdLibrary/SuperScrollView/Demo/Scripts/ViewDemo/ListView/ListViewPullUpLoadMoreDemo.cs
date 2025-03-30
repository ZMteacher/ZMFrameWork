using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewPullUpLoadMoreDemo : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        LoadingTipStatus mLoadingTipStatus = LoadingTipStatus.None;
        float mLoadingTipItemHeight = 100;
        public int mLoadCount = 20;
        ButtonPanelLoad mButtonPanel;
       
        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            // totalItemCount +1 because the last "load more" banner is also a item.
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount + 1, OnGetItemByIndex);
            mLoopListView.mOnBeginDragAction = OnBeginDrag;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelLoad();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mExtraHeaderItemCount = 0;
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
            if(index == mDataSourceMgr.TotalItemCount -1)
            {
                item.Padding = 0;
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
            LoadItem itemScript0 = item.GetComponent<LoadItem>();
            if(itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Load More";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
            }
        }

        void OnBeginDrag()
        {

        }
        void OnDraging()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus != LoadingTipStatus.None && mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount);
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount-1);
            if (item1 == null)
            {
                return;
            }
            float y  = mLoopListView.GetItemCornerPosInViewPort(item1,ItemCornerEnum.LeftBottom).y;
            if(y + mLoopListView.ViewPortSize >= mLoadingTipItemHeight)
            {
                if (mLoadingTipStatus != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatus = LoadingTipStatus.WaitRelease;
                UpdateLoadingTip(item);
            }
            else
            {
                if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatus = LoadingTipStatus.None;
                UpdateLoadingTip(item);
            }
        }

        void OnEndDrag()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus != LoadingTipStatus.None && mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
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

        void Update()
        {
            mDataSourceMgr.Update();
        }  
    }
}
