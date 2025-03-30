using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SliderItemList : MonoBehaviour
    {
        public List<SliderItem> mItemList;

        public void Init()
        {
            foreach (SliderItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
