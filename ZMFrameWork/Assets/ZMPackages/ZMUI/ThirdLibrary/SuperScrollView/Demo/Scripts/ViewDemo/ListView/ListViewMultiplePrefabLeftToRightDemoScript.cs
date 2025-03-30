using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewMultiplePrefabLeftToRightDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        ButtonPanel mButtonPanel;
        
        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanel();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.Start();
        }   
      
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index > mDataSourceMgr.TotalItemCount)
            {
                return null;
            }

            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            if( index % 3 == 0 )
            {
                item = listView.NewListViewItem("ItemPrefab1");
                BaseHorizontalItem itemScript = item.GetComponent<BaseHorizontalItem>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                }
                itemScript.SetItemData(itemData, index);     
               
            }
            else if( index % 3 == 1 )
            {
                item = listView.NewListViewItem("ItemPrefab2");
                SliderComplexItem itemScript = item.GetComponent<SliderComplexItem>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                }
                itemScript.SetItemData(itemData, index);                
            }
            else
            {
                item = listView.NewListViewItem("ItemPrefab3");
                ImageItem itemScript = item.GetComponent<ImageItem>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                }
                itemScript.SetItemData(itemData, index);               
            }
            return item;
        }
       
    }
}
