using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{   
    public class NestedItemData : ItemDataBase
    {
        public string mName;
        public int mIndex;       
        public DataSourceMgr<ItemData> mDataSourceMgr;
        int mNestedCount = 30;   
        static int[] mUseCount = 
        {  
            2,3,30,4,50,2,20,3,8,20,
            2,3,30,4,50,2,20,3,8,20,
            2,3,30,4,50,2,20,3,8,20,
            2,3,30,4,50,2,20,3,8,20,
            2,3,30,4,50,2,20,3,8,20,
        };   

        public override void Init(int index)
        {
            mIndex = index;
            mName = "Item" + index;
            mNestedCount = Random.Range(1,30);
            if(index < mUseCount.Length)
            {
                mNestedCount = mUseCount[index];
            }

            mDataSourceMgr = new DataSourceMgr<ItemData>(mNestedCount);
        }

        public override void OnIndexChanged(int index)
        {
            mIndex = index;
            mName = "Item" + index;
        }        
    }  
}