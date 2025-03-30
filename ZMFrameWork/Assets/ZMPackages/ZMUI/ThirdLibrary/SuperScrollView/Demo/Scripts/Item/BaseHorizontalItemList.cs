using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class BaseHorizontalItemList : MonoBehaviour
    {
        public List<BaseHorizontalItem> mItemList;

        public void Init()
        {
            foreach (BaseHorizontalItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
