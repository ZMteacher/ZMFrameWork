using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZMGC.Hall
{
    public class HallLoigcCtrl : ILogicBehaviour
    {
        public void OnCreate()
        {
            Debug.Log("HallLoigcCtrl OnCreate>>>");
        }

        public void EnterBattleWorld()
        {
            //销毁所有UI界面 弹出Loading加载界面
            //释放游戏内所有内存

            WorldManager.CreateWorld<Battle.BattleWorld>();

        }
        public void Test()
        {
            Debug.Log("HallLoigcCtrl Test>>>");
        }
        public void OnDestroy()
        {
            Debug.Log("HallLoigcCtrl OnDestroy>>>");
        }
    }
}