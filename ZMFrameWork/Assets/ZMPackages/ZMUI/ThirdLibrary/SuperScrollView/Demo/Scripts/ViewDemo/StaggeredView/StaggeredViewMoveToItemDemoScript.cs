using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class StaggeredViewMoveToItemDemoScript : MonoBehaviour
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
        Button mMoveToButton;
        InputField mMoveToInput;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            InitItemHeightArrayForDemo();
            GridViewLayoutParam param = new GridViewLayoutParam();
            param.mPadding1 = 16;
            param.mPadding2 = 16;
            param.mColumnOrRowCount = 3;
            //if you can know every item's size beforehand, you may set the OnGetItemSizeByIndex delegate that is to return the size and padding of a given itemIndex.
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, param, OnGetItemByIndex,null,OnGetItemSizeByIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);
            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);
            mMoveToButton = GameObject.Find("ButtonPanel/ButtonGroupMoveTo/MoveToButton").GetComponent<Button>();
            mMoveToInput = GameObject.Find("ButtonPanel/ButtonGroupMoveTo/MoveToInputField").GetComponent<InputField>();
            mMoveToButton.onClick.AddListener(OnMoveToItemButtonClicked);

        }

        LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView listView, int index)
        {
            if (index < 0 || index >= mTotalDataCount)
            {
                return null;
            }

            SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
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
            float itemHeight = mMinHeight + mItemHeightArrayForDemo[index % mItemHeightArrayForDemo.Length] * 10f;
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
            return item;
        }


        //to return the size and padding of a given itemIndex.
        (float,float) OnGetItemSizeByIndex(int itemIndex)
        {
            float itemHeight = mMinHeight + mItemHeightArrayForDemo[itemIndex % mItemHeightArrayForDemo.Length] * 10f;
            float itemPadding = 16;
            return (itemHeight, itemPadding);
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

        void OnMoveToItemButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mMoveToInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                return;
            }

            //to ensure the LoopStaggeredGridView have made layout up to the itemIndex
            mLoopListView.UpdateContentSizeUpToItemIndex(itemIndex);

            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }
    }
}
