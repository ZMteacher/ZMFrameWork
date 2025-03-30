using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ExpandItem : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Image[] mStarArray;
        public Text mDesc;
        public GameObject mExpandContentRoot;
        public Text mClickTip;
        public Button mExpandButton;
        public Color32 mRedStarColor = new Color32(249, 227, 101, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
        
        bool mIsExpand;
        int mItemDataIndex = -1;        
        ItemData mItemData;        
        
        public void Init()
        {
            for (int i = 0; i < mStarArray.Length; ++i)
            {
                int index = i;
                ClickEventListener listener = ClickEventListener.Get(mStarArray[i].gameObject);
                listener.SetClickEventHandler(delegate (GameObject obj) { OnStarClicked(index); });
            }

            mExpandButton.onClick.AddListener( OnExpandButtonClicked );
        }

        public void OnExpandChanged()
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            if (mIsExpand)
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 264f);
                mExpandContentRoot.SetActive(true);
                mClickTip.text = "Shrink";
            }
            else
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 155f);
                mExpandContentRoot.SetActive(false);
                mClickTip.text = "Expand";
            }
        }

        void OnExpandButtonClicked()
        {
            mIsExpand = !mIsExpand;
            mItemData.mIsExpand = mIsExpand;
            OnExpandChanged();
            LoopListViewItem2 item2 = gameObject.GetComponent<LoopListViewItem2>();
            item2.ParentListView.OnItemSizeChanged(item2.ItemIndex);
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

        public void SetItemData(ItemData itemData, int itemIndex)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mDesc.text = itemData.mDesc;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            SetStarCount(itemData.mStarCount);
            mIsExpand = itemData.mIsExpand;
            OnExpandChanged();
        }


    }
}
