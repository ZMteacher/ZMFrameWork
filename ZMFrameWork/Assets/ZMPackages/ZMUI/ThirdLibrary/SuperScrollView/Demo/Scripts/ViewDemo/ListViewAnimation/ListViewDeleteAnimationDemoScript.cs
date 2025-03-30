using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListViewDeleteAnimationDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<SimpleItemData> mDataSourceMgr;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mDeleteButton;
        InputField mDeleteInput;
        Button mBackButton;
        int mCurrentSelectItemId = -1;

        AnimationHelper mAnimationHelper = new AnimationHelper();
        public AnimationType mAnimaionType = AnimationType.SlideLeft;
        public float mAnimationTime = 1f;

        Color[] mItemColorArray;
        int mItemColorCount = 100;
        float mItemColorR = 55.0f;
        float mItemColorG = 58.0f;
        float mItemColorB = 67.0f;
        const float mColorMask = 255.0f;
        const float mColorRangeFrom = 0.0f;
        const float mColorRangeTo = 60.0f;

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mTotalDataCount);
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex);
            InitItemColorArray();
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

            mDeleteButton = GameObject.Find("ButtonPanel/ButtonGroupDelete/DeleteButton").GetComponent<Button>();
            mDeleteInput = GameObject.Find("ButtonPanel/ButtonGroupDelete/DeleteInputField").GetComponent<InputField>();
            mDeleteButton.onClick.AddListener(OnDeleteButtonClicked);

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }  

        void InitItemColorArray()
        {
            mItemColorArray = new Color[mItemColorCount];
            for (int i = 0; i < mItemColorCount; ++i)
            {
                float tmp = Random.Range(mColorRangeFrom, mColorRangeTo);
                float itemColorR = (mItemColorR+tmp)/mColorMask;
                float itemColorG = (mItemColorG+tmp)/mColorMask;
                float itemColorB = (mItemColorB+tmp)/mColorMask;
                mItemColorArray[i] =  new Color(itemColorR, itemColorG, itemColorB, 1.0f);
            }
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }

            SimpleItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            DeleteAnimationItem itemScript = item.GetComponent<DeleteAnimationItem>();
            UpdateItemColor(itemScript,itemData.mId);
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init(OnItemClicked);
            }
            item.ItemId = itemData.mId;
            itemScript.SetAnimationType(mAnimaionType);
            itemScript.SetItemData(itemData);
            itemScript.SetItemSelected(mCurrentSelectItemId == itemData.mId);

            float animationValue = mAnimationHelper.GetCurAnimationValue(itemData.mId);
            if (animationValue >= 0)
            {
                itemScript.SetAnimationValue(animationValue);
            }
            return item;
        }

        void Update()
        {
            mAnimationHelper.UpdateAllAnimation(Time.deltaTime);
            List<int> allAnimationKeys = mAnimationHelper.AllAnimationKeys;
            bool hasItemDeleted = false;
            if(allAnimationKeys.Count > 0)
            {
                foreach(int itemId in allAnimationKeys)
                {
                    float val = mAnimationHelper.GetCurAnimationValue(itemId);
                    LoopListViewItem2 item = mLoopListView.GetShownItemByItemId(itemId);
                    if (item != null)
                    {
                        item.GetComponent<DeleteAnimationItem>().SetAnimationValue(val);
                        mLoopListView.OnItemSizeChanged(item.ItemIndex);
                    }
                    if(mAnimationHelper.IsAnimationFinished(itemId))
                    {
                        hasItemDeleted = true;
                        mAnimationHelper.RemoveAnimation(itemId);
                        mDataSourceMgr.RemoveDataByItemId(itemId);
                    }
                }
            }
            if(hasItemDeleted)
            {
                mLoopListView.SetListItemCount(mDataSourceMgr.TotalItemCount, false);
                mLoopListView.RefreshAllShownItem();
            }
        }

        void UpdateItemColor(DeleteAnimationItem itemScript,int id)
        {              
            Image imageItem = itemScript.GetComponent<Image>();
            if( (mAnimaionType == AnimationType.SlideLeft)||(mAnimaionType == AnimationType.SlideRight))
            {
                Transform transform = itemScript.GetComponent<Transform>();
                imageItem = transform.GetChild(0).GetComponent<Image>();
            }
            imageItem.color = mItemColorArray[id % mItemColorCount];          
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

        void OnDeleteButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mDeleteInput.text, out itemIndex) == false)
            {
                return;
            }
            if ((itemIndex < 0) || (itemIndex > mDataSourceMgr.TotalItemCount))
            {
                return;
            } 
            SimpleItemData deleteData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
            mAnimationHelper.StartAnimation(deleteData.mId,1,0,mAnimationTime);
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
