using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{   
    public class ContentFitterItem : MonoBehaviour
    {    
        public Text mNameText;
        ContentFitterItemData mItemData;
        int mItemDataIndex = -1;
        
        public Image mImageSelect;

        public int ItemIndex
        {
            get
            {
                return mItemDataIndex;
            }
            set
            {
                mItemDataIndex = value;
            }
        }      
         
        System.Action<int> mOnClickItemCallBack;
        public void Init(System.Action<int> OnClickItemCallBack = null)
        {
            ClickEventListener listener = ClickEventListener.Get(gameObject);
            listener.SetClickEventHandler(OnButtonClicked);
            mOnClickItemCallBack = OnClickItemCallBack;   
        }

        void OnButtonClicked(GameObject obj)
        {
            if(mOnClickItemCallBack != null)
            {
                mOnClickItemCallBack(mItemDataIndex);
            }
        }

        public void SetItemData(ContentFitterItemData itemData, int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mNameText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            Vector2 size = mNameText.GetComponent<RectTransform>().rect.size;
            RectTransform tf = gameObject.GetComponent<RectTransform>();
            float y = size.y+40;
            if (y < 75)
            {
                y = 75;
            }
            tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
        }

        public void SetItemSelected(bool isSelected)
        {
            if(mImageSelect != null)
            {
                mImageSelect.gameObject.SetActive(isSelected);
            }  
        }
    }    
}
