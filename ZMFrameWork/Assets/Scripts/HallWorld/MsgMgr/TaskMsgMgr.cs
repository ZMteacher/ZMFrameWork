using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZMGC.Hall
{
    public class TaskMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            Debug.Log("TaskMsgMgr OnCreate>>>");
        }
        public void Test()
        {
            Debug.Log("TaskMsgMgr Test>>>");
        }
        public void OnDestroy()
        {
            Debug.Log("TaskMsgMgr OnDestroy>>>");
        }
    }
}