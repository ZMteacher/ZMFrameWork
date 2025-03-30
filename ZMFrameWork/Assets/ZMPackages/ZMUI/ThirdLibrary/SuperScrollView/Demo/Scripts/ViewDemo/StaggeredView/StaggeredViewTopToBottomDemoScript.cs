using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class StaggeredViewTopToBottomDemoScript : MonoBehaviour
    {
        public LoopStaggeredGridView mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        int[] mItemHeightArrayForDemo = null;
        float mMinHeight = 260.0f;
        int mCount = 100;
        ButtonPanelStaggeredView mButtonPanel;  

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            InitItemHeightArrayForDemo();
            GridViewLayoutParam param = new GridViewLayoutParam();
            param.mPadding1 = 16;
            param.mPadding2 = 16;
            param.mColumnOrRowCount = 3;
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, param, OnGetItemByItemIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelStaggeredView();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.Start();
        }     

        LoopStaggeredGridViewItem OnGetItemByItemIndex(LoopStaggeredGridView listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }
            //get the data to showing
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if(itemData == null)
            {
                return null;
            }
            /*get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopStaggeredGridView Inspector Setting */
            LoopStaggeredGridViewItem item = listView.NewListViewItem("ItemPrefab");
            //get your own component
            BaseHorizontalItem itemScript = item.GetComponent<BaseHorizontalItem>();
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData, index);
            //set the item's height, just for demo showing.
            float itemHeight = mMinHeight + mItemHeightArrayForDemo[index % mItemHeightArrayForDemo.Length] * 10f;
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
            return item;
        }

        void InitItemHeightArrayForDemo()
        {
            mItemHeightArrayForDemo = new int[mCount];
            for (int i = 0; i < mItemHeightArrayForDemo.Length; ++i)
            {
                mItemHeightArrayForDemo[i] = Random.Range(0, 20);
            }
        } 
    }
}
