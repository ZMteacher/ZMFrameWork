using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

namespace SuperScrollView
{
    public class OneDirectionDragHelper : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        ScrollRect scrollRect;
        bool isHorizontal;
        bool isVertical;
        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            isHorizontal = scrollRect.horizontal;
            isVertical = scrollRect.vertical;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float dragAngle = Vector2.Angle(eventData.delta, Vector2.up);
            bool isHorizonalDrag = (dragAngle > 45f && dragAngle < 135f);
            if (isHorizonalDrag)
            {
                scrollRect.vertical = false;
            }
            else
            {
                scrollRect.horizontal = false;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            scrollRect.horizontal = isHorizontal;
            scrollRect.vertical = isVertical;
        }
    }
}
