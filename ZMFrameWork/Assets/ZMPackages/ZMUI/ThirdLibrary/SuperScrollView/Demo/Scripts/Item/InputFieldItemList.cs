using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class InputFieldItemList : MonoBehaviour
    {
        public List<InputFieldItem> mItemList;

        public void Init()
        {
            foreach (InputFieldItem item in mItemList)
            {
                item.Init();
            }
        }
    }   
}
