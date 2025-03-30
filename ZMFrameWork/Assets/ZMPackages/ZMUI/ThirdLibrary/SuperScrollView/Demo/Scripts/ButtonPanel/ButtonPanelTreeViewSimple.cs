using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ButtonPanelTreeViewSimple
    {
        public LoopListView2 mLoopListView;
        public TreeViewDataSourceMgr<SimpleItemData> mTreeViewDataSourceMgr;
        public TreeViewItemCountMgr mTreeItemCountMgr;
        Button mScrollToButton;
        InputField mScrollToInputItem;
        InputField mScrollToInputChild;
        Button mExpandAllButton;
        Button mCollapseAllButton;
        Button mAddButton;
        InputField mAddInputItem;
        Button mBackButton;         

        public void Start()
        {
            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);
            mScrollToInputItem = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputFieldItem").GetComponent<InputField>();
            mScrollToInputChild = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputFieldChild").GetComponent<InputField>();
            
            mExpandAllButton = GameObject.Find("ButtonPanel/ButtonGroupExpandAll/ExpandAllButton").GetComponent<Button>();
            mExpandAllButton.onClick.AddListener(OnExpandAllButtonClicked);

            mCollapseAllButton = GameObject.Find("ButtonPanel/ButtonGroupCollapseAll/CollapseAllButton").GetComponent<Button>();
            mCollapseAllButton.onClick.AddListener(OnCollapseAllButtonClicked);

            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddInputItem = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputFieldItem").GetComponent<InputField>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked); 
        }     

        void OnExpandAllButtonClicked()
        {
            int count = mTreeItemCountMgr.TreeViewItemCount;
            for (int i = 0; i < count; ++i)
            {
                mTreeItemCountMgr.SetItemExpand(i, true);
            }
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnCollapseAllButtonClicked()
        {
            int count = mTreeItemCountMgr.TreeViewItemCount;
            for (int i = 0; i < count; ++i)
            {
                mTreeItemCountMgr.SetItemExpand(i, false);
            }
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
            mLoopListView.RefreshAllShownItem();
        }         

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            int childIndex = 0;            
            if (int.TryParse(mScrollToInputItem.text, out itemIndex) == false)
            {
                return;
            }
            if (int.TryParse(mScrollToInputChild.text, out childIndex) == false)
            {
                return;
            }
            if (childIndex < 0)
            {
                return;
            }
            mTreeItemCountMgr.SetItemExpand(itemIndex, true);
            ScrollTo(itemIndex, childIndex);               
        }            

        void OnAddButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mAddInputItem.text, out itemIndex) == false)
            {
                return;
            } 
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (itemCountData == null)
            {
                return;
            }      
            int childCount = itemCountData.mChildCount;      
            SimpleItemData newData = mTreeViewDataSourceMgr.AddNewItemChild(itemIndex, childCount);          
            mTreeItemCountMgr.SetItemChildCount(itemIndex, childCount+1);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false); 
            mLoopListView.RefreshAllShownItem();
        }

        void ScrollTo(int itemIndex, int childIndex)
        {
            int finalIndex = 0;
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if(itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.mChildCount;
            if (itemCountData.mIsExpand == false || childCount == 0 || childIndex == 0)
            {
                finalIndex = itemCountData.mBeginIndex;
            }
            else
            {
                if ((childIndex < 0)||(childIndex >= childCount))
                {
                    return;             
                }
                finalIndex = itemCountData.mBeginIndex + childIndex+1;
            }
            mLoopListView.MovePanelToItemIndex(finalIndex, 0);            
        }    

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }  
    }
}
