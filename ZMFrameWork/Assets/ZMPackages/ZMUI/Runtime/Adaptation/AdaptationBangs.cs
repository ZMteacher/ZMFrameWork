using UnityEngine;

/*----------------------------------------------------------------------------
* Title: 留海屏适配组件
*
* Author: 铸梦xy
*
* Date: 2024/09/01 14:15:58
*
* Description: 直接挂载到窗口的UIContent节点上即可(在需要适配的窗口上自行挂载，非全屏弹窗一般不许需要挂载)
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* GitHub：https://github.com/ZMteacher?tab=repositories
----------------------------------------------------------------------------*/
namespace ZM.UI
{
    public class AdaptationBangs : MonoBehaviour
    {
        /// <summary>
        /// 静态字段(提升性能，在游戏初始化时获取，放置每次界面打开都进行获取)
        /// </summary>
        private static float m_StatusBarHeight;

        /// <summary>
        /// 是否是横屏
        /// </summary>
        public static bool IsHorizontal { get; private set; } = true;

        /// <summary>
        /// 初始化适配信息 只计算一次，提升性能
        /// </summary>
        public static void InitializeAdaptation()
        {
            //默认为横向
            m_StatusBarHeight = Screen.safeArea.x;
            // 判断是否是纵向
            if (Screen.width < Screen.height)
            {
                m_StatusBarHeight = Screen.safeArea.y;
                IsHorizontal = false;
            }
        }

        private void Awake()
        {
            GeneratorAdaptation();
        }

        public void GeneratorAdaptation()
        {
            RectTransform rectTrans = transform.GetComponent<RectTransform>();
            if (IsHorizontal)
            {
                float anchorOffset = (m_StatusBarHeight / Screen.width);

                Vector2 anchorMinV2 = rectTrans.anchorMin;
                anchorMinV2.x = anchorOffset;
                rectTrans.anchorMin = anchorMinV2;

                Vector2 anchorMaxV2 = rectTrans.anchorMax;
                anchorMaxV2.x -= anchorOffset;
                rectTrans.anchorMax = anchorMaxV2;
            }
            else
            {
                float anchorOffset = (m_StatusBarHeight / Screen.height);

                Vector2 anchorMinV2 = rectTrans.anchorMin;
                anchorMinV2.y = anchorOffset;
                rectTrans.anchorMin = anchorMinV2;

                Vector2 anchorMaxV2 = rectTrans.anchorMax;
                anchorMaxV2.y -= anchorOffset;
                rectTrans.anchorMax = anchorMaxV2;
            }
        }
    }
}