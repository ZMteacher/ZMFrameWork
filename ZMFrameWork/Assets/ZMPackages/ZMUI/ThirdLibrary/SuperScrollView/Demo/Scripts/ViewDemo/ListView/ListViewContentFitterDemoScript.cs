using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewContentFitterDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ContentFitterItemData> mDataSourceMgr;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        Button mBackButton;
        int mCurrentSelectIndex = -1;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ContentFitterItemData>(mTotalDataCount);
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex);
            InitButtonPanel();           
        }

        void InitButtonPanel()
        {
            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);

            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);

            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }  

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }

            ContentFitterItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            ContentFitterItem itemScript = item.GetComponent<ContentFitterItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);
            }
            itemScript.SetItemData(itemData,index);
            itemScript.SetItemSelected(mCurrentSelectIndex == index);
            return item;
        }    

        void OnItemClicked(int index)
        {
            mCurrentSelectIndex = index;
            mLoopListView.RefreshAllShownItem();
        }

        void OnSetCountButtonClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count < 0)
            {
                return;
            }
            if(count <= mCurrentSelectIndex)
            {
                mCurrentSelectIndex = -1;
            }
            mDataSourceMgr.SetDataTotalCount(count);
            mLoopListView.SetListItemCount(count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if ((itemIndex < 0) || (itemIndex >= mDataSourceMgr.TotalItemCount))
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddButtonClicked()
        {
            ContentFitterItemData newData = mDataSourceMgr.InsertData(mDataSourceMgr.TotalItemCount);
            mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
