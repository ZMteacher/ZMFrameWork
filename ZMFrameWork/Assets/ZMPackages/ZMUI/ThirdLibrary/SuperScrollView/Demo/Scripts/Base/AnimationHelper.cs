using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class AnimationHelper
    {
        public class AnimationData
        {
            public float mCurValue = 0.0f;
            public float mTargetValue = 1.0f;
            public float mSpeed = 1.0f;
            public bool mIsFinished = false;
            public bool Update(float deltaTime)
            {
                mCurValue = mCurValue + mSpeed * deltaTime;
                if(mSpeed > 0 && mCurValue >= mTargetValue)
                {
                    mIsFinished = true;
                }
                else if(mSpeed < 0 && mCurValue <= mTargetValue)
                {
                    mIsFinished = true;
                }
                return mIsFinished;
            }
        }

        List<int> mAllAnimationKeys = new List<int>();
        Dictionary<int, AnimationData> mAnimationDataDict = new Dictionary<int, AnimationData>();

        public List<int> AllAnimationKeys
        {
            get
            {
                mAllAnimationKeys.Clear();
                mAllAnimationKeys.AddRange(mAnimationDataDict.Keys);
                return mAllAnimationKeys;
            }
        }

        public void StartAnimation(int itemId,float startValue,float targetValue,float totalTime,bool forceFromStart = false)
        {
            AnimationData data = null;
            if (mAnimationDataDict.TryGetValue(itemId, out data) == false)
            {
                data = new AnimationData();
                mAnimationDataDict[itemId] = data;
                data.mCurValue = startValue;
            }
            else
            {
                if(forceFromStart)
                {
                    data.mCurValue = startValue;
                }
            }
            data.mTargetValue = targetValue;
            data.mSpeed = 1.0f / totalTime;
            if(data.mTargetValue < data.mCurValue)
            {
                data.mSpeed = -data.mSpeed;
            }
        }

        public void RemoveAnimation(int itemId)
        {
            mAnimationDataDict.Remove(itemId);
        }

        public bool IsAnimationFinished(int itemId)
        {
            AnimationData data = null;
            if (mAnimationDataDict.TryGetValue(itemId, out data))
            {
                return data.mIsFinished;
            }
            else
            {
                return true;
            }
        }

        public void UpdateAllAnimation(float deltaTime)
        {
            foreach(KeyValuePair<int, AnimationData> data in mAnimationDataDict)
            {
                data.Value.Update(deltaTime);
            }
        }

        public float GetCurAnimationValue(int itemId)
        {
            AnimationData data = null;
            if(mAnimationDataDict.TryGetValue(itemId, out data))
            {
                float val = Mathf.Clamp01(data.mCurValue);
                return val;
            }
            else
            {
                return -1f;
            }
        }
    }
}
