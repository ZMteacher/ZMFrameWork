using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpecialGridViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        const int mItemCountPerRow = 3;// how many items in one row
        ButtonPanelSpecial mButtonPanel;        

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            int row = mDataSourceMgr.TotalItemCount / mItemCountPerRow;
            if(mDataSourceMgr.TotalItemCount % mItemCountPerRow > 0)
            {
                row++;
            }
            //count is the total row count
            mLoopListView.InitListView(row, OnGetItemByIndex);    
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelSpecial();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mDataSourceMgr = mDataSourceMgr;
            mButtonPanel.mItemCountPerRow = mItemCountPerRow;
            mButtonPanel.Start();
        }               

        /*when a row is getting show in the scrollrect viewport, 
        this method will be called with the row’ rowIndex as a parameter, 
        to let you create the row  and update its content.

        SuperScrollView uses single items with subitems that make up the columns in the row.
        so in fact, the GridView is ListView.
        if one row is make up with 3 subitems, then the GridView looks like:

            row0:  subitem0 subitem1 subitem2
            row1:  subitem3 subitem4 subitem5
            row2:  subitem6 subitem7 subitem8
            row3:  subitem9 subitem10 subitem11
            ...
        */
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0 )
            {
                return null;
            }
            
            //create one row
            LoopListViewItem2 item = null;
            if(rowIndex % 2 == 1)
            {
                item = listView.NewListViewItem("ItemPrefab1");
            }
            else
            {
                item = listView.NewListViewItem("ItemPrefab2");
            }
            
            BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            //update all items in the row
            for (int i = 0;i< mItemCountPerRow; ++i)
            {
                int itemIndex = rowIndex * mItemCountPerRow + i;
                if(itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                //update the subitem content.
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
    }
}
