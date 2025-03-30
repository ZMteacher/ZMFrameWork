using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewFilterDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;        
        InputField mFilterClickInput;
        Button mFilterButton;
        InputField mFilterInput;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        InputField mAddInput;
        Button mBackButton;

        List<ItemData> mFilteredDataList = null;
        string mFilerStr;
        string mFilerClickStr;

        // Use this for initialization
        void Start()
        {           
            mFilterButton = GameObject.Find("ScrollViewRoot/InputFieldClick/Button").GetComponent<Button>();
            mFilterInput = GameObject.Find("ScrollViewRoot/InputField").GetComponent<InputField>();
            mFilterClickInput = GameObject.Find("ScrollViewRoot/InputFieldClick").GetComponent<InputField>();

            mFilterButton.onClick.AddListener(OnFilterButtonClicked);
            mFilterInput.onValueChanged.AddListener(OnInputChanged);

            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);

            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);

            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);
            mAddInput = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputField").GetComponent<InputField>();

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);

            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            mFilterInput.text = "";
            mFilterClickInput.text = "";
            UpdateFilteredDataList("");
            mLoopListView.InitListView(mFilteredDataList.Count, OnGetItemByIndex);
        }     
       
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mFilteredDataList.Count)
            {
                return null;
            } 

            ItemData itemData = mFilteredDataList[index];
            if(itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            BaseVerticalItem itemScript = item.GetComponent<BaseVerticalItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(itemData,index);
            return item;
        }

        void OnFilterButtonClicked()
        {
            string newFilter = mFilterClickInput.text;
            if(newFilter == mFilerClickStr)
            {
                return;
            }
            mFilerClickStr = newFilter;
            UpdateFilteredDataList(mFilerClickStr);
            mLoopListView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnInputChanged(string value)
        {
            string newFilter = value;
            mFilerStr = newFilter;
            UpdateFilteredDataList(mFilerStr);
            mLoopListView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

        //update the filtered data list with the filter string
        void UpdateFilteredDataList(string filterStr)
        {
            mFilteredDataList = mDataSourceMgr.GetFilteredItemList(filterStr);
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
            mDataSourceMgr.SetDataTotalCount(count);
            UpdateFilteredDataList(mFilerStr);
            mLoopListView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if ((itemIndex < 0) || (itemIndex >= mFilteredDataList.Count))
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mAddInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                return;
            }
            ItemData newData = mDataSourceMgr.InsertData(itemIndex);
            newData.mDesc = newData.mDesc + " [New]";
            UpdateFilteredDataList(mFilerStr);
            mLoopListView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
