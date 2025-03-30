using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ToggleItemList : MonoBehaviour
    {
        public List<ToggleItem> mItemList;

        public void Init()
        {
            foreach (ToggleItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
