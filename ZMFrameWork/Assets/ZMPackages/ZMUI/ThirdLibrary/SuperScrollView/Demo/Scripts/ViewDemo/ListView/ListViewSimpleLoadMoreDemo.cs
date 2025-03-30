using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewSimpleLoadMoreDemo : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;
        Button mBackButton;
        int mCurrentSelectItemId = -1;
        LoadingTipStatus mLoadingTipStatus = LoadingTipStatus.None;
        public int mLoadCount = 20; 
        IEnumerator mDelayCoroutine;
        float mDelayTime = 0.2f;
       
        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            // totalItemCount +1 because the last "load more" banner is also a item.
            int totalCount = mDataSourceMgr.TotalItemCount;
            if(mLoadCount > 0)
            {
                totalCount = mDataSourceMgr.TotalItemCount+1;
            }
            mLoopListView.InitListView(totalCount, OnGetItemByIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
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
                }
                StartCoroutineLoadMore();
                return item;
            }
            SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab1");
            SimpleItem itemScript = item.GetComponent<SimpleItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);
            }
            itemScript.SetItemData(itemData);
            itemScript.SetItemSelected(mCurrentSelectItemId == itemData.mId);
            return item;
        }

        void StartCoroutineLoadMore()
        {
            if(mDelayCoroutine != null)
            {
                StopCoroutine(mDelayCoroutine);
                mDelayCoroutine = null;
            }    
            mDelayCoroutine = StartLoadMore();              
            StartCoroutine(mDelayCoroutine); 
        }

        IEnumerator StartLoadMore()
        {
            yield return new WaitForSeconds(mDelayTime);
            LoadMore();
        } 

        void LoadMore()
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
            if( mLoadCount <= 0)
            {
                mLoadCount = 0;
                UpdateLoadingTip(item);  
                return;
            }
            mLoadingTipStatus = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip(item);            
            mDataSourceMgr.RequestLoadMoreDataList(mLoadCount, OnDataSourceLoadMoreFinished);
        }  

        void UpdateLoadingTip(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            SimpleLoadItem itemScript0 = item.GetComponent<SimpleLoadItem>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);               
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);            
            }
        }

        void Update()
        {
            mDataSourceMgr.Update();
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
                mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount+1, false);
                mLoopListView.RefreshAllShownItem();
            }
        }    

        void OnItemClicked(int itemId)
        {
            mCurrentSelectItemId = itemId;
            mLoopListView.RefreshAllShownItem();
        }    

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
