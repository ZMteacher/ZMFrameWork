using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ChatViewChangeViewportHeightScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public DragChangSizeScript mDragChangSizeScript;
        Button mScrollToButton;
        InputField mScrollToInput;        
        Button mAppendUser1Button;
        Button mAppendUser2Button;
        Button mBackButton;

        // Use this for initialization
        void Start()
        {
            mLoopListView.InitListView(ChatMsgDataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);
            mDragChangSizeScript.mOnDragEndAction = OnViewPortHeightChanged;
            //mDragChangSizeScript.mOnDraggingAction = OnViewPortHeightChanged;

            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);
            mAppendUser1Button = GameObject.Find("ButtonPanel/ButtonGroupAppend/AppendUser1Button").GetComponent<Button>();
            mAppendUser1Button.onClick.AddListener(OnAppendUser1ButtonClicked);
            mAppendUser2Button = GameObject.Find("ButtonPanel/ButtonGroupAppend/AppendUser2Button").GetComponent<Button>();
            mAppendUser2Button.onClick.AddListener(OnAppendUser2ButtonClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }      

        void OnViewPortHeightChanged()
        {
            mLoopListView.ResetListView(false);
        }
       
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= ChatMsgDataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }

            ChatMsg itemData = ChatMsgDataSourceMgr.Get.GetChatMsgByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (itemData.mPersonId == 0)
            {
                item = listView.NewListViewItem("ItemPrefab1");
            }
            else
            {
                item = listView.NewListViewItem("ItemPrefab2");
            }
            ChatViewItem itemScript = item.GetComponent<ChatViewItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData, index);
            return item;
        }

        void OnAppendUser1ButtonClicked()
        {
            ChatMsgDataSourceMgr.Get.AppendOneMsg(0);
            mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.MovePanelToItemIndex(ChatMsgDataSourceMgr.Get.TotalItemCount-1, 0);
        }

        void OnAppendUser2ButtonClicked()
        {
            ChatMsgDataSourceMgr.Get.AppendOneMsg(1);
            mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.MovePanelToItemIndex(ChatMsgDataSourceMgr.Get.TotalItemCount-1, 0);
        }

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0) || (itemIndex >= mLoopListView.ItemTotalCount))
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
