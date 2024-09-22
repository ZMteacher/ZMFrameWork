using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Battle {
    public class BattleDataMgr : IDataBehaviour
    {
        public void OnCreate()
        { 
         Debug.Log("BattleDataMgr  OnCreate>>>");
        }

        public void OnDestroy()
        {
            Debug.Log("BattleDataMgr  OnDestroy>>>");
        }
    }
}