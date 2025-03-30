using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class TreeViewWithStickyHeadDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        TreeViewDataSourceMgr<ItemData> mTreeViewDataSourceMgr;
        //an helper class for TreeView item showing.
        TreeViewItemCountMgr mTreeItemCountMgr = new TreeViewItemCountMgr();
        //the sticky head item
        public TreeViewItemHead mStickeyHeadItem;
        RectTransform mStickeyHeadItemRf;
        float mStickeyHeadItemHeight = -1;
        ButtonPanelTreeViewSticky mButtonPanel;

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
           
            mStickeyHeadItemHeight = mStickeyHeadItem.GetComponent<RectTransform>().rect.height;
            mStickeyHeadItem.Init();
            mStickeyHeadItem.SetClickCallBack(this.OnExpandClicked);
            mStickeyHeadItemRf = mStickeyHeadItem.gameObject.GetComponent<RectTransform>();
            
            mLoopListView.ScrollRect.onValueChanged.AddListener(OnScrollContentPosChanged);
            UpdateStickeyHeadPos();
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mButtonPanel = new ButtonPanelTreeViewSticky();
            mButtonPanel.mLoopListView = mLoopListView;
            mButtonPanel.mTreeViewDataSourceMgr = mTreeViewDataSourceMgr;
            mButtonPanel.mTreeItemCountMgr = mTreeItemCountMgr;
            mButtonPanel.mStickeyHeadItemHeight = mStickeyHeadItemHeight;
            mButtonPanel.Start();             
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
                item.UserIntData1 = treeItemIndex;
                item.UserIntData2 = 0;
                itemScript.mText.text = treeViewItemData.mName;
                itemScript.SetItemData(treeItemIndex, countData.mIsExpand);
                return item;
            }
            else// if is a TreeChildItem
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
                item.UserIntData1 = treeItemIndex;
                item.UserIntData2 = childIndex;
                itemScript.SetItemData(itemData, treeItemIndex, childIndex);
                //float height = Random.Range(200, 400);//random the item's height, just for demo show
                //item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                return item;
            }
        }

        public void OnExpandClicked(int index)
        {
            mTreeItemCountMgr.ToggleItemExpand(index);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }
        
        void UpdateStickeyHeadPos()
        {
            bool isHeadItemVisible = mStickeyHeadItem.gameObject.activeSelf;
            int count = mLoopListView.ShownItemCount;
            if (count == 0)
            {
                if(isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            LoopListViewItem2 item0 = mLoopListView.GetShownItemByIndex(0);
            Vector3 topPos0 = mLoopListView.GetItemCornerPosInViewPort(item0, ItemCornerEnum.LeftTop);

            LoopListViewItem2 targetItem = null;
            float start = topPos0.y;
            float end = start - item0.ItemSizeWithPadding;
            int targetItemShownIndex = -1;
            if (start <= 0)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            if (end < 0)
            {
                targetItem = item0;
                targetItemShownIndex = 0;
            }
            else
            {
                for (int i = 1; i < count; ++i)
                {
                    LoopListViewItem2 item = mLoopListView.GetShownItemByIndexWithoutCheck(i);
                    start = end;
                    end = start - item.ItemSizeWithPadding;
                    if (start >= 0 && end <= 0)
                    {
                        targetItem = item;
                        targetItemShownIndex = i;
                        break;
                    }
                }
            }
            if (targetItem == null)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            int itemIndex = targetItem.UserIntData1;
            int childIndex = targetItem.UserIntData2;
            TreeViewItemCountData countData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (countData == null)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            if(countData.mIsExpand == false || countData.mChildCount == 0)
            {
                if (isHeadItemVisible)
                {
                    mStickeyHeadItem.gameObject.SetActive(false);
                }
                return;
            }
            if (isHeadItemVisible == false)
            {
                mStickeyHeadItem.gameObject.SetActive(true);
            }
            if(mStickeyHeadItem.TreeItemIndex != itemIndex)
            {
                TreeViewItemData<ItemData> treeViewItemData = mTreeViewDataSourceMgr.GetItemDataByIndex(itemIndex);
                mStickeyHeadItem.mText.text = treeViewItemData.mName;
                mStickeyHeadItem.SetItemData(itemIndex, countData.mIsExpand);
            }
            mStickeyHeadItem.gameObject.transform.localPosition = Vector3.zero;
            float lastChildPosAbs = -end;
            float lastPadding = targetItem.Padding;
            if(lastChildPosAbs - lastPadding >= mStickeyHeadItemHeight)
            {
                return;
            }
            for (int i = targetItemShownIndex+1; i < count; ++i)
            {
                LoopListViewItem2 item = mLoopListView.GetShownItemByIndexWithoutCheck(i);
                if (item.UserIntData1 != itemIndex)
                {
                    break;
                }
                lastChildPosAbs += item.ItemSizeWithPadding;
                lastPadding = item.Padding;
                if (lastChildPosAbs - lastPadding >= mStickeyHeadItemHeight)
                {
                    return;
                }
            }
            float y = mStickeyHeadItemHeight - (lastChildPosAbs - lastPadding);
            mStickeyHeadItemRf.anchoredPosition3D = new Vector3(0, y, 0);
        }

        void OnScrollContentPosChanged(Vector2 pos)
        {
            UpdateStickeyHeadPos();
        }
        
    }
}
