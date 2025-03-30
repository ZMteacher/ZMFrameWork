using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class IconTextItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        
        ItemData mItemData;
        int mItemDataIndex = -1;

        public void Init()
        {     
        }      
       
        public void SetItemData(ItemData itemData,int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName; 
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
        }     
    }
}
