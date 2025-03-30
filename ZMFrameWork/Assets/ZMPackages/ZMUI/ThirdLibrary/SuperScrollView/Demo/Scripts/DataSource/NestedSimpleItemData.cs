using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class NestedSimpleItemData : ItemDataBase
    {
        public string mName;
        public int mIndex;       
        public DataSourceMgr<SimpleItemData> mDataSourceMgr;
        int mNestedCount = 30;   
        static int[] mUseCount = 
        {  
            3,30,2,50,2,10,3,8,50,9,
            3,30,2,50,2,10,3,8,50,9,
            3,30,2,50,2,10,3,8,50,9,
            3,30,2,50,2,10,3,8,50,9,
            3,30,2,50,2,10,3,8,50,9,
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

            mDataSourceMgr = new DataSourceMgr<SimpleItemData>(mNestedCount);
        }

        public override void OnIndexChanged(int index)
        {
            mIndex = index;
            mName = "Item" + index;
        }        
    }  
}