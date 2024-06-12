using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Battle { 
    public class BattleWorld :World
    {
        public override void OnCretae()
        {
            base.OnCretae();
            Debug.Log("BattleWorld  OnCretae>>>");
         }

        public override void OnDestroy()
        {
            base.OnDestroy();
            //释放游戏内所有已经资源
            //该销毁的销毁，该释放的释放
     
            
            Debug.Log("BattleWorld  OnDestroy>>>");
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("BattleWorld  OnDestroyPostProcess>>>");
            ZMGC.Hall.HallWorld.EnterHallWorldFormGame(args);
        }
    }
}