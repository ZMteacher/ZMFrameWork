using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ImageItemList : MonoBehaviour
    {
        public List<ImageItem> mItemList;

        public void Init()
        {
            foreach (ImageItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
