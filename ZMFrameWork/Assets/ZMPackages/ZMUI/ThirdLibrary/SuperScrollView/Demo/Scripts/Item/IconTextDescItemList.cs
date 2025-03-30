using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class IconTextDescItemList : MonoBehaviour
    {
        public List<IconTextDescItem> mItemList;

        public void Init()
        {
            foreach (IconTextDescItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
