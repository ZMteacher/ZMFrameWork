using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ButtonPanelTreeViewSticky
    {
        public LoopListView2 mLoopListView;
        public TreeViewDataSourceMgr<ItemData> mTreeViewDataSourceMgr;
        public TreeViewItemCountMgr mTreeItemCountMgr;
        public float mStickeyHeadItemHeight = -1;
        Button mScrollToButton;
        InputField mScrollToInputItem;
        InputField mScrollToInputChild;
        Button mExpandAllButton;
        Button mCollapseAllButton;
        Button mAddButton;
        InputField mAddInputItem;
        InputField mAddInputChild;
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
            mAddInputChild = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputFieldChild").GetComponent<InputField>();
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
            scrollTo(itemIndex, childIndex);     
        }        

        void OnAddButtonClicked()
        {
            int itemIndex = 0;
            int childIndex = 0;
            if (int.TryParse(mAddInputItem.text, out itemIndex) == false)
            {
                return;
            }
            if (int.TryParse(mAddInputChild.text, out childIndex) == false)
            {
                childIndex = 0;
            }
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.mChildCount;
            if ((childIndex < 0)||(childIndex > childCount))
            {
                return;             
            }
            ItemData newData = mTreeViewDataSourceMgr.AddNewItemChild(itemIndex, childIndex);          
            newData.mDesc = newData.mDesc +" [New]";            
            mTreeItemCountMgr.SetItemChildCount(itemIndex, childCount+1);
            mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false); 
            mLoopListView.RefreshAllShownItem();
        }    

        void scrollTo(int itemIndex, int childIndex)
        {
            int finalIndex = 0;
            TreeViewItemCountData itemCountData = mTreeItemCountMgr.GetTreeItem(itemIndex);
            if (itemCountData == null)
            {
                return;
            }
            int childCount = itemCountData.mChildCount;
            if (itemCountData.mIsExpand == false || childCount == 0)
            {
                finalIndex = itemCountData.mBeginIndex;
                mLoopListView.MovePanelToItemIndex(finalIndex, 0);
            }
            else
            {
                if (childIndex >= childCount)
                {
                    return;             
                }
                finalIndex = itemCountData.mBeginIndex + childIndex + 1;
                mLoopListView.MovePanelToItemIndex(finalIndex, mStickeyHeadItemHeight);
            }  
        }    

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }   
    }
}
