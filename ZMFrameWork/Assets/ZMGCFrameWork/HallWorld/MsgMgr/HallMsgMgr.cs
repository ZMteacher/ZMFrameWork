using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Hall
{
    public class HallMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            Debug.Log("HallMsgMgr OnCreate>>>");
        }

        public void OnDestroy()
        {
            Debug.Log("HallMsgMgr OnDestroy>>>");
        }
    }
}