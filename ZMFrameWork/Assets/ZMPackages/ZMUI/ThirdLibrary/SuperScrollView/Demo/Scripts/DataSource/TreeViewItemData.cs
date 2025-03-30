using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class TreeViewItemData<T> where T : ItemDataBase, new()
    {
        public string mName;
        //public string mIcon;
        public List<T> mChildItemDataList = new List<T>();

        public int ChildCount
        {
            get { return mChildItemDataList.Count; }
        }        

        public T AddNewItemChild(int index,int childIndex)
        {
            T childItemData = new T();
            childItemData.Init(childIndex, index);
            AddChildByIndex(index, childIndex, childItemData);
            return childItemData;
        }    

        public T GetItemChildDataByIndex(int childIndex)
        {            
            return GetChild(childIndex);
        }

        public void RefreshItemDataList(int index, int childItemCount)
        {
            mName = "Item" + index;
            //mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 20));
            int childCount = childItemCount;
            for (int childIndex = 0; childIndex < childCount; ++childIndex)
            {
                T childItemData = new T();
                childItemData.Init(childIndex,index);                                
                AddChild(childItemData);
            }
        }    

        public void AddChild(T data)
        {
            mChildItemDataList.Add(data);
        }

        public void AddChildByIndex(int index, int childIndex,T data)
        {
            if (childIndex < 0)
            {
                childIndex = 0;
            }
            if (childIndex >= mChildItemDataList.Count)
            {
                mChildItemDataList.Add(data);
            }
            else
            {
                mChildItemDataList.Insert(childIndex, data);
                for (int i = childIndex; i < mChildItemDataList.Count; ++i)
                {
                    T tmpChildItemData = mChildItemDataList[i];
                    tmpChildItemData.OnIndexChanged(i, index);
                }
            }
        }

        public bool RemoveChildByIndex(int index,int childIndex)
        {
            if (childIndex < 0 || childIndex >= mChildItemDataList.Count)
            {
                return false;
            }
            mChildItemDataList.RemoveAt(childIndex);
            for (int i = childIndex; i < mChildItemDataList.Count; ++i)
            {
                T tmpChildItemData = mChildItemDataList[i];
                tmpChildItemData.OnIndexChanged(i, index);
            }
            return true;
        }

        public T GetChild(int index)
        {
            if(index < 0 || index >= mChildItemDataList.Count)
            {
                return null;
            }
            return mChildItemDataList[index];
        }
    }
}