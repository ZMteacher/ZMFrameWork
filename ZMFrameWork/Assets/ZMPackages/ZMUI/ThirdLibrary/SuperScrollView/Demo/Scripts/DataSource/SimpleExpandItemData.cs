using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class SimpleExpandItemData : ItemDataBase
    {
        public int mIndex;
        public string mName;
        public bool mIsExpand;
        public string mDesc;

        public override void Init(int index)
        {
            mIndex = index;
            mName = "Item" + index;
            int count = DescList.mLongStrList.Length;
            mDesc = DescList.mLongStrList[Random.Range(0, 99) % count];
        }

        public override void OnIndexChanged(int index)
        {
            mIndex = index;
            mName = "Item" + index;
        }
    }    
}