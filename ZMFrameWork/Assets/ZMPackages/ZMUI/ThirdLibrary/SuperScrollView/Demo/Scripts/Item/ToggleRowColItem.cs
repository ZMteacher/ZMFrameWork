using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ToggleRowColItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Image mStarIcon;
        public Text mStarCount;
        public Text mRowText;
        public Text mColumnText;
        public Color32 mRedStarColor = new Color32(236, 217, 103, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
        public Toggle mToggle;

        int mItemDataIndex = -1;
        ItemData mItemData;
        
        public void Init()
        {
            ClickEventListener listener = ClickEventListener.Get(mStarIcon.gameObject);
            listener.SetClickEventHandler(OnStarClicked);
            mToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        void OnToggleValueChanged(bool check)
        {
            mItemData.mChecked = check;
        }

        void OnStarClicked(GameObject obj)
        {
            if (mItemData.mStarCount == 5)
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

        public void SetItemData(ItemData itemData, int itemIndex, int row, int column)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mRowText.text = "Row: " + row;
            mColumnText.text = "Column: " + column;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            SetStarCount(itemData.mStarCount);
            mToggle.isOn = itemData.mChecked;
        }


    }
}
