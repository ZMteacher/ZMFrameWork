using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewTopToBottomDemoScript : MonoBehaviour
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
        private int m_ViewDataCount = 99; //你的数据列表长度 建议在当前接口触发时向数据层索取
        public void RefreshListView(bool reSetPos)
        {
         
            // if (!loopListView.ListViewInited)
            // {
            //     //初始化滚动列表 切记不可在Awake或Start中初始化Count=0的列表。会有BUG
            //     loopListView.InitListView(m_ViewDataCount, OnGetItemByIndex);
            // }
            // else
            // {
            //     //数据发生变化，重新设置最新的数据，数据增删必须要调用此接口，否则会出现item索引与数据不一致和一切其他的显示BUG
            //     loopListView.SetListItemCount(m_ViewDataCount, reSetPos);
            //     loopListView.RefreshAllShownItem();
            // }
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= m_ViewDataCount) return null;
             
            //获取item显示数据
            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if(itemData == null) return null; 
         
            //创建对应item预制体
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            //获取Item上的脚本组件
            // BaseVerticalItem itemScript = item.GetComponent<BaseVerticalItem>();
            //
            // if (item.IsInitHandlerCalled == false)
            // {
            //     item.IsInitHandlerCalled = true;
            //     //item脚本初始化接口
            //     itemScript.Init();
            // }
            // //设置Item脚本数据
            // itemScript.SetItemData(itemData,index);
            return item;
        }
    }
}
