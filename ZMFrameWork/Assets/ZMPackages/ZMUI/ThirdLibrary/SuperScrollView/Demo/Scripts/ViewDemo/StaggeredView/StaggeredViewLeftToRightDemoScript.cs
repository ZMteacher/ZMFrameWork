using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class StaggeredViewLeftToRightDemoScript : MonoBehaviour
    {
        public LoopStaggeredGridView mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;       
        int[] mItemWidthArrayForDemo = null;
        float mMinWidth = 340.0f;
        int mCount = 100;
        ButtonPanelStaggeredView mButtonPanel;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);           
            InitItemWidthArray();
            GridViewLayoutParam param = new GridViewLayoutParam();
            param.mPadding1 = 10;
            param.mPadding2 = 10;
            param.mColumnOrRowCount = 2;
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, param, OnGetItemByIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelStaggeredView();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.Start();
        }  

        LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopStaggeredGridViewItem item = null;
            item = listView.NewListViewItem("ItemPrefab");      
            
            BaseHorizontalItem itemScript = item.GetComponent<BaseHorizontalItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetItemData(itemData, index);

            float itemWidth = mMinWidth + mItemWidthArrayForDemo[index % mItemWidthArrayForDemo.Length] * 10f;
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemWidth);

            return item;
        }

        void InitItemWidthArray()
        {
            mItemWidthArrayForDemo = new int[mCount];
            for (int i = 0; i < mItemWidthArrayForDemo.Length; ++i)
            {
                mItemWidthArrayForDemo[i] = Random.Range(0, 20);
            }
        }        
    }
}
