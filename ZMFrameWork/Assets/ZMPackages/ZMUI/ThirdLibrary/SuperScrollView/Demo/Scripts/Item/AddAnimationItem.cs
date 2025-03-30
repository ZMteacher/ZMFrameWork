using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{  
    public class AddAnimationItem : MonoBehaviour
    {    
        public Text mNameText;
        public Image mImageSelect;
        public RectTransform mContentRootTrans;
        SimpleItemData mItemData;
        Button mButton;  

        float mItemHeight = 120.0f;
        AnimationType mAnimaionType;
        float mAnimationValue = 1.0f;

        System.Action<int> mOnClickItemCallBack;
        public void Init(System.Action<int> OnClickItemCallBack = null)
        {
            mOnClickItemCallBack = OnClickItemCallBack;
            mButton = GetComponent<Button>();
            if(mButton != null)
            {
                mButton.onClick.AddListener(OnButtonClicked);
            }                   
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

        public void SetItemData(SimpleItemData itemData)
        {
            mItemData = itemData;
            mNameText.text = itemData.mName;
            ResetItemAnimation();
        }

        public void SetItemSelected(bool isSelected)
        {
            if(mImageSelect != null)
            {
                mImageSelect.gameObject.SetActive(isSelected);
            }  
        }

        public void SetAnimationType(AnimationType type)
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
                case AnimationType.Clip:
                {
                    ResetItemClip();
                    break;
                }
                case AnimationType.Fade:
                {
                    ResetItemFade();
                    break;
                }                
                case AnimationType.ClipFade:
                {
                    ResetItemClipFade();
                    break;
                }
                case AnimationType.SlideLeft:
                {
                    ResetItemSlideLeft();
                    break;
                }
                case AnimationType.SlideRight:
                {
                    ResetItemSlideRight();
                    break;
                }                
            }
        }

        void UpdateItemAnimation()
        {
            switch (mAnimaionType)
            {
                case AnimationType.Clip:
                {
                    UpdateItemClip();
                    break;
                }
                case AnimationType.Fade:
                {
                    UpdateItemFade();
                    break;
                }                
                case AnimationType.ClipFade:
                {
                    UpdateItemClipFade();
                    break;
                }
                case AnimationType.SlideLeft:
                {
                    UpdateItemSlideLeft();
                    break;
                }
                case AnimationType.SlideRight:
                {
                    UpdateItemSlideRight();
                    break;
                }                
            }
        }        

        void ResetItemClip()
        {
            SetHeight(mItemHeight);
        }

        void ResetItemFade()
        {
            SetAlpha(1.0f);
        }

        void ResetItemClipFade()
        {
            ResetItemClip();
            ResetItemFade();
        }      

        void ResetItemSlideLeft()
        {
            SetPosition(0);
        }

        void ResetItemSlideRight()
        {
            SetPosition(0);
        }   

        void UpdateItemClip()
        {
            SetHeight(mItemHeight * mAnimationValue);
        }

        void UpdateItemFade()
        {
            SetAlpha(mAnimationValue);
        }

        void UpdateItemClipFade()
        {
            UpdateItemClip();
            UpdateItemFade();
        }

        void UpdateItemSlideLeft()
        {
            RectTransform rectItem = GetComponent<RectTransform>();
            float itemWidth = rectItem.rect.width;
            float curX = itemWidth * (1 - mAnimationValue);
            SetPosition(curX);
        }

        void UpdateItemSlideRight()
        {
            RectTransform rectItem = GetComponent<RectTransform>();
            float itemWidth = rectItem.rect.width;
            float curX = -itemWidth * (1 - mAnimationValue);
            SetPosition(curX);
        }  

        void SetHeight(float height)
        {
            RectTransform rectTrans = GetComponent<RectTransform>();
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }        

        void SetAlpha(float alpha)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = alpha;
        }  

        void SetPosition(float positionX)
        {
            mContentRootTrans.anchoredPosition = new Vector2(positionX, 0);            
        }   
    }    
}
