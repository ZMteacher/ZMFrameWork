using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class IconItemList : MonoBehaviour
    {
        public List<IconItem> mItemList;

        public void Init()
        {
            foreach (IconItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
