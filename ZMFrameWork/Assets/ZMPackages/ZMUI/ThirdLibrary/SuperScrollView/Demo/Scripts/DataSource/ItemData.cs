using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class ItemData : ItemDataBase
    {
        public int mIndex;
        public int mParentIndex = -1; //Only used by treeview.
        public string mName;
        public string mDesc;
        public string mDescExtend;
        public string mIcon;
        public int mStarCount;
        public bool mChecked;
        public bool mIsExpand;
        public float mSliderValue;
        public string mInputFieldText;
        public string mContentImage;        

        public override void Init(int index)
        {
            mIndex = index;
            mName = "Item" + index;
            int count = DescList.mStrList.Length;
            mDescExtend = DescList.mStrList[Random.Range(0, 99) % count];                
            mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 20));
            mStarCount = Random.Range(0, 6);
            int fileSize = Random.Range(20, 999);
            mDesc = fileSize.ToString()+"KB";
            mChecked = false;
            mIsExpand = false;      
            mSliderValue = Random.Range(0.1f, 0.9f);          
            mInputFieldText = "";       
            mContentImage = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 20));
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
            
            int count = DescList.mStrList.Length;
            mDescExtend = DescList.mStrList[Random.Range(0, 99) % count];                
            mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 20));
            mStarCount = Random.Range(0, 6);
            int fileSize = Random.Range(20, 999);
            mDesc = fileSize.ToString()+"KB";
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