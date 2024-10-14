using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.SK
{
    public class SKMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            Debug.Log("SKMsgMgr  OnCreate>>>");
        }

        public void OnDestroy()
        {
            Debug.Log("SKMsgMgr  OnDestroy>>>");
        }
    }
}