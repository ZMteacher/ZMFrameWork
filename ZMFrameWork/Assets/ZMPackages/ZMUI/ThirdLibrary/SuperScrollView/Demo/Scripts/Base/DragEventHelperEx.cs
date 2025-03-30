using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperScrollView
{
    public class DragEventHelperEx : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public delegate void OnDragEventHandler(PointerEventData eventData,object param);
        public OnDragEventHandler mOnBeginDragHandler;
        public OnDragEventHandler mOnDragHandler;
        public OnDragEventHandler mOnEndDragHandler;
        object mParam;


        public object Param
        {
            get => mParam;
            set { mParam = value;}
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (mOnBeginDragHandler != null)
            {
                mOnBeginDragHandler(eventData, mParam);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (mOnDragHandler != null)
            {
                mOnDragHandler(eventData, mParam);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (mOnEndDragHandler != null)
            {
                mOnEndDragHandler(eventData, mParam);
            }
        }
    }
}
