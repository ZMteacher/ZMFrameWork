using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class GridViewSimpleFilterDemoScript : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;        
        InputField mFilterClickInput;
        Button mFilterButton;
        InputField mFilterInput;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        Button mBackButton;
        int mCurrentSelectItemId = -1;  

        List<SimpleItemData> mFilteredDataList = null;
        string mFilerStr;
        string mFilerClickStr;

        // Use this for initialization
        void Start()
        {           
            mFilterButton = GameObject.Find("GridViewRoot/InputFieldClick/Button").GetComponent<Button>();
            mFilterInput = GameObject.Find("GridViewRoot/InputField").GetComponent<InputField>();
            mFilterClickInput = GameObject.Find("GridViewRoot/InputFieldClick").GetComponent<InputField>();

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

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);

            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            mFilterInput.text = "";
            mFilterClickInput.text = "";
            UpdateFilteredDataList("");
            mLoopGridView.InitGridView(mFilteredDataList.Count, OnGetItemByRowColumn);
        }     

        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
        {
            if (index < 0 || index >= mFilteredDataList.Count)
            {
                return null;
            } 
            //get the data to showing            
            SimpleItemData itemData = mFilteredDataList[index];
            if(itemData == null)
            {
                return null;
            }

            /*
            get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopGridView Inspector Setting
            */
            LoopGridViewItem item = gridView.NewListViewItem("ItemPrefab");

            SimpleItem itemScript = item.GetComponent<SimpleItem>();//get your own component
            // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);// here to init the item, such as add button click event listener.
            }
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData);
            itemScript.SetItemSelected(mCurrentSelectItemId == itemData.mId);
            return item;
        }

        void OnItemClicked(int itemId)
        {
            mCurrentSelectItemId = itemId;
            mLoopGridView.RefreshAllShownItem();
        }

        void OnFilterButtonClicked()
        {
            string newFilter = mFilterClickInput.text;
            if(newFilter == mFilerClickStr)
            {
                return;
            }
            mCurrentSelectItemId = -1;
            mFilerClickStr = newFilter;
            UpdateFilteredDataList(mFilerClickStr);
            mLoopGridView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopGridView.RefreshAllShownItem();
        }

        void OnInputChanged(string value)
        {
            mCurrentSelectItemId = -1;
            string newFilter = value;
            mFilerStr = newFilter;
            UpdateFilteredDataList(mFilerStr);
            mLoopGridView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopGridView.RefreshAllShownItem();
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
            mLoopGridView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopGridView.RefreshAllShownItem();
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
            mLoopGridView.MovePanelToItemByIndex(itemIndex, 0);
        }

        void OnAddButtonClicked()
        {
            SimpleItemData newData = mDataSourceMgr.InsertData(mDataSourceMgr.TotalItemCount);
            UpdateFilteredDataList(mFilerStr);
            mLoopGridView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopGridView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
