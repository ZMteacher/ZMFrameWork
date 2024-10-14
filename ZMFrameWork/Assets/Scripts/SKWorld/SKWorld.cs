using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.SK
{
    /// <summary>
    /// À´ø€”Œœ∑ ¿ΩÁ
    /// </summary>
    public class SKWorld : World
    {
        private SKDataMgr mSKData;
        private SKMsgMgr mSKMsg;
        private SKLogicCtrl mSKLogic;
        public override void OnCreate()
        {
            base.OnCreate();
            mSKData = GetExitsDataMgr<SKDataMgr>();
            mSKMsg = GetExitsMsgMgr<SKMsgMgr>();
            mSKLogic = GetExitsLogicCtrl<SKLogicCtrl>();
            UIModule.Instance.PopUpWindow<SKWindow>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            ZMGC.Hall.HallWorld.EnterHallWorldFormGame();
        }
    }
}