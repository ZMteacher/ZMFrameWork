using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class BaseRowColItemList : MonoBehaviour
    {
        public List<BaseRowColItem> mItemList;

        public void Init()
        {
            foreach (BaseRowColItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
