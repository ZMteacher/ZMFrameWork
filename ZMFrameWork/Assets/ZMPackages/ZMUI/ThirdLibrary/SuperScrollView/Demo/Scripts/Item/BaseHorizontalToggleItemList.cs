using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class BaseHorizontalToggleItemList : MonoBehaviour
    {
        public BaseHorizontalToggleItem[] mItemList;

        public void Init()
        {
            foreach (BaseHorizontalToggleItem item in mItemList)
            {
                item.Init();
            }
        }

    }



}
