using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ResponsiveViewRefreshLoadDemoScript : MonoBehaviour
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

        Button mScrollToButton;
        InputField mScrollToInput;
        Button mBackButton;
        int mItemCountPerRow = 3;
        public DragChangSizeScript mDragChangSizeScript;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            // totalItemCount +2 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(GetMaxRowCount()+2, OnGetItemByIndex);

            mDragChangSizeScript.mOnDragEndAction = OnViewPortSizeChanged;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            OnViewPortSizeChanged();
            InitButtonPanel();            
        }

        void InitButtonPanel()
        {
            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }

        void UpdateItemPrefab()
        {
            ItemPrefabConfData tData = mLoopListView.GetItemPrefabConfData("ItemPrefab2");
            GameObject prefabObj = tData.mItemPrefab;
            RectTransform rf = prefabObj.GetComponent<RectTransform>();
            BaseHorizontalItemList itemScript = prefabObj.GetComponent<BaseHorizontalItemList>();
            float w = mLoopListView.ViewPortWidth;
            int count = itemScript.mItemList.Count;
            GameObject p0 = itemScript.mItemList[0].gameObject;
            RectTransform rf0 = p0.GetComponent<RectTransform>();
            float w0 = rf0.rect.width;
            int c = Mathf.FloorToInt(w / w0);
            if (c == 0)
            {
                c = 1;
            }
            mItemCountPerRow = c;
            float padding = (w - w0 * c) / (c + 1);
            if (padding < 0)
            {
                padding = 0;
            }
            rf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            if (c > count)
            {
                int dif = c - count;
                for (int i = 0; i < dif; ++i)
                {
                    GameObject go = Object.Instantiate(p0, Vector3.zero, Quaternion.identity, rf);
                    RectTransform trf = go.GetComponent<RectTransform>();
                    trf.localScale = Vector3.one;
                    trf.anchoredPosition3D = Vector3.zero;
                    trf.rotation = Quaternion.identity;
                    BaseHorizontalItem t = go.GetComponent<BaseHorizontalItem>();
                    itemScript.mItemList.Add(t);
                }
            }
            else if (c < count)
            {
                int dif = count - c;
                for (int i = 0; i < dif; ++i)
                {

                    BaseHorizontalItem go = itemScript.mItemList[itemScript.mItemList.Count - 1];
                    itemScript.mItemList.RemoveAt(itemScript.mItemList.Count - 1);
                    Object.DestroyImmediate(go.gameObject);
                }
            }
            float curX = padding;
            for (int k = 0; k < itemScript.mItemList.Count; ++k)
            {
                GameObject obj = itemScript.mItemList[k].gameObject;
                obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(curX, 0, 0);
                curX = curX + w0 + padding;
            }
            mLoopListView.OnItemPrefabChanged("ItemPrefab2");
        }

        void OnViewPortSizeChanged()
        {
            UpdateItemPrefab();
            mLoopListView.SetListItemCount(GetMaxRowCount()+2, false);
            mLoopListView.RefreshAllShownItem();
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

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int row)
        {
            if (row < 0)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            if (row == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTipForRefresh(item);
                return item;
            }
            if (row == GetMaxRowCount() + 1)
            {
                item = listView.NewListViewItem("ItemPrefab1");
                UpdateLoadingTipForLoad(item);
                return item;
            }
            int itemRow = row - 1;

            item = listView.NewListViewItem("ItemPrefab2");
            BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            for (int i = 0; i < mItemCountPerRow; ++i)
            {
                int itemIndex = itemRow * mItemCountPerRow + i;
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
            OnDragingForLoad();
        }

        void OnEndDrag()
        {
            OnEndDragForRefresh();
            OnEndDragForLoad();
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

        void UpdateLoadingTipForLoad(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mLoopListView.ViewPortWidth);
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
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount() + 1);
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount());
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
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount() + 1);
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
                mLoopListView.SetListItemCount(GetMaxRowCount() + 2, false);
                mLoopListView.RefreshAllShownItem();
            }
        }

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0) || (itemIndex >= mDataSourceMgr.TotalItemCount))
            {
                return;
            }
            int tmpRowIndex = GetRowIndex(itemIndex+1);         
            mLoopListView.MovePanelToItemIndex(tmpRowIndex+1, 0);
        }

        int GetRow(int itemCount)
        {
            int tmpRow = itemCount / mItemCountPerRow;
            if (itemCount % mItemCountPerRow > 0)
            {
                tmpRow++;
            }
            return tmpRow;
        }
        
        int GetRowIndex(int itemCount)
        {
            int tmpRow = GetRow(itemCount);
            if (tmpRow > 0)
            {
                tmpRow--;
            }
            return tmpRow;
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }  
}
