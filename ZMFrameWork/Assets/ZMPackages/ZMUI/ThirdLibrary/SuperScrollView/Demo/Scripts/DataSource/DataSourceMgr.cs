using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{     
    public class DataSourceMgr<T> where T : ItemDataBase, new()
    {
        List<T> mItemDataList = new List<T>();
        System.Action mOnRefreshFinished = null;
        System.Action mOnLoadMoreFinished = null;
        int mLoadMoreCount = 20;
        float mDataLoadLeftTime = 0;
        float mDataRefreshLeftTime = 0;
        bool mIsWaittingRefreshData = false;
        bool mIsWaitLoadingMoreData = false;

        public DataSourceMgr(int count)
        {
            DoRefreshDataSource(count);
        }        

        public T GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= mItemDataList.Count)
            {
                return null;
            }
            return mItemDataList[index];
        }       

        public int TotalItemCount
        {
            get
            {
                return mItemDataList.Count;
            }
        }

        public List<T> ItemDataList
        {
            get
            {
                return mItemDataList;
            }
        }

        public void RequestRefreshDataList(System.Action onReflushFinished)
        {
            mDataRefreshLeftTime = 1;
            mOnRefreshFinished = onReflushFinished;
            mIsWaittingRefreshData = true;
        }

        public void RequestLoadMoreDataList(int loadCount,System.Action onLoadMoreFinished)
        {
            mLoadMoreCount = loadCount;
            mDataLoadLeftTime = 1;
            mOnLoadMoreFinished = onLoadMoreFinished;
            mIsWaitLoadingMoreData = true;
        }

        public void Update()
        {
            if (mIsWaittingRefreshData)
            {
                mDataRefreshLeftTime -= Time.deltaTime;
                if (mDataRefreshLeftTime <= 0)
                {
                    mIsWaittingRefreshData = false;
                    DoRefreshDataSource(mItemDataList.Count);
                    if (mOnRefreshFinished != null)
                    {
                        mOnRefreshFinished();
                    }
                }
            }
            if (mIsWaitLoadingMoreData)
            {
                mDataLoadLeftTime -= Time.deltaTime;
                if (mDataLoadLeftTime <= 0)
                {
                    mIsWaitLoadingMoreData = false;
                    AppendData(mLoadMoreCount);
                    if (mOnLoadMoreFinished != null)
                    {
                        mOnLoadMoreFinished();
                    }
                }
            }
        }

        public void SetDataTotalCount(int count)
        {
            int curCount = mItemDataList.Count;
            if (count == curCount)
            {
                return;
            }
            if(count > curCount)
            {
                AppendData(count - curCount);
            }
            else
            {
                mItemDataList.RemoveRange(count, curCount - count);
            }
        }

        public void ExchangeData(int index1,int index2)
        {
            T tData1 = mItemDataList[index1];
            T tData2 = mItemDataList[index2];
            mItemDataList[index1] = tData2;
            mItemDataList[index2] = tData1;
        }

        public void RemoveData(int index)
        {
            mItemDataList.RemoveAt(index);
        }

        public void RemoveDataByItemId(int itemId)
        {
            int index = 0;
            for (int i = 0; i < mItemDataList.Count; ++i)
            {
                if(mItemDataList[i].mId == itemId)
                {
                    index = i;
                    mItemDataList.RemoveAt(i);
                    break;
                }
            }

            for(int i = index; i < mItemDataList.Count; ++i)
            {
                mItemDataList[i].OnIndexChanged(i);
            }
        }

        public T InsertData(int index)
        {
            T newData = new T();
            newData.Init(index);
            mItemDataList.Insert(index, newData);
            for(int i = index+1; i < mItemDataList.Count; ++i)
            {
                mItemDataList[i].OnIndexChanged(i);
            }
            return newData;
        }

        public T InsertData(int index,T newData)
        {
            mItemDataList.Insert(index, newData);
            for (int i = index + 1; i < mItemDataList.Count; ++i)
            {
                mItemDataList[i].OnIndexChanged(i);
            }
            return newData;
        }

        void DoRefreshDataSource(int count)
        {
            mItemDataList.Clear();
            for (int i = 0; i < count; ++i)
            {
                T tData = new T();
                tData.Init(i);
                mItemDataList.Add(tData);
            }
        }

        public void AppendData(int addCount)
        {
            int count = mItemDataList.Count;
            for (int k = 0; k < addCount; ++k)
            {
                int i = k + count;
                T tData = new T();
                tData.Init(i);
                mItemDataList.Add(tData);
            }
        }

        public void AppendData(T itemData)
        {
            mItemDataList.Add(itemData);
        }

        public List<T> GetFilteredItemList(string filterStr)
        {
            if(string.IsNullOrEmpty(filterStr))
            {
                return mItemDataList;
            }
            List<T> filteredItemList = new List<T>();
            foreach(T itemData in mItemDataList)
            {
                if(itemData.IsFilterMatched(filterStr))
                {
                    filteredItemList.Add(itemData);
                }
            }
            return filteredItemList;
        }
    }
}