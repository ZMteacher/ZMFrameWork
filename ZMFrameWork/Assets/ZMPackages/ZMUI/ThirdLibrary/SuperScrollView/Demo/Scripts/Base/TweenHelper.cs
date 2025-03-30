using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class TweenHelper : MonoBehaviour
    {
        public Vector3 StartPos { get; set; }
       
        public Vector3 EndPos { get; set; }

        public float TweenTotalTime { get; set; }

        public float TweenSpeed { get; set; }

        public RectTransform TweenTrans { get; set; }

        public float StartTweenTime { get; set; }

        public float TweenEndTime { get; set; }

        public bool IsTweenFinished { get; set; }

        public System.Action<TweenHelper> OnTweenFinished;

        bool mIsTweenFinishedHandlerAutoReset = true;


        public void SetTweenFinishedHandler(System.Action<TweenHelper> handler,bool autoReset)
        {
            mIsTweenFinishedHandlerAutoReset = autoReset;
            OnTweenFinished = handler;
        }


        // Update is called once per frame
        public bool DoUpdate()
        {
            float curTime = Time.time;
            float t = (curTime - StartTweenTime)/TweenTotalTime;
            if(t >= 1)
            {
                TweenTrans.anchoredPosition3D = EndPos;
                if(OnTweenFinished != null)
                {
                    OnTweenFinished(this);
                    if(mIsTweenFinishedHandlerAutoReset)
                    {
                        OnTweenFinished = null;
                    }
                }
                IsTweenFinished = true;
                enabled = false;
            }
            else
            {
                TweenTrans.anchoredPosition3D = Vector3.Lerp(StartPos,EndPos,t);
            }
            return IsTweenFinished;
        }

        public static TweenHelper TweenAnchorPosX(RectTransform trans, float endPosX, float speed)
        {
            TweenHelper tweenHelper = trans.gameObject.GetComponent<TweenHelper>();
            if (tweenHelper == null)
            {
                tweenHelper = trans.gameObject.AddComponent<TweenHelper>();
            }
            tweenHelper.TweenTrans = trans;
            tweenHelper.StartPos = trans.anchoredPosition3D;
            Vector3 endPos = trans.anchoredPosition3D;
            endPos.x = endPosX;
            tweenHelper.EndPos = endPos;
            tweenHelper.TweenSpeed = speed;
            float distance = Mathf.Abs(tweenHelper.EndPos.x - tweenHelper.StartPos.x);
            tweenHelper.TweenTotalTime = distance / speed;
            tweenHelper.StartTweenTime = Time.time;
            tweenHelper.TweenEndTime = tweenHelper.StartTweenTime + tweenHelper.TweenTotalTime;
            tweenHelper.enabled = true;
            tweenHelper.IsTweenFinished = false;
            return tweenHelper;
        }


        public static TweenHelper TweenAnchorPosY(RectTransform trans, float endPosY, float speed)
        {
            TweenHelper tweenHelper = trans.gameObject.GetComponent<TweenHelper>();
            if (tweenHelper == null)
            {
                tweenHelper = trans.gameObject.AddComponent<TweenHelper>();
            }
            tweenHelper.TweenTrans = trans;
            tweenHelper.StartPos = trans.anchoredPosition3D;
            Vector3 endPos = trans.anchoredPosition3D;
            endPos.y = endPosY;
            tweenHelper.EndPos = endPos;
            tweenHelper.TweenSpeed = speed;
            float distance = Mathf.Abs(tweenHelper.EndPos.y - tweenHelper.StartPos.y);
            tweenHelper.TweenTotalTime = distance / speed;
            tweenHelper.StartTweenTime = Time.time;
            tweenHelper.TweenEndTime = tweenHelper.StartTweenTime + tweenHelper.TweenTotalTime;
            tweenHelper.enabled = true;
            tweenHelper.IsTweenFinished = false;
            return tweenHelper;
        }


    }
}
