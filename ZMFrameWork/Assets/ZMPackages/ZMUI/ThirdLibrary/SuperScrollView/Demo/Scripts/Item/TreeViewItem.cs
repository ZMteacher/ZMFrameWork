using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class TreeViewItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Image[] mStarArray;
        public Text mDesc;
        public Text mDescExtend;
        public Color32 mRedStarColor = new Color32(249, 227, 101, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);

        int mItemDataIndex = -1;
        int mChildDataIndex = -1;
        ItemData mItemData;
        
        public void Init()
        {
            for (int i = 0; i < mStarArray.Length; ++i)
            {
                int index = i;
                ClickEventListener listener = ClickEventListener.Get(mStarArray[i].gameObject);
                listener.SetClickEventHandler(delegate (GameObject obj) { OnStarClicked(index); });
            }
        }

        void OnStarClicked(int index)
        {           
            if (index == 0 && mItemData.mStarCount == 1)
            {
                mItemData.mStarCount = 0;
            }
            else
            {
                mItemData.mStarCount = index + 1;
            }
            SetStarCount(mItemData.mStarCount);
        }

        public void SetStarCount(int count)
        {
            int i = 0;
            for (; i < count; ++i)
            {
                mStarArray[i].color = mRedStarColor;
            }
            for (; i < mStarArray.Length; ++i)
            {
                mStarArray[i].color = mGrayStarColor;
            }
        }

        public void SetItemData(ItemData itemData, int itemIndex,int childIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mChildDataIndex = childIndex;
            mNameText.text = itemData.mName;
            mDesc.text = itemData.mDesc;
            mDescExtend.text = itemData.mDescExtend;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            SetStarCount(itemData.mStarCount);
        }
    }
}
