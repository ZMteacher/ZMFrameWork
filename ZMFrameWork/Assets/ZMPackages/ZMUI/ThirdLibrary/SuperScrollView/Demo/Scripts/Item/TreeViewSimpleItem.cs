using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class TreeViewSimpleItem : MonoBehaviour
    {
        int mItemDataIndex = -1;
        int mChildDataIndex = -1;
        public Text mNameText;        
        SimpleItemData mItemData;

        public Image mImageSelect;
        Button mButton;  
        
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

        public int ItemChildIndex
        {
            get
            {
                return mChildDataIndex;
            }
            set
            {
                mChildDataIndex = value;
            }
        }      
         
        System.Action<int,int> mOnClickItemCallBack;
        public void Init(System.Action<int,int> OnClickItemCallBack = null)
        {
            mOnClickItemCallBack = OnClickItemCallBack;
            mButton = GetComponent<Button>();
            if(mButton != null)
            {
                mButton.onClick.AddListener(OnButtonClicked);
            }                
        }

        void OnButtonClicked()
        {
            if(mOnClickItemCallBack != null)
            {
                mOnClickItemCallBack(mItemDataIndex,mChildDataIndex);
            }
        }
      
        public void SetItemData(SimpleItemData itemData, int itemIndex,int childIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mChildDataIndex = childIndex;
            mNameText.text = itemData.mName;
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
