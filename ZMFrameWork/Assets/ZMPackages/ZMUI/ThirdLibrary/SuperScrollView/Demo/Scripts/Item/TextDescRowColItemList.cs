using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class TextDescRowColItemList : MonoBehaviour
    {
        public List<TextDescRowColItem> mItemList;

        public void Init()
        {
            foreach (TextDescRowColItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
