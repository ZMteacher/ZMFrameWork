using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpinDatePickerDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListViewYear;
        public LoopListView2 mLoopListViewMonth;
        public LoopListView2 mLoopListViewDay;
        public Color mColorReserved;
        public Color mColorSelected;
        public Text CurSelect;
        Button mBackButton;

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

        int mFirstYear = 1970;
        int mFirstMonth = 1;
        int mFirstDay = 1;
        int mYearCount = 1000;
        int mMonthCount = 12;        
        int mCurSelectedMonth = 1;
        int mCurSelectedDay = 1;
        int mCurSelectedYear = 2023;

        public int CurSelectedYear
        {
            get => mCurSelectedYear;
        }
        public int CurSelectedMonth
        {
            get => mCurSelectedMonth;
        }
        public int CurSelectedDay
        {
            get => mCurSelectedDay;
        }

        // Use this for initialization
        void Start()
        {
            //set all snap callback.
            mLoopListViewYear.mOnSnapNearestChanged = OnYearSnapTargetChanged;
            mLoopListViewMonth.mOnSnapNearestChanged = OnMonthSnapTargetChanged;
            mLoopListViewDay.mOnSnapNearestChanged = OnDaySnapTargetChanged;
            mLoopListViewYear.mOnSnapItemFinished = OnYearSnapTargetFinished;
            mLoopListViewMonth.mOnSnapItemFinished = OnMonthSnapTargetFinished;

            //init all superListView.
            mLoopListViewYear.InitListView(-1, OnGetItemByIndexForYear);
            mLoopListViewMonth.InitListView(-1, OnGetItemByIndexForMonth);
            mLoopListViewDay.InitListView(-1, OnGetItemByIndexForDay);

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
            int indexYear = mCurSelectedYear - mFirstYear - 1;
            mLoopListViewYear.MovePanelToItemIndex(indexYear, 0);
            mLoopListViewYear.FinishSnapImmediately();


            int mCurSelectedMonth = DateTime.Now.Month;
            int indexMonth = mCurSelectedMonth - mFirstMonth - 1;
            mLoopListViewMonth.MovePanelToItemIndex(indexMonth, 0);
            mLoopListViewMonth.FinishSnapImmediately();

            mCurSelectedDay = DateTime.Now.Day;
            int indexDay = mCurSelectedDay - mFirstDay -1;   
            mLoopListViewDay.MovePanelToItemIndex(indexDay, 0);
            mLoopListViewDay.FinishSnapImmediately();

        }

        void UpdateCurSelect()
        {
            int daysInMonth = DateTime.DaysInMonth(CurSelectedYear, CurSelectedMonth);
            if(mCurSelectedDay > daysInMonth)
            {
                mCurSelectedDay = daysInMonth;
            }
            DateTime curDate = new DateTime(CurSelectedYear, CurSelectedMonth, CurSelectedDay);
            string weekDayStr = mWeekDayNameArray[(int)(curDate.DayOfWeek)];
            CurSelect.text = string.Format("{0}, {1} {2:D2}, {3}", weekDayStr,mMonthNameArray[CurSelectedMonth - 1], CurSelectedDay, CurSelectedYear);
        }

        LoopListViewItem2 OnGetItemByIndexForYear(LoopListView2 listView, int index)
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
                val = index % mYearCount;
            }
            else
            {
                val = mYearCount + ((index + 1) % mYearCount) - 1;
            }
            val = val + mFirstYear;
            itemScript.Value = val;
            itemScript.mText.text = val.ToString();
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
            int dayCount = DateTime.DaysInMonth(CurSelectedYear, CurSelectedMonth);
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

        void OnYearSnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            SpinPickerItem itemScript = item.GetComponent<SpinPickerItem>();
            mCurSelectedYear = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
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

        void OnYearSnapTargetFinished(LoopListView2 listView, LoopListViewItem2 item)
        {
            LoopListViewItem2 item0 = mLoopListViewDay.GetShownItemByIndex(0);
            if(item0 == null)
            {
                return;
            }
            SpinPickerItem itemScript = item0.GetComponent<SpinPickerItem>();
            int index = itemScript.Value - 1;
            mLoopListViewDay.RefreshAllShownItemWithFirstIndex(index);
            mLoopListViewDay.RefreshAllShownItem();
        }

        void OnMonthSnapTargetFinished(LoopListView2 listView, LoopListViewItem2 item)
        {
            LoopListViewItem2 item0 = mLoopListViewDay.GetShownItemByIndex(0);
            if (item0 == null)
            {
                return;
            }
            SpinPickerItem itemScript = item0.GetComponent<SpinPickerItem>();
            int index = itemScript.Value - 1;
            mLoopListViewDay.RefreshAllShownItemWithFirstIndex(index);
            mLoopListViewDay.RefreshAllShownItem();
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
