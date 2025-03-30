using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewSimpleLoopDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mBackButton;
        int mCurrentSelectItemId = -1;
        public int mLoopCount = 10000;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mLoopCount);
            mLoopListView.InitListView(-1, OnGetItemByIndex);
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

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }  

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            int itemIndex = 0;
            if (index >= 0)
            {
                itemIndex = index % mLoopCount;
            }
            else
            {
                itemIndex = mLoopCount + ((index + 1) % mLoopCount) - 1;
            }

            SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
            if (itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            SimpleItem itemScript = item.GetComponent<SimpleItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);
            }
            itemScript.SetItemData(itemData);
            itemScript.SetItemSelected(mCurrentSelectItemId == itemData.mId);

            return item;
        }    

        void OnItemClicked(int itemId)
        {
            mCurrentSelectItemId = itemId;
            mLoopListView.RefreshAllShownItem();
        }

        void OnSetCountButtonClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count <= 0)
            {
                return;
            }
            mLoopCount = count;
            mDataSourceMgr.SetDataTotalCount(count);
            mLoopListView.RefreshAllShownItem();
        }

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
