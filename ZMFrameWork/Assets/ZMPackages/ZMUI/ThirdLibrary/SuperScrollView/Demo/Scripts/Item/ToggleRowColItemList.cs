using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ToggleRowColItemList : MonoBehaviour
    {
        public List<ToggleRowColItem> mItemList;

        public void Init()
        {
            foreach (ToggleRowColItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
