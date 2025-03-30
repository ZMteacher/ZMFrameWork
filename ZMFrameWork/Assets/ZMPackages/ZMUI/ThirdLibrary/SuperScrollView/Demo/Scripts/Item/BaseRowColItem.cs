using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class BaseRowColItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Image mStarIcon;
        public Text mStarCount;
        public Text mRowText;
        public Text mColumnText;
        public Text mDescText;
        public Color32 mRedStarColor = new Color32(236, 217, 103, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
  
        ItemData mItemData;
        int mItemDataIndex = -1;

        public void Init()
        {
            ClickEventListener listener = ClickEventListener.Get(mStarIcon.gameObject);
            listener.SetClickEventHandler(OnStarClicked);
        }

        void OnStarClicked(GameObject obj)
        {
            if (mItemData.mStarCount >= 99)
            {
                mItemData.mStarCount = 0;
            }
            else
            {
                mItemData.mStarCount = mItemData.mStarCount + 1;
            }
            SetStarCount(mItemData.mStarCount);
        }

        public void SetStarCount(int count)
        {
            mStarCount.text = count.ToString();
            if (count == 0)
            {
                mStarIcon.color = mGrayStarColor;
            }
            else
            {
                mStarIcon.color = mRedStarColor;
            }
        }

        public void SetItemData(ItemData itemData, int itemIndex,int row,int column)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mRowText.text = "Row: "+row;
            mColumnText.text = "Column: " + column;
            mDescText.text = itemData.mDesc;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            SetStarCount(itemData.mStarCount);
        }
    }
}
