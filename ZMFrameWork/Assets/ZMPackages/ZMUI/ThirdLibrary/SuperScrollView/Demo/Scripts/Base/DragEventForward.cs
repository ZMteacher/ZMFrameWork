using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

namespace SuperScrollView
{
    public class DragEventForward : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ScrollRect parentScrollRect;
        ScrollRect scrollRect;

        bool isParentHorizontal;
        bool isParentVertical;

        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            if (parentScrollRect == null)
            {
                parentScrollRect = transform.parent.GetComponentInParent<ScrollRect>();
            }
            if (parentScrollRect != null)
            {
                isParentHorizontal = parentScrollRect.horizontal;
                isParentVertical = parentScrollRect.vertical;
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(parentScrollRect != null)
            {
                parentScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (parentScrollRect == null)
            {
                return;
            }
            float dragAngle = Vector2.Angle(eventData.delta, Vector2.up);
            bool isHorizonalDrag = (dragAngle > 45f && dragAngle < 135f);
            if(isHorizonalDrag)
            {
                if(scrollRect.horizontal)
                {
                    parentScrollRect.vertical = false;
                }
                else
                {
                    parentScrollRect.horizontal = isParentHorizontal;
                    parentScrollRect.vertical = isParentVertical;
                    parentScrollRect.OnDrag(eventData);
                }
            }
            else
            {
                if (scrollRect.vertical)
                {
                    parentScrollRect.horizontal = false;
                }
                else
                {
                    parentScrollRect.horizontal = isParentHorizontal;
                    parentScrollRect.vertical = isParentVertical;
                    parentScrollRect.OnDrag(eventData);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (parentScrollRect != null)
            {
                parentScrollRect.OnEndDrag(eventData);
                parentScrollRect.horizontal = isParentHorizontal;
                parentScrollRect.vertical = isParentVertical;
            }
        }
    }
}
