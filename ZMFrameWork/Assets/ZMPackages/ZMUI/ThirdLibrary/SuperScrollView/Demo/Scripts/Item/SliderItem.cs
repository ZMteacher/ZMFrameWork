using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SliderItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Text mDesc;
        public Slider mSlider;
        
        ItemData mItemData;
        int mItemDataIndex = -1;        

        public void Init()
        {            
            mSlider.onValueChanged.AddListener(OnSliderValueChange);            
        }      
       
        public void SetItemData(ItemData itemData,int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mDesc.text = itemData.mDesc;
            mSlider.value = itemData.mSliderValue;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
        }

        public void OnSliderValueChange(float value)
        {
            mItemData.mSliderValue = value;           
        }
    }
}
