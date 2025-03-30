using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ButtonPanelGridViewDelete
    {
        public LoopGridView mLoopGridView;
        public DataSourceMgr<ItemData> mDataSourceMgr;
        Button mSelectAllButton;
        Button mCancelAllButton;
        Button mDeleteButton;
        Button mBackButton;  

        public void Start()
        {
            mSelectAllButton = GameObject.Find("ButtonPanel/SelectAllButton").GetComponent<Button>();
            mSelectAllButton.onClick.AddListener(OnSelectAllButtonClicked);
            mCancelAllButton = GameObject.Find("ButtonPanel/CancelAllButton").GetComponent<Button>();
            mCancelAllButton.onClick.AddListener(OnCancelAllButtonClicked);
            mDeleteButton = GameObject.Find("ButtonPanel/DeleteButton").GetComponent<Button>();
            mDeleteButton.onClick.AddListener(OnDeleteButtonClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }              

        public void CheckAllItem(bool isCheck)
        {
            List<ItemData> itemDataList = mDataSourceMgr.ItemDataList;
            int count = itemDataList.Count;
            for (int i = 0; i < count; ++i)
            {
                itemDataList[i].mChecked = isCheck;
            }
        }

        public bool DeleteAllCheckedItem()
        {
            List<ItemData> itemDataList = mDataSourceMgr.ItemDataList;
            int oldCount = itemDataList.Count;
            itemDataList.RemoveAll(it => it.mChecked);
            return (oldCount != itemDataList.Count);
        }

        void OnSelectAllButtonClicked()
        {
            CheckAllItem(true);
            mLoopGridView.RefreshAllShownItem();
        }

        void OnCancelAllButtonClicked()
        {
            CheckAllItem(false);
            mLoopGridView.RefreshAllShownItem();
        }

        void OnDeleteButtonClicked()
        {
            bool isChanged = DeleteAllCheckedItem();
            if (isChanged == false)
            {
                return;
            }
            mLoopGridView.SetListItemCount(mDataSourceMgr.TotalItemCount, false);
            mLoopGridView.RefreshAllShownItem();
        }   
        
        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }  
    }
}
