using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpecialGridViewPullUpLoadMoreDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;        
        LoadingTipStatus mLoadingTipStatusForLoad = LoadingTipStatus.None;        
        float mLoadingTipItemHeightForLoad = 100;
        public int mLoadCount = 20;
        int mItemCountPerRow = 3;
        ButtonPanelSpecialLoad mButtonPanel;    

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            // totalItemCount +1 because the "pull to load" banner is also a item.
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
            mButtonPanel.mExtraHeaderItemCount = 0;
            mButtonPanel.mExtraFooterItemCount = 1;
            mButtonPanel.Start();
        }    

        int GetMaxRowCount()
        {
            int count1 = mDataSourceMgr.TotalItemCount / mItemCountPerRow;
            if (mDataSourceMgr.TotalItemCount % mItemCountPerRow > 0)
            {
                count1++;
            }
            return count1;
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            if (rowIndex == GetMaxRowCount())
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTipForLoad(item);
                return item;
            }
            int initRowIndex = rowIndex;
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

        void Update()
        {
            mDataSourceMgr.Update();
        }

        void OnDraging()
        {
            OnDragingForLoad();
        }

        void OnEndDrag()
        {
            OnEndDragForLoad();
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
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatusForLoad != LoadingTipStatus.None && mLoadingTipStatusForLoad != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount());
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount()-1);
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
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatusForLoad != LoadingTipStatus.None && mLoadingTipStatusForLoad != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount());
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
            if (mLoadingTipStatusForLoad == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatusForLoad = LoadingTipStatus.None;
                mLoopListView.SetListItemCount(GetMaxRowCount() + 1, false);
                mLoopListView.RefreshAllShownItem();
            }
        }          
    }
}
