using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class TreeViewAddAndRemoveDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        TreeViewDataSourceMgr<ItemData> mTreeViewDataSourceMgr;

        // an helper class for TreeView item showing.
        TreeViewItemCountMgr mTreeItemCountMgr = new TreeViewItemCountMgr();



        Button mAddChildButton;
        InputField mAddChildInputItem;
        InputField mAddChildInputChild;

        Button mAddItemButton;
        InputField mAddItemInputItem;


        Button mRemoveChildButton;
        InputField mRemoveChildInputItem;
        InputField mRemoveChildInputChild;

        Button mRemoveItemButton;
        InputField mRemoveItemInputItem;


        // Use this for initialization
        void Start()
        {

            mTreeViewDataSourceMgr = new TreeViewDataSourceMgr<ItemData>();
            int count = mTreeViewDataSourceMgr.TreeViewItemCount;
            //tells mTreeItemCountMgr there are how many TreeItems and every TreeItem has how many ChildItems.
            for (int i = 0; i < count; ++i)
            {
                int childCount = mTreeViewDataSourceMgr.GetItemDataByIndex(i).ChildCount;
                //second param "true" tells mTreeItemCountMgr this TreeItem is in expand status, that is to say all its children are showing.
                mTreeItemCountMgr.AddTreeItem(childCount, true);
            }

            //initialize the InitListView
            //mTreeItemCountMgr.GetTotalItemAndChildCount() return the total items count in the TreeView, include all TreeItems and all TreeChildItems.
            mLoopListView.InitListView(mTreeItemCountMgr.GetTotalItemAndChildCount(), OnGetItemByIndex);


            mAddChildButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddChildInputItem = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputFieldItem").GetComponent<InputField>();
            mAddChildInputChild = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputFieldChild").GetComponent<InputField>();
            mAddChildButton.onClick.AddListener(OnAddChildButtonClicked);

            mAddItemButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddItemButton").GetComponent<Button>();
            mAddItemInputItem = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputFieldItem1").GetComponent<InputField>();
            mAddItemButton.onClick.AddListener(OnAddItemButtonClicked);


            mRemoveChildButton = GameObject.Find("ButtonPanel/ButtonGroupRemove/RemoveAtButton").GetComponent<Button>();
            mRemoveChildInputItem = GameObject.Find("ButtonPanel/ButtonGroupRemove/RemoveAtInputFieldItem").GetComponent<InputField>();
            mRemoveChildInputChild = GameObject.Find("ButtonPanel/ButtonGroupRemove/RemoveAtInputFieldChild").GetComponent<InputField>();
            mRemoveChildButton.onClick.AddListener(OnRemoveChildButtonClicked);

            mRemoveItemButton = GameObject.Find("ButtonPanel/ButtonGroupRemove/RemoveItemButton").GetComponent<Button>();
            mRemoveItemInputItem = GameObject.Find("ButtonPanel/ButtonGroupRemove/RemoveItemInputFieldItem").GetComponent<InputField>();
            mRemoveItemButton.onClick.AddListener(OnRemoveItemButtonClicked);


        }


        //when a TreeItem or TreeChildItem is getting in the scrollrect viewport, 
        //this method will be called with the item’ index as a parameter, 
        //to let you create the item and update its content.
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }

            /*to check the index'th item is a TreeItem or a TreeChildItem.for example,
             
             0  TreeItem0
             1      TreeChildItem0_0
             2      TreeChildItem0_1
             3      TreeChildItem0_2
             4      TreeChildItem0_3
             5  TreeItem1
             6      TreeChildItem1_0
             7      TreeChildItem1_1
             8      TreeChildItem1_2
             9  TreeItem2
             10     TreeChildItem2_0
             11     TreeChildItem2_1
             12     TreeChildItem2_2

             the first column value is the param 'index', for example, if index is 1,
             then we should return TreeChildItem0_0 to SuperScrollView, and if index is 5,
             then we should return TreeItem1 to SuperScrollView
            */

            TreeViewItemCountData countData = mTreeItemCountMgr.QueryTreeItemByTotalIndex(index);
            if (countData == null)
            {
                return null;
            }
            int treeItemIndex = countData.mTreeItemIndex;
            TreeViewItemData<ItemData> treeViewItemData = mTreeViewDataSourceMgr.GetItemDataByIndex(treeItemIndex);
            if (countData.IsChild(index) == false)// if is a TreeItem
            {
                //get a new TreeItem
                LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
                TreeViewItemHead itemScript = item.GetComponent<TreeViewItemHead>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                    itemScript.SetClickCallBack(this.OnExpandClicked);
                }
                //update the TreeItem's content
                itemScript.mText.text = treeViewItemData.mName;
                itemScript.SetItemData(treeItemIndex, countData.mIsExpand);
                return item;
            }
            else // if is a TreeChildItem
            {
                //childIndex is from 0 to ChildCount.
                //for example, TreeChildItem0_0 is the 0'th child of TreeItem0
                //and TreeChildItem1_2 is the 2'th child of TreeItem1
                int childIndex = countData.GetChildIndex(index);
                ItemData itemData = treeViewItemData.GetItemChildDataByIndex(childIndex);
                if (itemData == null)
                {
                    return null;
                }
                //get a new TreeChildItem
                LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab2");
                TreeViewItem itemScript = item.GetComponent<TreeViewItem>();
                if (item.IsInitHandlerCalled == false)
                {
                    item.IsInitHandlerCalled = true;
                    itemScript.Init();
                }
                //update the TreeChildItem's content
                itemScript.SetItemData(itemData, treeItemIndex, childIndex);
                return item;
            }
        }
        public void OnExpandClicked(int index)
        {
            mTreeItemCountMgr.ToggleItemExpand(index);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }


        void OnAddItemButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mAddItemInputItem.text, out itemIndex) == false)
            {
                return;
            }
           
            mTreeViewDataSourceMgr.AddNewItem(itemIndex);
            mTreeItemCountMgr.AddTreeItemBeforeIndex(itemIndex, 0,true);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }


        void OnAddChildButtonClicked()
        {
            int itemIndex = 0;
            int childIndex = 0;
            if (int.TryParse(mAddChildInputItem.text, out itemIndex) == false)
            {
                return;
            }
            if (int.TryParse(mAddChildInputChild.text, out childIndex) == false)
            {
                childIndex = 0;
            }
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.mChildCount;
            if ((childIndex < 0) || (childIndex > childCount))
            {
                return;
            }
            ItemData newData = mTreeViewDataSourceMgr.AddNewItemChild(itemIndex, childIndex);
            newData.mDesc = newData.mDesc + " [New]";
            mTreeItemCountMgr.SetItemChildCount(itemIndex, childCount + 1);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }




        void OnRemoveItemButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mRemoveItemInputItem.text, out itemIndex) == false)
            {
                return;
            }

            bool removeSucceed = mTreeViewDataSourceMgr.RemoveItem(itemIndex);
            if (removeSucceed == false)
            {
                return;
            }
            mTreeItemCountMgr.RemoveTreeItem(itemIndex);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }


        void OnRemoveChildButtonClicked()
        {
            int itemIndex = 0;
            int childIndex = 0;
            if (int.TryParse(mRemoveChildInputItem.text, out itemIndex) == false)
            {
                return;
            }
            if (int.TryParse(mRemoveChildInputChild.text, out childIndex) == false)
            {
                childIndex = 0;
            }
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.mChildCount;
            if (childCount == 0)
            {
                return;
            }
            bool removeSucceed = mTreeViewDataSourceMgr.RemoveItemChild(itemIndex, childIndex);
            if(removeSucceed == false)
            {
                return;
            }
            mTreeItemCountMgr.SetItemChildCount(itemIndex, childCount - 1);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }

    }
}
