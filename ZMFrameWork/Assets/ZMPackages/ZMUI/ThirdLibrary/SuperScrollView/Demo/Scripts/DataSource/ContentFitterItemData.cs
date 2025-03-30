using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{  
    public class ContentFitterItemData : ItemDataBase
    {
        public int mIndex;
        public string mName;

        public override void Init(int index)
        {
            mIndex = index;
            int count = DescList.mLongStrList.Length;
            mName = DescList.mLongStrList[Random.Range(0, 99) % count];
        }
       
        public override void OnIndexChanged(int index)
        {
            mIndex = index;
            int count = DescList.mLongStrList.Length;
            mName = DescList.mLongStrList[Random.Range(0, 99) % count];
        }

        public override bool IsFilterMatched(string filterStr)
        {
            return mName.Contains(filterStr);
        }
    }
}