using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ToggleItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Text mDescText;        
        public Toggle mToggle;

        ItemData mItemData;
        int mItemIndex = -1;

        public void Init()
        {
            mToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        void OnToggleValueChanged(bool check)
        {
            mItemData.mChecked = check;
        }

        public void SetItemData(ItemData itemData,int itemIndex)
        {
            mItemData = itemData;
            mItemIndex = itemIndex;
            mNameText.text = itemData.mName;
            mDescText.text = itemData.mDescExtend;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            mToggle.isOn = itemData.mChecked;
        }       
    }
}
