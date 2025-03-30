using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class SimpleItemData : ItemDataBase
    {
        public int mIndex;
        public int mParentIndex = -1; //Only used by treeview.
        public string mName;
        
        public override void Init(int index)
        {
            mIndex = index;
            mName = "Item" + index;
        }

        public override void Init(int index, int parentIndex = -1)
        {
            mIndex = index;
            mParentIndex = parentIndex;            
            if(parentIndex != -1)
            {
                mName = "Item" + parentIndex + ": Child"+ index;
            }
            else
            {
                mName = "Item" + index;
            }           
        }        

        public override void OnIndexChanged(int index)
        {
            mIndex = index;
            mName = "Item" + index;
        }

        public override void OnIndexChanged(int index, int parentIndex = -1)
        {
            mIndex = index;
            if(mParentIndex != -1)
            {
                mName = "Item" + mParentIndex + ": Child"+ index;
            }
            else
            {
                mName = "Item" + index;
            }
        }

        public override bool IsFilterMatched(string filterStr)
        {
            return mName.Contains(filterStr);
        }
    }
}