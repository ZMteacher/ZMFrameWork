using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class StaggeredViewSimpleTopToBottomDemoScript : MonoBehaviour
    {
        public LoopStaggeredGridView mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;
        int[] mItemHeightArrayForDemo = null;
        int mCount = 100;
        float mMinHeight = 260.0f;
        int mCurrentSelectItemId = -1;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mAddButton;
        Button mBackButton;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            InitItemHeightArrayForDemo();
            GridViewLayoutParam param = new GridViewLayoutParam();
            param.mPadding1 = 16;
            param.mPadding2 = 16;
            param.mColumnOrRowCount = 3;
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, param, OnGetItemByIndex);
            InitButtonPanel();              
        }

        void InitButtonPanel()
        {
            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);
            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }  

        LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView listView, int index)
        {
            if (index < 0 || index >= mTotalDataCount)
            {
                return null;
            }

            SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if(itemData == null)
            {
                return null;
            }

            //create one row
            LoopStaggeredGridViewItem item = listView.NewListViewItem("ItemPrefab");
            SimpleItem itemScript = item.GetComponent<SimpleItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);
            }
            itemScript.SetItemData(itemData);
            itemScript.SetItemSelected(mCurrentSelectItemId == itemData.mId);            

            //random set the item's height, just for example.
            float itemHeight = mMinHeight + mItemHeightArrayForDemo[index % mItemHeightArrayForDemo.Length]*10f;
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
            return item;
        }

        void InitItemHeightArrayForDemo()
        {
            mItemHeightArrayForDemo = new int[mCount];
            for (int i = 0; i < mItemHeightArrayForDemo.Length; ++i)
            {
                mItemHeightArrayForDemo[i] = Random.Range(0, 20);
            }
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
            if (count < 0)
            {
                return;
            }
            mDataSourceMgr.SetDataTotalCount(count);
            mLoopListView.SetListItemCount(count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnAddButtonClicked()
        {
            SimpleItemData newData = mDataSourceMgr.InsertData(mDataSourceMgr.TotalItemCount);
            mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
