using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.SK {
    public class SKDataMgr : IDataBehaviour
    {
        public void OnCreate()
        { 
         Debug.Log("SKDataMgr  OnCreate>>>");
        }

        public void OnDestroy()
        {
            Debug.Log("SKDataMgr  OnDestroy>>>");
        }
    }
}