using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class GridViewSelectDeleteDemoScript : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
        public int mTotalDataCount = 10000;//total item count in the GridView
        DataSourceMgr<ItemData> mDataSourceMgr;
        ButtonPanelGridViewDelete mButtonPanel;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            mLoopGridView.InitGridView(mDataSourceMgr.TotalItemCount, OnGetItemByRowColumn);
            InitButtonPanel();
        }
        
        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelGridViewDelete();
            mButtonPanel.mLoopGridView = mLoopGridView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.Start();
        }          

        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
        {
            if (index < 0)
            {
                return null;
            }

            ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            /*
            get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopGridView Inspector Setting
            */
            LoopGridViewItem item = gridView.NewListViewItem("ItemPrefab");

            BaseHorizontalToggleItem itemScript = item.GetComponent<BaseHorizontalToggleItem>();//get your own component
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData,index);
            return item;
        }
    }
}
