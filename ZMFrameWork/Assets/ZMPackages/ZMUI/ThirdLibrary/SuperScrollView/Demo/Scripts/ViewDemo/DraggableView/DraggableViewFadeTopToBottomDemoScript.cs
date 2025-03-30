using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class DraggableViewFadeTopToBottomDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;

        DataSourceMgr<DraggableItemData> mDataSourceMgr;
        Button mBackButton;

        Vector2 mDragOffset;
        float mListViewMoveVec = 0;
        LoopListViewItem2 mDraggingItem;
        Camera mCachedEventCamera;
        float mDragAlpha = 0.6f;
        float mListViewMoveSpeed = 600;
        Vector3[] mItemWorldCorners = new Vector3[4];

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<DraggableItemData>(mTotalDataCount);
            mLoopListView.InitListView(mDataSourceMgr.TotalItemCount, OnGetItemByIndex);
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
            {
                return null;
            }
            //get the data to showing
            DraggableItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            /*get a new item. Every item can use a different prefab, 
             the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting*/
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            //get your own component
            DraggableVerticalItem itemScript = item.GetComponent<DraggableVerticalItem>();
            //IsInitHandlerCalled is false means this item is new created but not fetched from pool.
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();// here to init the item, such as add button click event listener.
                DragEventHelperEx deh = itemScript.mDragBar.AddComponent<DragEventHelperEx>();
                deh.Param = item;
                deh.mOnBeginDragHandler = OnBeginDragItem;
                deh.mOnDragHandler = OnDragItem;
                deh.mOnEndDragHandler = OnEndDragItem;
            }
            CanvasGroup canvasGroup = item.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            //update the item’s content for showing, such as image,text.
            itemScript.SetItemData(itemData, index);
            return item;
        }

        public void OnBeginDragItem(PointerEventData eventData, object param)
        {
            LoopListViewItem2 draggingItem = param as LoopListViewItem2;
            mLoopListView.ScrollRect.StopMovement();
            LoopListViewItem2 orignFirstItem = mLoopListView.GetShownItemByIndex(0);
            if (orignFirstItem == null)
            {
                return;
            }

            mDraggingItem = draggingItem;
            //clone the dragging item.
            LoopListViewItem2 newItem = mLoopListView.NewListViewItem(draggingItem.ItemPrefabName);
            newItem.ItemId = draggingItem.ItemId;
            newItem.ItemIndex = draggingItem.ItemIndex;
            newItem.ItemPrefabName = draggingItem.ItemPrefabName;
            DraggableVerticalItem itemScript = newItem.GetComponent<DraggableVerticalItem>();
            DraggableItemData itemData = mDataSourceMgr.GetItemDataByIndex(mDraggingItem.ItemIndex);
            itemScript.SetItemData(itemData, newItem.ItemIndex);
            int indexInShownItemList = mLoopListView.GetIndexInShownItemList(draggingItem);
            //replace the shown item with the new created clone item.
            mLoopListView.SetShownItemByIndex(indexInShownItemList, newItem);

            mDraggingItem.CachedRectTransform.SetParent(mLoopListView.ViewPortTrans);

            RectTransform rtf = draggingItem.CachedRectTransform;
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rtf, eventData.position, eventData.pressEventCamera, out localPos))
            {
                mDragOffset = localPos;
            }
            SetDraggedPosition(eventData, draggingItem);
            rtf.SetAsLastSibling();
            CanvasGroup canvasGroup = mDraggingItem.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = mDragAlpha;
        }

        public void OnDragItem(PointerEventData eventData, object param)
        {
            LoopListViewItem2 draggingItem = param as LoopListViewItem2;
            SetDraggedPosition(eventData, draggingItem);

            draggingItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
            Vector3 topPos = mLoopListView.ViewPortTrans.InverseTransformPoint(mItemWorldCorners[1]);
            Vector3 downPos = mLoopListView.ViewPortTrans.InverseTransformPoint(mItemWorldCorners[0]);

            if (mLoopListView.ArrangeType == ListItemArrangeType.TopToBottom)
            {
                mListViewMoveVec = 0;
                if (topPos.y > 0)
                {
                    mListViewMoveVec = -mListViewMoveSpeed;
                }
                else if (downPos.y < -mLoopListView.ViewPortHeight)
                {
                    mListViewMoveVec = mListViewMoveSpeed;
                }
            }
            else
            {
                mListViewMoveVec = 0;
                Vector2 localPos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mLoopListView.ViewPortTrans, eventData.position, eventData.pressEventCamera, out localPos))
                {
                    if (topPos.y > mLoopListView.ViewPortHeight)
                    {
                        mListViewMoveVec = mListViewMoveSpeed;
                    }
                    else if (downPos.y < 0)
                    {
                        mListViewMoveVec = -mListViewMoveSpeed;
                    }
                }
            }
        }


        void Update()
        {
            if (Mathf.Abs(mListViewMoveVec) > 1)
            {
                float offset = mListViewMoveVec * Time.deltaTime;
                mLoopListView.MovePanelByOffset(offset);
            }
            
        }

        public void OnEndDragItem(PointerEventData eventData, object param)
        {
            mListViewMoveVec = 0;
            LoopListViewItem2 draggingItem = param as LoopListViewItem2;
            SetDraggedPosition(eventData, draggingItem);

            int count = mLoopListView.ShownItemCount;
            if (count == 0)
            {
                return;
            }
            Vector3 draggingItemPos = mLoopListView.ContainerTrans.InverseTransformPoint(draggingItem.CachedRectTransform.position);

            float selfPosY = draggingItemPos.y;
            int selfIndex = draggingItem.ItemIndex;
            int selfIndexInShownItems = mLoopListView.GetIndexInShownItemList(draggingItem);
            int insertBeforeIndex = -1;

            if (mLoopListView.ArrangeType == ListItemArrangeType.TopToBottom)
            {
                float draggingItemMiddleY = draggingItemPos.y - draggingItem.ItemSize / 2;
                LoopListViewItem2 curItem = mLoopListView.GetShownItemByIndex(0);
                float curY = curItem.TopY;
                float curMinDist = Mathf.Abs(draggingItemMiddleY - curY);
                insertBeforeIndex = curItem.ItemIndex;
                for (int i = 0; i < count; ++i)
                {
                    curItem = mLoopListView.GetShownItemByIndex(i);
                    curY = curY - curItem.ItemSizeWithPadding;
                    float dist = Mathf.Abs(draggingItemMiddleY - curY);
                    if (curMinDist > dist)
                    {
                        curMinDist = dist;
                        insertBeforeIndex = curItem.ItemIndex + 1;
                    }
                }
            }
            else
            {
                float draggingItemMiddleY = draggingItemPos.y + draggingItem.ItemSize / 2;
                LoopListViewItem2 curItem = mLoopListView.GetShownItemByIndex(0);
                float curY = curItem.BottomY;
                float curMinDist = Mathf.Abs(draggingItemMiddleY - curY);
                insertBeforeIndex = curItem.ItemIndex;
                for (int i = 0; i < count; ++i)
                {
                    curItem = mLoopListView.GetShownItemByIndex(i);
                    curY = curY + curItem.ItemSizeWithPadding;
                    float dist = Mathf.Abs(draggingItemMiddleY - curY);
                    if (curMinDist > dist)
                    {
                        curMinDist = dist;
                        insertBeforeIndex = curItem.ItemIndex + 1;
                    }
                }
            }

            mDraggingItem.CachedRectTransform.SetParent(mLoopListView.ContainerTrans);
            mDraggingItem.CachedRectTransform.SetAsLastSibling();

            if (insertBeforeIndex == draggingItem.ItemIndex || insertBeforeIndex == (draggingItem.ItemIndex + 1))
            {
                mLoopListView.RecycleItemImmediately(mDraggingItem);
                mDraggingItem = null;
                mLoopListView.RefreshAllShownItem();
                return;
            }


            if (mLoopListView.ArrangeType == ListItemArrangeType.TopToBottom)
            {
                if (insertBeforeIndex < draggingItem.ItemIndex)
                {
                    int targetIndex = insertBeforeIndex;
                    DraggableItemData tData = mDataSourceMgr.GetItemDataByIndex(selfIndex);
                    mDataSourceMgr.RemoveData(selfIndex);
                    mDataSourceMgr.InsertData(targetIndex, tData);
                }
                else
                {
                    int targetIndex = insertBeforeIndex - 1;
                    DraggableItemData tData = mDataSourceMgr.GetItemDataByIndex(selfIndex);
                    mDataSourceMgr.InsertData(targetIndex + 1, tData);
                    mDataSourceMgr.RemoveData(selfIndex);
                }
            }
            else
            {
                if (insertBeforeIndex < draggingItem.ItemIndex)
                {
                    int targetIndex = insertBeforeIndex;
                    DraggableItemData tData = mDataSourceMgr.GetItemDataByIndex(selfIndex);
                    mDataSourceMgr.RemoveData(selfIndex);
                    mDataSourceMgr.InsertData(targetIndex, tData);
                }
                else
                {
                    int targetIndex = insertBeforeIndex - 1;
                    DraggableItemData tData = mDataSourceMgr.GetItemDataByIndex(selfIndex);
                    mDataSourceMgr.InsertData(targetIndex + 1, tData);
                    mDataSourceMgr.RemoveData(selfIndex);
                }
            }

            mLoopListView.RecycleItemImmediately(mDraggingItem);
            mDraggingItem = null;
            mLoopListView.RefreshAllShownItem();
        }

        void SetDraggedPosition(PointerEventData eventData, LoopListViewItem2 draggingItem)
        {
            RectTransform rtf = draggingItem.CachedRectTransform;
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mLoopListView.ViewPortTrans, eventData.position, eventData.pressEventCamera, out localPos))
            {
                Vector2 old = rtf.anchoredPosition;
                if (mLoopListView.IsVertList)
                {
                    old.y = localPos.y - mDragOffset.y;
                }
                else
                {
                    old.x = localPos.x - mDragOffset.x;
                }
                rtf.anchoredPosition = old;
            }
        }


        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
