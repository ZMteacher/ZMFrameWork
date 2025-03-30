using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{   
    public class PageViewItemElem
    {
        public GameObject mRootObj;
        public Image mIcon;
        public Text mName;
    }

    public class PageViewItem : MonoBehaviour
    {     
        public List<PageViewItemElem> mElemItemList = new List<PageViewItemElem>();

        public void Init()
        {
            int childCount = transform.childCount;
            for(int i= 0;i<childCount;++i)
            {
                Transform tf = transform.GetChild(i);
                PageViewItemElem elem = new PageViewItemElem();
                elem.mRootObj = tf.gameObject;
                elem.mIcon = tf.Find("IconMask/Icon").GetComponent<Image>();
                elem.mName = tf.Find("Name").GetComponent<Text>();
                mElemItemList.Add(elem);
            }
        }
    }
}
