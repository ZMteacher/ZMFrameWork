using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class GridViewComplexDemoScript : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        ButtonPanelGridView mButtonPanel;     

        // Use this for initialization
        void Start()
        {
            /*LoopGridViewSettingParam settingParam = new LoopGridViewSettingParam();
            settingParam.mItemSize = new Vector2(500, 500);
            settingParam.mItemPadding = new Vector2(40, 40);
            settingParam.mPadding = new RectOffset(10, 20, 30, 40);
            settingParam.mGridFixedType = GridFixedType.RowCountFixed;
            settingParam.mFixedRowOrColumnCount = 6;
            mLoopGridView.InitGridView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex, settingParam);
            */

            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            mLoopGridView.InitGridView(mDataSourceMgr.TotalItemCount, OnGetItemByRowColumn);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelGridView();
            mButtonPanel.mLoopGridView = mLoopGridView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.Start();
        }         
       
         LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index,int row,int column)
        {
            if (index < 0)
            {
                return null;
            }
            
            //get the data to showing
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
            LoopGridViewItem item = item = gridView.NewListViewItem("ItemPrefab"); 
            BaseRowColItem itemScript = item.GetComponent<BaseRowColItem>();//get your own component
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData, index, row, column);            
            return item;
        }
    }
}
