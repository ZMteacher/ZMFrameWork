using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{  
    public class ExpandAnimationItem : MonoBehaviour
    {    
        public Text mNameText;
        public Text mDescText;
        public Image mImageSelect;
        public Text mClickTip;
        public Button mExpandButton;
        SimpleExpandItemData mItemData;
        Button mButton;

        float mItemMinHeight = 130.0f;
        float mItemMaxHeight = 0.0f;
        float mItemContentMinHeight = 0.0f;
        float mItemContentMaxHeight = 0.0f;
        float mAnimationTime = 1.0f;
        float mAnimationValue = 1.0f;
        float mItemGap = 30.0f;
        float mItemAverageHeight = 300.0f;
        System.Action<int> mOnClickItemCallBack;
        AnimationHelper mAnimationHelper;
        ExpandAnimationType mAnimaionType;

        public void Init(System.Action<int> OnClickItemCallBack, AnimationHelper animationHelper)
        {
            mOnClickItemCallBack = OnClickItemCallBack;
            mAnimationHelper = animationHelper;
            mButton = GetComponent<Button>();
            if(mButton != null)
            {
                mButton.onClick.AddListener(OnButtonClicked);
            }
            mExpandButton.onClick.AddListener(OnExpandButtonClicked);
        } 
        
        public void Init()
        {            
        }     

        void OnButtonClicked()
        {
            if(mOnClickItemCallBack != null)
            {
                mOnClickItemCallBack(mItemData.mId);
            }
        }

        void OnExpandButtonClicked()
        {
            mItemData.mIsExpand = !mItemData.mIsExpand;
            if (mItemData.mIsExpand)
            {
                mClickTip.text = "Shrink";
                mAnimationHelper.StartAnimation(mItemData.mId, 0, 1, mAnimationTime);
            }
            else
            {
                mClickTip.text = "Expand";
                mAnimationHelper.StartAnimation(mItemData.mId, 1, 0, mAnimationTime);
            }
        }

        public void SetItemData(SimpleExpandItemData itemData)
        {
            mItemData = itemData;
            mNameText.text = itemData.mName;
            mDescText.text = itemData.mDesc;
            ResetItemAnimation();            
        }

        public void SetItemSelected(bool isSelected)
        {
            if(mImageSelect != null)
            {
                mImageSelect.gameObject.SetActive(isSelected);
            }  
        }

        public void SetAnimationType(ExpandAnimationType type)
        {
            mAnimaionType = type;
        }

        public void SetAnimationValue(float animationValue)
        {
            mAnimationValue = animationValue;
            UpdateItemAnimation();            
        }

        void ResetItemAnimation()
        {
            switch (mAnimaionType)
            {
                case ExpandAnimationType.Clip:
                {
                    ResetItemClip();
                    break;
                }
                case ExpandAnimationType.Fade:
                {
                    ResetItemClipFade();
                    break;
                }               
                case ExpandAnimationType.ClipFade:
                {
                    ResetItemClipFade();
                    break;
                }
            }
        }

        void UpdateItemAnimation()
        {
            switch (mAnimaionType)
            {
                case ExpandAnimationType.Clip:
                {
                    UpdateItemClip();
                    break;
                }
                case ExpandAnimationType.Fade:
                {
                    UpdateItemFade();
                    break;
                }                
                case ExpandAnimationType.ClipFade:
                {
                    UpdateItemClipFade();
                    break;
                }                         
            }
        }  

        void ResetItemClip()
        {
            mDescText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            Vector2 size = mDescText.GetComponent<RectTransform>().rect.size;            

            RectTransform rectTransContenRoot = GetComponent<Transform>().Find("ContentRoot").GetComponent<RectTransform>();
            mItemContentMaxHeight = size.y + mItemGap;
            SetMinMaxHeight(rectTransContenRoot, mItemContentMinHeight, mItemContentMaxHeight);
            
            RectTransform rectTrans = GetComponent<RectTransform>();
            mItemMaxHeight = size.y + mItemMinHeight + mItemGap;
            SetMinMaxHeight(rectTrans, mItemMinHeight, mItemMaxHeight);
            mAnimationTime = (mItemMaxHeight - mItemMinHeight) / mItemAverageHeight;
        }        

        void ResetItemClipFade()
        {
            SetAlpha(1.0f);
            ResetItemClip();
        }

        void UpdateItemClip()
        {
            RectTransform rectTransContenRoot = GetComponent<Transform>().Find("ContentRoot").GetComponent<RectTransform>();
            SetHeight(rectTransContenRoot, mItemContentMinHeight, mItemContentMaxHeight, mAnimationValue);

            RectTransform rectTrans = GetComponent<RectTransform>();
            SetHeight(rectTrans, mItemMinHeight, mItemMaxHeight, mAnimationValue);  
        }

        void UpdateItemFade()
        {
            SetAlpha(mAnimationValue);

            RectTransform rectTransContenRoot = GetComponent<Transform>().Find("ContentRoot").GetComponent<RectTransform>();
            SetMinMaxHeight(rectTransContenRoot, mItemContentMinHeight, mItemContentMaxHeight);
            
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            SetMinMaxHeight(rectTrans, mItemMinHeight, mItemMaxHeight);
        }

        void UpdateItemClipFade()
        {
            SetAlpha(mAnimationValue);
            UpdateItemClip();
        }

        void SetHeight(RectTransform rectTrans, float minHight, float maxHight, float animationValue)
        {
            float curHeight = Mathf.Lerp(minHight, maxHight, animationValue);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, curHeight);
        }

        void SetMinMaxHeight(RectTransform rectTrans, float minHight, float maxHight)
        {            
            if(mItemData.mIsExpand)
            {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHight);
            }
            else
            {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minHight);
            }
        }

        void SetAlpha(float alpha)
        {
            CanvasGroup canvasGroup = GetComponent<Transform>().Find("ContentRoot").GetComponent<CanvasGroup>();
            canvasGroup.alpha = alpha;
        }
    }    
}
