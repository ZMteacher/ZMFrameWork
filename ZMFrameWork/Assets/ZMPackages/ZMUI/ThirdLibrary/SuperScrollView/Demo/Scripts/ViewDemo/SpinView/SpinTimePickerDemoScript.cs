using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpinTimePickerDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListViewHour;
        public LoopListView2 mLoopListViewMinute;
        public Color mColorReserved;
        public Color mColorSelected;
        public Text CurSelect;
        Button mBackButton;
       
        int mFirstYear = 1970;
        int mFirstMonth = 1;
        int mFirstDay = 1;
        int mFirstHour = 0;
        int mFirstMinute = 0;
        int mHourCount = 24;
        int mMinuteCount = 60; 
        int mCurSelectedHour = 0;
        int mCurSelectedMinute = 0;
       
        public int CurSelectedHour
        {
            get => mCurSelectedHour;
        }

        public int CurSelectedMinute
        {
            get => mCurSelectedMinute;
        }

        // Use this for initialization
        void Start()
        {
            //set all snap callback.
            mLoopListViewHour.mOnSnapNearestChanged = OnHourSnapTargetChanged;
            mLoopListViewMinute.mOnSnapNearestChanged = OnMinuteSnapTargetChanged;

            //init all superListView.
            mLoopListViewHour.InitListView(-1, OnGetItemByIndexForHour);
            mLoopListViewMinute.InitListView(-1, OnGetItemByIndexForMinute);

            ScrollToCurrentDate();
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);
        }

        void ScrollToCurrentDate()
        {    
            int mCurSelectedYear = DateTime.Now.Year;
            int indexYear = mCurSelectedYear - mFirstYear-1;   

            int mCurSelectedMonth = DateTime.Now.Month;
            int indexMonth = mCurSelectedMonth - mFirstMonth -1;   

            int mCurSelectedDay = DateTime.Now.Day;
            int indexDay = mCurSelectedDay - mFirstDay -1;   

            mCurSelectedHour = DateTime.Now.Hour;
            int indexHour = mCurSelectedHour - mFirstHour -1;   
            mLoopListViewHour.MovePanelToItemIndex(indexHour, 0);
            mLoopListViewHour.FinishSnapImmediately();

            mCurSelectedMinute = DateTime.Now.Minute;
            int indexMinute = mCurSelectedMinute - mFirstMinute -1;   
            mLoopListViewMinute.MovePanelToItemIndex(indexMinute, 0);
            mLoopListViewMinute.FinishSnapImmediately();
        }

        void UpdateCurSelect()
        {
            CurSelect.text = string.Format("{0:D2}:{1:D2}",CurSelectedHour, CurSelectedMinute);
        }
      
        LoopListViewItem2 OnGetItemByIndexForMinute(LoopListView2 listView, int index)
        {
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            int val = 0;
            if (index >= 0)
            {
                val = index % mMinuteCount;
            }
            else
            {
                val = mMinuteCount + ((index + 1) % mMinuteCount) - 1;
            }
            val = val + mFirstMinute;
            itemScript.Value = val;
            itemScript.mText.text = string.Format("{0:D2}",val);
            return item;
        }

        LoopListViewItem2 OnGetItemByIndexForHour(LoopListView2 listView, int index)
        {
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            int val = 0;
            if(index >= 0)
            {
                val = index % mHourCount;
            }
            else
            {
                val = mHourCount + ((index + 1) % mHourCount) - 1;
            }
            val = val + mFirstHour;
            itemScript.Value = val;
            itemScript.mText.text = string.Format("{0:D2}",val);
            return item;
        }        

        void OnHourSnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            mCurSelectedHour = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
        }

        void OnMinuteSnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            mCurSelectedMinute = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
        }

        void OnListViewSnapTargetChanged(LoopListView2 listView, int targetIndex)
        {
            int count = listView.ShownItemCount;
            for (int i = 0; i < count; ++i)
            {
                LoopListViewItem2 item2 = listView.GetShownItemByIndex(i);
                SpinPickerItem itemScript = item2.GetComponent<SpinPickerItem>();
                if (i == targetIndex)
                {
                    itemScript.mText.color = mColorSelected;
                }
                else
                {
                    itemScript.mText.color = mColorReserved;
                }
            }
            UpdateCurSelect();
        }

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }
    }
}
