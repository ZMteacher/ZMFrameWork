using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class BaseVerticalLineItemList : MonoBehaviour
    {
        public List<BaseVerticalLineItem> mItemList;

        public void Init()
        {
            foreach (BaseVerticalLineItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
