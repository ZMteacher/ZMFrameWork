using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class ItemDataBase
    {
        static int mItemDataTotalCount = 0;
        public int mId;
        public ItemDataBase()
        {
            mItemDataTotalCount++;
            mId = mItemDataTotalCount;
        }
        public virtual void Init(int index)
        {
        }

        public virtual void Init(int index, int parentIndex)
        {
        }
       
        public virtual void OnIndexChanged(int index) 
        {
        }   

        public virtual void OnIndexChanged(int index, int parentIndex) 
        {
        }    

        public virtual bool IsFilterMatched(string filterStr)
        {
            return true;
        }
    }
}