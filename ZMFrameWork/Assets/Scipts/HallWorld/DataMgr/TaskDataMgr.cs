using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZMGC.Hall
{
    public class TaskDataMgr : IDataBehaviour
    {
        public void OnCreate()
        {
            Debug.Log("TaskDataMgr OnCreate>>>");
        }
        public void Test()
        {
            Debug.Log("TaskDataMgr Test>>>");
        }
        public void OnDestroy()
        {
            Debug.Log("TaskDataMgr OnDestroy>>>");
        }
    }
}