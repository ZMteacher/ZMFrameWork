using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpecialGridViewPullDownRefreshDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;     
        LoadingTipStatus mLoadingTipStatusForRefresh = LoadingTipStatus.None;
        float mDataLoadedTipShowLeftTime = 0;
        float mLoadingTipItemHeightForRefresh = 100;        
        int mItemCountPerRow = 3;
        ButtonPanelSpecialLoad mButtonPanel;  

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            // totalItemCount +1 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(GetMaxRowCount()+1, OnGetItemByIndex);
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelSpecialLoad();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mItemCountPerRow = mItemCountPerRow;
            mButtonPanel.mExtraHeaderItemCount = 1;// the 0'th item is the "pull to refresh" banner, not a real item.
            mButtonPanel.mExtraFooterItemCount = 0;
            mButtonPanel.Start();
        }     

        int GetMaxRowCount()
        {
            int tmpCount = mDataSourceMgr.TotalItemCount / mItemCountPerRow;
            if (mDataSourceMgr.TotalItemCount % mItemCountPerRow > 0)
            {
                tmpCount++;
            }
            return tmpCount;
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            if (rowIndex == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTipForRefresh(item);
                return item;
            }
            int initRowIndex = rowIndex - 1;
            item = listView.NewListViewItem("ItemPrefab1");
            BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            for (int i = 0; i < mItemCountPerRow; ++i)
            {
                int itemIndex = initRowIndex * mItemCountPerRow + i;
                if (itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }
            return item;
        }

        void UpdateLoadingTipForRefresh(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mLoopListView.ViewPortWidth);
            LoadComplexItem itemScript0 = item.GetComponent<LoadComplexItem>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh == LoadingTipStatus.None)
            {
                itemScript0.mRoot1.SetActive(false);
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitContinureDrag)
            {
                itemScript0.mRoot1.SetActive(true);
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot1.SetActive(false);
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Refresh";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForRefresh);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot1.SetActive(false);
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeightForRefresh);
            }
            else if (mLoadingTipStatusForRefresh == LoadingTipStatus.Loaded)
            {
                itemScript0.mRoot1.SetActive(false);
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
        }

        void OnEndDrag()
        {
            OnEndDragForRefresh();
        }

        void OnDragingForRefresh()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatusForRefresh != LoadingTipStatus.None && mLoadingTipStatusForRefresh != LoadingTipStatus.WaitRelease
                && mLoadingTipStatusForRefresh != LoadingTipStatus.WaitContinureDrag)
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
            if(pos.y >= 0)
            {
                if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitContinureDrag)
                {
                    mLoadingTipStatusForRefresh = LoadingTipStatus.None;
                    UpdateLoadingTipForRefresh(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
                }
            }
            else if (pos.y < 0 && pos.y > -mLoadingTipItemHeightForRefresh)
            {
                if (mLoadingTipStatusForRefresh == LoadingTipStatus.None || mLoadingTipStatusForRefresh == LoadingTipStatus.WaitRelease)
                {
                    mLoadingTipStatusForRefresh = LoadingTipStatus.WaitContinureDrag;
                    UpdateLoadingTipForRefresh(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
                }
            }
            else if(pos.y <= -mLoadingTipItemHeightForRefresh)
            {
                if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitContinureDrag)
                {
                    mLoadingTipStatusForRefresh = LoadingTipStatus.WaitRelease;
                    UpdateLoadingTipForRefresh(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, mLoadingTipItemHeightForRefresh, 0);
                }
            }
        }

        void OnEndDragForRefresh()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatusForRefresh == LoadingTipStatus.WaitRelease)
            {
                mLoadingTipStatusForRefresh = LoadingTipStatus.WaitLoad;
                UpdateLoadingTipForRefresh(item);
                mDataSourceMgr.RequestRefreshDataList(OnDataSourceRefreshFinished);
            }
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
    }
}
