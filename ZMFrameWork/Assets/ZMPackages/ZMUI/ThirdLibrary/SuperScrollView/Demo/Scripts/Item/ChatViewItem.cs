using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ChatViewItem : MonoBehaviour
    {
        public Text mMsgText;
        public RectTransform mMsgPicMask;
        public Image mMsgPic;
        public Image mIcon;
        public Image mItemBg;
        public Image mArrow;
        public Text mIndexText;

        int mItemIndex = -1;
        float mMsgPicScaleX = 0.7f;
        float mMsgPicScaleY = 0.7f;

        public int ItemIndex
        {
            get
            {
                return mItemIndex;
            }
        }
        public void Init()
        {

        }
      
        public void SetItemData(ChatMsg itemData, int itemIndex)
        {
            mIndexText.text = itemIndex.ToString();
            PersonInfo person = ChatMsgDataSourceMgr.Get.GetPersonInfo(itemData.mPersonId);
            mItemIndex = itemIndex;
            if(itemData.mMsgType == MsgTypeEnum.Str)
            {
                mMsgPicMask.gameObject.SetActive(false);
                mMsgText.gameObject.SetActive(true);
                mMsgText.text = itemData.mSrtMsg;
                mMsgText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                mIcon.sprite = ResManager.Get.GetSpriteByName(person.mHeadIcon);
                mItemBg.gameObject.SetActive(true);
                Vector2 size = mItemBg.GetComponent<RectTransform>().sizeDelta;
                size.x = mMsgText.GetComponent<RectTransform>().sizeDelta.x + 20;
                size.y = mMsgText.GetComponent<RectTransform>().sizeDelta.y + 34;
                mItemBg.GetComponent<RectTransform>().sizeDelta = size;
                RectTransform tf = gameObject.GetComponent<RectTransform>();
                float y = size.y;
                if (y < 75)
                {
                    y = 75;
                }
                tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
            }
            else
            {
                mMsgPicMask.gameObject.SetActive(true);
                mMsgText.gameObject.SetActive(false);
                mMsgPic.sprite = ResManager.Get.GetSpriteByName(itemData.mPicMsgSpriteName);
                float w = mMsgPic.overrideSprite.rect.width;
                float h = mMsgPic.overrideSprite.rect.height;

                mMsgPicMask.sizeDelta = new Vector2(w *mMsgPicScaleX, h*mMsgPicScaleY);

                mIcon.sprite = ResManager.Get.GetSpriteByName(person.mHeadIcon);
                mItemBg.gameObject.SetActive(false);
                Vector2 size = Vector2.zero;
                size.x = mMsgPicMask.sizeDelta.x + 20;
                size.y = mMsgPicMask.sizeDelta.y + 20;
                RectTransform tf = gameObject.GetComponent<RectTransform>();
                float y = size.y;
                if (y < 75)
                {
                    y = 75;
                }
                tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);

            }
            

        }


    }
}
