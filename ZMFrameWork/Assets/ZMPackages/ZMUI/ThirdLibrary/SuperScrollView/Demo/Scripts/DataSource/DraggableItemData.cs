using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class DraggableItemData : ItemDataBase
    {
        public int mIndex;
        public int mParentIndex = -1; //Only used by treeview.
        public string mName;
        public string mIcon;      

        public override void Init(int index)
        {
            mIndex = index;
            mName = "Item" + index;
            mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 20));
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
            mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 20));
        }    
    }    
}