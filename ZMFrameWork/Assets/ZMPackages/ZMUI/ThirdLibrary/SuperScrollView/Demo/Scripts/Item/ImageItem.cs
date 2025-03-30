using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ImageItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Text mDesc;
        public Image mContentImage;
        
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
            mDesc.text = itemData.mDesc;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            mContentImage.sprite = ResManager.Get.GetSpriteByName(itemData.mContentImage);
        }     
    }
}
