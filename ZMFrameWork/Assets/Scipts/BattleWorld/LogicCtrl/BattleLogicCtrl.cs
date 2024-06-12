using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Battle
{
    public class BattleLogicCtrl : ILogicBehaviour
    {
        public void OnCreate()
        {
            Debug.Log("BattleLogicCtrl  OnCreate>>>");
        }

        public void ExitbattleGame()
        {
            WorldManager.DestroyWorld<BattleWorld>();
        }
        public void OnDestroy()
        {
            Debug.Log("BattleLogicCtrl  OnDestroy>>>");
        }
    }
}
