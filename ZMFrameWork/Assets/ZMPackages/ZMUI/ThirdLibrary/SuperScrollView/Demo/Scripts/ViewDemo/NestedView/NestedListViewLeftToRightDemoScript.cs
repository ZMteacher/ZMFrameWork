using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class NestedListViewLeftToRightDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 100;
        DataSourceMgr<NestedItemData> mDataSourceMgr;
        ButtonPanelNested mButtonPanel;               

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<NestedItemData>(mTotalDataCount);
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex);     
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelNested();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.Start();
        }     

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }

            NestedItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }

            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            NestedTopBottomItem itemScript = item.GetComponent<NestedTopBottomItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData);
            return item;
        }
    }
}
