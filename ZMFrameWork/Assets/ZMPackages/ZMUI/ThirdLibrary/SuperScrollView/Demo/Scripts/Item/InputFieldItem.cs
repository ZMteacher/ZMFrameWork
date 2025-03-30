using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class InputFieldItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Text mDesc;
        public InputField mInputField;

        ItemData mItemData;
        int mItemDataIndex = -1;

        public void Init()
        {            
            mInputField.onValueChanged.AddListener(OnInputFieldValueChange);            
        }      
       
        public void SetItemData(ItemData itemData,int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mDesc.text = itemData.mDesc;
            mInputField.text = itemData.mInputFieldText;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
        }

        public void OnInputFieldValueChange(string text)
        {
            mItemData.mInputFieldText = text;           
        }
    }
}
