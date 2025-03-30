using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class DraggableVerticalItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public GameObject mDragBar;
        CanvasGroup mCanvasGroup;
        DraggableItemData mItemData;
        int mItemDataIndex = -1;

        public void Init()
        {           
        }        

        public void SetItemData(DraggableItemData itemData, int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
        }
    }
}
