using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewPullDownRefreshOrPullUpLoadDemo : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        LoadingTipStatus mLoadingTipStatusForRefresh = LoadingTipStatus.None;
        LoadingTipStatus mLoadingTipStatusForLoad = LoadingTipStatus.None;
        float mDataLoadedTipShowLeftTime = 0;
        float mLoadingTipItemHeightForRefresh = 100;
        float mLoadingTipItemHeightForLoad = 100;
        int mLoadMoreCount = 20;
        ButtonPanelLoad mButtonPanel;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);

            // totalItemCount +2 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount + 2, OnGetItemByIndex);
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelLoad();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mExtraHeaderItemCount = 1;
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
            if (index == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTipForRefresh(item);
                return item;
            }
            if (index == mDataSourceMgr.TotalItemCount+1)
            {
                item = listView.NewListViewItem("ItemPrefab1");
                UpdateLoadingTipForLoad(item);
                return item;
            }
            int itemDataIndex = index - 1;
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemDataIndex);
            if (itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab2");
            InputFieldItem itemScript = item.GetComponent<InputFieldItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            if (index == mDataSourceMgr.TotalItemCount)
            {
                item.Padding = 0;
            }

            itemScript.SetItemData(itemData, itemDataIndex);
            return item;
        }

        void UpdateLoadingTipForRefresh(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            LoadItem itemScript0 = item.GetComponent<LoadItem>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Refresh";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForRefresh);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForRefresh);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.Loaded)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(false);
                itemScript0.mText.text = "Refreshed Success";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForRefresh);
            }
        }

        void OnDraging()
        {
            OnDragingForRefresh();
            OnDragingForLoad();
        }

        void OnEndDrag()
        {
            OnEndDragForRefresh();
            OnEndDragForLoad();
        }

        void OnDragingForRefresh()
        {
            if (mLoopListView.ShownItemCount <= 2)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh != LoadingTipStatus.None && mLoadingTipStatusForRefresh != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            ScrollRect sr = mLoopListView.ScrollRect;
            Vector3 pos = sr.content.anchoredPosition3D;
            if (pos.y < -mLoadingTipItemHeightForRefresh)
            {
                if (mLoadingTipStatusForRefresh != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatusForRefresh = LoadingTipStatus.WaitRelease;
                UpdateLoadingTipForRefresh(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, mLoadingTipItemHeightForRefresh, 0);
            }
            else
            {
                if (mLoadingTipStatusForRefresh != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatusForRefresh = LoadingTipStatus.None;
                UpdateLoadingTipForRefresh(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            }
        }

        void OnEndDragForRefresh()
        {
            if (mLoopListView.ShownItemCount <= 2)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh != LoadingTipStatus.None && mLoadingTipStatusForRefresh != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatusForRefresh != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            mLoadingTipStatusForRefresh = LoadingTipStatus.WaitLoad;
            UpdateLoadingTipForRefresh(item);
            mDataSourceMgr.RequestRefreshDataList(OnDataSourceRefreshFinished);
        }

        void OnDataSourceRefreshFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatusForRefresh = LoadingTipStatus.Loaded;
                mDataLoadedTipShowLeftTime = 0.7f;
                LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                if (item == null)
                {
                    return;
                }
                UpdateLoadingTipForRefresh(item);
                mLoopListView.RefreshAllShownItem();
            }
        }

        void Update()
        {
            mDataSourceMgr.Update();
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh == LoadingTipStatus.Loaded)
            {
                mDataLoadedTipShowLeftTime -= Time.deltaTime;
                if (mDataLoadedTipShowLeftTime <= 0)
                {
                    mLoadingTipStatusForRefresh = LoadingTipStatus.None;
                    LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                    if (item == null)
                    {
                        return;
                    }
                    UpdateLoadingTipForRefresh(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, -mLoadingTipItemHeightForRefresh, 0);
                    mLoopListView.OnItemSizeChanged(0);
                }
            }
        }

        void UpdateLoadingTipForLoad(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            LoadItem itemScript0 = item.GetComponent<LoadItem>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatusForLoad == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatusForLoad == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Load More";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForLoad);
            }
            else if (mLoadingTipStatusForLoad == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForLoad);
            }
        }
       
        void OnDragingForLoad()
        {
            if (mLoopListView.ShownItemCount <= 2)
            {
                return;
            }
            if (mLoadingTipStatusForLoad != LoadingTipStatus.None && mLoadingTipStatusForLoad != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount+1);
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount);
            if (item1 == null)
            {
                return;
            }
            float y = mLoopListView.GetItemCornerPosInViewPort(item1, ItemCornerEnum.LeftBottom).y;
            if (y + mLoopListView.ViewPortSize >= mLoadingTipItemHeightForLoad)
            {
                if (mLoadingTipStatusForLoad != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatusForLoad = LoadingTipStatus.WaitRelease;
                UpdateLoadingTipForLoad(item);
            }
            else
            {
                if (mLoadingTipStatusForLoad != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatusForLoad = LoadingTipStatus.None;
                UpdateLoadingTipForLoad(item);
            }
        }

        void OnEndDragForLoad()
        {
            if (mLoopListView.ShownItemCount <= 2)
            {
                return;
            }
            if (mLoadingTipStatusForLoad != LoadingTipStatus.None && mLoadingTipStatusForLoad != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(mDataSourceMgr.TotalItemCount+1);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatusForLoad != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            mLoadingTipStatusForLoad = LoadingTipStatus.WaitLoad;
            UpdateLoadingTipForLoad(item);
            mDataSourceMgr.RequestLoadMoreDataList(mLoadMoreCount, OnDataSourceLoadMoreFinished);
        }

        void OnDataSourceLoadMoreFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatusForLoad == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatusForLoad = LoadingTipStatus.None;
                mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount + 2, false);
                mLoopListView.RefreshAllShownItem();
            }
        }      
    }
}
