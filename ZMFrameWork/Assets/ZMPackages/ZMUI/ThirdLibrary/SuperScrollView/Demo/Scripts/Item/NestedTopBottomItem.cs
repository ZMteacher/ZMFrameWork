using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class NestedTopBottomItem : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public Text mTitle;
        int mIndex;
        DataSourceMgr<ItemData> mDataSourceMgr;

        public void Init()
        {
            mLoopListView.InitListView(0, OnGetItemByIndex);
        }

        public void SetItemData(NestedItemData itemData)
        {
            mIndex = itemData.mIndex;
            mTitle.text = itemData.mName;
            mDataSourceMgr = itemData.mDataSourceMgr;
            mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount);
            mLoopListView.MovePanelToItemIndex(0, 0);
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
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
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            BaseVerticalLineItem itemScript = item.GetComponent<BaseVerticalLineItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemData.mName = "Item"+ mIndex + "_" + index; 
            itemScript.SetItemData(itemData,index);
            bool visible = (index != (mDataSourceMgr.TotalItemCount-1));
            itemScript.SetLineVisible(visible);
            return item;
        }
    }
}
