using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{        
    public class PageViewDemoScript : MonoBehaviour
    {
        class DotElem
        {
            public GameObject mDotElemRoot;
            public GameObject mDotNormal;
            public GameObject mDotSelect;
        }

        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 100;
        public RectTransform mParentView;
        public RectTransform mDotsRoot;
        public RectTransform mDotTemplate;     
        DataSourceMgr<ItemData> mDataSourceMgr;
        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        Button mBackButton;   

        int mPageCount = 10;
        int mMaxPageCount = 10;
        int mCountEachPage = 16;
        int mMaxItemCount = 0;
        List<DotElem> mDotElemList = new List<DotElem>();        
        List<RectTransform> mDotRectList = new List<RectTransform>(); 

        void Start()
        {
            mMaxItemCount = mMaxPageCount * mCountEachPage;
            if (mTotalDataCount > mMaxItemCount)
            {
                mTotalDataCount = mMaxItemCount;
            }
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            UpdatePageCount(mDataSourceMgr.TotalItemCount);
            InitAllDots();
            LoopListViewInitParam initParam = LoopListViewInitParam.CopyDefaultInitParam();
            initParam.mSnapVecThreshold = 99999;
            mLoopListView.mOnBeginDragAction = OnBeginDrag;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            mLoopListView.mOnSnapNearestChanged = OnSnapNearestChanged;
            mLoopListView.InitListView(mPageCount, OnGetItemByIndex, initParam);     
            InitButtonPanel();
        }

        public void InitButtonPanel()
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

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= mPageCount)
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            PageViewItem itemScript = item.GetComponent<PageViewItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            List<PageViewItemElem> elemList = itemScript.mElemItemList;
            int count = elemList.Count;
            int picBeginIndex = pageIndex * count;
            int i = 0;
            for(;i< count;++i)
            {
                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(picBeginIndex+i);
                if(itemData == null)
                {
                    break;
                }
                PageViewItemElem elem = elemList[i];
                elem.mRootObj.SetActive(true);
                elem.mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
                elem.mName.text = itemData.mName;
            }
            if(i < count)
            {
                for(;i< count;++i)
                {
                    elemList[i].mRootObj.SetActive(false);
                }
            }
            return item;
        }  

        void UpdatePageCount(int itemCount)
        {            
            int tmpPageCount = itemCount/mCountEachPage;
            if(tmpPageCount > mMaxPageCount)
            {
                tmpPageCount = mMaxPageCount;
            }
            else
            {
                if (itemCount % mCountEachPage > 0)
                {
                    tmpPageCount++;
                }    
            } 
            mPageCount = tmpPageCount;
        }

        void InitAllDots()
        {
            mDotTemplate.gameObject.SetActive(false);
            CreateDots(mPageCount);   
        }

        void CreateDots(int count)
        {                        
            for (int i = 0; i < count; i++)
            {                
                CreateOneDot(mDotsRoot, mDotTemplate);                                  
            }                              
        }

        void CreateOneDot(RectTransform rectParent, RectTransform rectTemplate)
        {
            int dotIndex = mDotElemList.Count;
            GameObject obj = GameObject.Instantiate(rectTemplate.gameObject, rectParent);
            obj.gameObject.name = "dot" + dotIndex;
            obj.gameObject.SetActive(true);
            RectTransform rectTrans = obj.GetComponent<RectTransform>();
            rectTrans.localScale = Vector3.one;
            rectTrans.localEulerAngles = Vector3.zero;
            rectTrans.anchoredPosition3D = Vector3.zero;
            rectTrans.SetAsLastSibling();

            DotElem elem = new DotElem();
            elem.mDotElemRoot = obj;
            elem.mDotNormal = obj.transform.Find("DotNormal").gameObject;
            elem.mDotSelect = obj.transform.Find("DotSelect").gameObject;
            mDotElemList.Add(elem);
            ClickEventListener listener = ClickEventListener.Get(elem.mDotElemRoot);
            listener.SetClickEventHandler(delegate (GameObject tmpObj) { OnDotClicked(dotIndex); }); 
        }

        void OnDotClicked(int index)
        {
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            if (curNearestItemIndex < 0 || curNearestItemIndex >= mPageCount)
            {
                return;
            }
            if(index == curNearestItemIndex)
            {
                return;
            }
            mLoopListView.SetSnapTargetItemIndex(index);            
        }

        void UpdateAllDots()
        {
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            if(curNearestItemIndex < 0 || curNearestItemIndex >= mPageCount)
            {
                return;
            }
            int count = mDotElemList.Count;
            if(curNearestItemIndex >= count)
            {
                return;
            }
            RefreshAllDots(curNearestItemIndex);            
        }

        void RefreshAllDots(int selectedIndex)
        {
            for(int i = 0; i < mDotElemList.Count; ++i)
            {
                DotElem elem = mDotElemList[i];
                if(i != selectedIndex)
                {
                    elem.mDotNormal.SetActive(true);
                    elem.mDotSelect.SetActive(false);
                }
                else
                {
                    elem.mDotNormal.SetActive(false);
                    elem.mDotSelect.SetActive(true);
                }
            }
        }

        void ResetDots()
        {
            if(mPageCount == mDotElemList.Count)
            {
                return;
            }
            if(mPageCount > mDotElemList.Count)
            {
                int addCount = mPageCount-mDotElemList.Count;
                AppendDots(addCount);
            }
            else
            {
                int removeCount = mDotElemList.Count-mPageCount;
                RemoveDots(removeCount);
            }
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            RefreshAllDots(curNearestItemIndex);
        }

        void AppendDots(int count)
        {
            CreateDots(count);
        }

        void RemoveDots(int count)
        {
            while(count > 0)
            {
                int removeIndex = mDotElemList.Count - 1;
                DotElem elem = mDotElemList[removeIndex];
                mDotElemList.RemoveAt(removeIndex);
                GameObject.Destroy(elem.mDotElemRoot);
                count--;
            }
        }       

        void OnSnapNearestChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            UpdateAllDots();
        }  

        void OnBeginDrag()
        {
        }

        void OnDraging()
        {
        }

        void OnEndDrag()
        {
            float vec = mLoopListView.ScrollRect.velocity.x;
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(curNearestItemIndex);
            if(item == null)
            {
                mLoopListView.ClearSnapData();
                return;
            }
            if (Mathf.Abs(vec) < 50f)
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                return;
            }
            Vector3 pos = mLoopListView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftTop);
            if(pos.x > 0)
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex - 1);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                }
            }
            else if (pos.x < 0)
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex+1);
                }
            }
            else
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex-1);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex + 1);
                }
            }
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
            if (count > mMaxItemCount)
            {
                return;
            }
            if(count == mDataSourceMgr.TotalItemCount)
            {
                return;
            }
            mDataSourceMgr.SetDataTotalCount(count);
            UpdatePageCount(mDataSourceMgr.TotalItemCount);
            mLoopListView.SetListItemCount(mPageCount, false);
            mLoopListView.RefreshAllShownItem();
            ResetDots();
        }



        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0) || (itemIndex >= mDataSourceMgr.TotalItemCount))
            {
                return;
            }

            int tmpItemIndex = itemIndex + 1;
            int tmpPageIndex = tmpItemIndex / mCountEachPage;
            if (tmpItemIndex % mCountEachPage > 0)
            {
                tmpPageIndex++;
            }
            if (tmpPageIndex > 0)
            {
                tmpPageIndex--;
            }
            mLoopListView.MovePanelToItemIndex(tmpPageIndex, 0);
            mLoopListView.FinishSnapImmediately();
        }

        void OnAddButtonClicked()
        {       
            if(mDataSourceMgr.TotalItemCount >= mMaxItemCount )
            {
                return;
            }             
            ItemData newData = mDataSourceMgr.InsertData(mDataSourceMgr.TotalItemCount);            
            newData.mDesc = newData.mDesc +" [New]";
            UpdatePageCount(mDataSourceMgr.TotalItemCount);
            mLoopListView.SetListItemCount(mPageCount, false);
            mLoopListView.RefreshAllShownItem();
            ResetDots();
        }       

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }  
    }
}
