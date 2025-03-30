using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public enum LoadingTipStatus
    {
        None,
        WaitContinureDrag,
        WaitRelease,
        WaitLoad,
        Loaded,
    }    

    public class ListViewPullDownRefreshDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        LoadingTipStatus mLoadingTipStatus = LoadingTipStatus.None;
        float mDataLoadedTipShowLeftTime = 0;
        float mLoadingTipItemHeight = 100;
        ButtonPanelLoad mButtonPanel;
        
        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            // totalItemCount +1 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount+1, OnGetItemByIndex);
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelLoad();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mExtraHeaderItemCount = 1;// the 0'th item is the "pull to refresh" banner, not a real item.
            mButtonPanel.mExtraFooterItemCount = 0;
            mButtonPanel.Start();
        }    

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView,int index)
        {
            if(index < 0 || index > mDataSourceMgr.TotalItemCount)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (index == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTip(item);
                return item;
            }
            int itemDataIndex = index - 1;
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemDataIndex);
            if(itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab1");
            BaseVerticalLineItem itemScript = item.GetComponent<BaseVerticalLineItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(itemData, itemDataIndex);
            bool visible = (itemDataIndex != (mDataSourceMgr.TotalItemCount-1));
            itemScript.SetLineVisible(visible);
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
                itemScript0.mText.text = "Release to Refresh";
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
            else if (mLoadingTipStatus == LoadingTipStatus.Loaded)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(false);
                itemScript0.mText.text = "Refreshed Success";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
            }
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
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if(item == null)
            {
                return;
            }
            ScrollRect sr = mLoopListView.ScrollRect;
            Vector3 pos = sr.content.anchoredPosition3D;
            if (pos.y < -mLoadingTipItemHeight)
            {
                if(mLoadingTipStatus != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatus = LoadingTipStatus.WaitRelease;
                UpdateLoadingTip(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, mLoadingTipItemHeight, 0);
            }
            else
            {
                if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatus = LoadingTipStatus.None;
                UpdateLoadingTip(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
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
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
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
            mDataSourceMgr.RequestRefreshDataList(OnDataSourceRefreshFinished);
        }

        void OnDataSourceRefreshFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatus = LoadingTipStatus.Loaded;
                mDataLoadedTipShowLeftTime = 0.7f;
                LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                if (item == null)
                {
                    return;
                }
                UpdateLoadingTip(item);  
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
            if (mLoadingTipStatus == LoadingTipStatus.Loaded)
            {
                mDataLoadedTipShowLeftTime -= Time.deltaTime;
                if (mDataLoadedTipShowLeftTime <= 0)
                {
                    mLoadingTipStatus = LoadingTipStatus.None;
                    LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                    if (item == null)
                    {
                        return;
                    }
                    UpdateLoadingTip(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, -mLoadingTipItemHeight, 0);
                    mLoopListView.OnItemSizeChanged(0);
                }
            }
        }
    }
}
