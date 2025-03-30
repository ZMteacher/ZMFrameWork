using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ButtonPanelMenu : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public static int lastSelectSceneArrayIndex = 0;
        static float[] lastSelectSceneMenuYPos = new float[] {0,920,1840,2760,3660};
        // Start is called before the first frame update
        void Start()
        {
            Vector3 pos = scrollRect.content.anchoredPosition3D;
            pos.y = lastSelectSceneMenuYPos[lastSelectSceneArrayIndex];
            scrollRect.content.anchoredPosition3D = pos;
        }
       
    }
}