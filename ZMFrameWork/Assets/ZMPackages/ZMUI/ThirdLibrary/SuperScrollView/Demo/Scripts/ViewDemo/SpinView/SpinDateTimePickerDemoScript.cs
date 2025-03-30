using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpinDateTimePickerDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListViewMonth;
        public LoopListView2 mLoopListViewDay;
        public LoopListView2 mLoopListViewHour;
        public LoopListView2 mLoopListViewMinute;
        public Color mColorReserved;
        public Color mColorSelected;
        public Text CurSelect;
        public int CurYear = 2023;
        Button mBackButton;

        static string[] mWeekDayNameArray = new string[]
        {
            "Sun",
            "Mon",
            "Tues",
            "Wed",
            "Thur",
            "Fri",
            "Sat",
        };

        static string[] mMonthNameArray = new string[]
        {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec",
        };
        int mFirstYear = 1970;
        int mFirstMonth = 1;
        int mFirstDay = 1;
        int mFirstHour = 0;
        int mFirstMinute = 0;
        int mMonthCount = 12; 
        int mHourCount = 24;
        int mMinuteCount = 60; 

        int mCurSelectedMonth = 1;
        int mCurSelectedDay = 1;
        int mCurSelectedHour = 0;
        int mCurSelectedMinute = 0;

        public int CurSelectedMonth
        {
            get => mCurSelectedMonth;
        }
        public int CurSelectedDay
        {
            get => mCurSelectedDay;
        }
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
            mLoopListViewMonth.mOnSnapNearestChanged = OnMonthSnapTargetChanged;
            mLoopListViewDay.mOnSnapNearestChanged = OnDaySnapTargetChanged;
            mLoopListViewHour.mOnSnapNearestChanged = OnHourSnapTargetChanged;
            mLoopListViewMinute.mOnSnapNearestChanged = OnMinuteSnapTargetChanged;

            //init all superListView.
            mLoopListViewMonth.InitListView(-1, OnGetItemByIndexForMonth);
            mLoopListViewDay.InitListView(-1, OnGetItemByIndexForDay);
            mLoopListViewHour.InitListView(-1, OnGetItemByIndexForHour);
            mLoopListViewMinute.InitListView(-1, OnGetItemByIndexForMinute);
            mLoopListViewMonth.mOnSnapItemFinished = OnMonthSnapTargetFinished;

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
            mLoopListViewMonth.MovePanelToItemIndex(indexMonth, 0);
            mLoopListViewMonth.FinishSnapImmediately();

            mCurSelectedDay = DateTime.Now.Day;
            int indexDay = mCurSelectedDay - mFirstDay -1;   
            mLoopListViewDay.MovePanelToItemIndex(indexDay, 0);
            mLoopListViewDay.FinishSnapImmediately();

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
            int daysInMonth = DateTime.DaysInMonth(CurYear, CurSelectedMonth);
            if (mCurSelectedDay > daysInMonth)
            {
                mCurSelectedDay = daysInMonth;
            }
            DateTime curDate = new DateTime(CurYear, CurSelectedMonth, CurSelectedDay);
            string weekDayStr = mWeekDayNameArray[(int)(curDate.DayOfWeek)];
            CurSelect.text = string.Format("{0}, {1} {2:D2}, {3}     {4:D2}:{5:D2}", weekDayStr, mMonthNameArray[CurSelectedMonth - 1], CurSelectedDay, CurYear, CurSelectedHour, CurSelectedMinute);
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

        LoopListViewItem2 OnGetItemByIndexForMonth(LoopListView2 listView, int index)
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
                val = index % mMonthCount;
            }
            else
            {
                val = mMonthCount + ((index+1) % mMonthCount)-1;
            }
            val = val + mFirstMonth;
            itemScript.Value = val;
            itemScript.mText.text = mMonthNameArray[val-1];
            return item;
        }

        LoopListViewItem2 OnGetItemByIndexForDay(LoopListView2 listView, int index)
        {
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            int dayCount = DateTime.DaysInMonth(CurYear, CurSelectedMonth);
            int val = 0;
            if (index >= 0)
            {
                val = index % dayCount;
            }
            else
            {
                val = dayCount + ((index + 1) % dayCount) - 1;
            }
            val = val + mFirstDay;
            itemScript.Value = val;
            itemScript.mText.text = string.Format("{0:D2}",val);
            return item;
        }

        void OnMonthSnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            mCurSelectedMonth = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
        }

        void OnDaySnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            mCurSelectedDay = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
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


        void OnMonthSnapTargetFinished(LoopListView2 listView, LoopListViewItem2 item)
        {
            LoopListViewItem2 item0 = mLoopListViewMonth.GetShownItemByIndex(0);
            if(item0 == null)
            {
                return;
            }
            SpinPickerItem itemScript = item0.GetComponent<SpinPickerItem>();
            int index = itemScript.Value - 1;
            mLoopListViewMonth.RefreshAllShownItemWithFirstIndex(index);
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
