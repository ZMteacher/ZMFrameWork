using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class IconTextItemList : MonoBehaviour
    {
        public List<IconTextItem> mItemList;

        public void Init()
        {
            foreach (IconTextItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
