using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{   
    public class SimpleItem : MonoBehaviour
    {    
        public Text mNameText;          
        SimpleItemData mItemData;
        
        public Image mImageSelect;
        Button mButton;  
         
        System.Action<int> mOnClickItemCallBack;
        public void Init(System.Action<int> OnClickItemCallBack = null)
        {
            mOnClickItemCallBack = OnClickItemCallBack;
            mButton = GetComponent<Button>();
            if(mButton != null)
            {
                mButton.onClick.AddListener(OnButtonClicked);
            }                
        } 
        
        public void Init()
        {            
        }     

        void OnButtonClicked()
        {
            if(mOnClickItemCallBack != null)
            {
                mOnClickItemCallBack(mItemData.mId);
            }
        }

        public void SetItemData(SimpleItemData itemData)
        {
            mItemData = itemData;
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
