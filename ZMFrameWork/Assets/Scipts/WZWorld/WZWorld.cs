using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.WZ { 
    public class WZWorld :World
    {
        private WuZhangDataMgr mSKData;
        private WuZhangMsgMgr mSKMsg;
        private WuZhangLogicCtrl mSKLogic;
        public override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("WZWorld  OnCreate>>>");
            mSKData = GetExitsDataMgr<WuZhangDataMgr>();
            mSKMsg = GetExitsMsgMgr<WuZhangMsgMgr>();
            mSKLogic = GetExitsLogicCtrl<WuZhangLogicCtrl>();
            UIModule.Instance.PopUpWindow<WuZhangWindow>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            //释放游戏内所有已经资源
            //该销毁的销毁，该释放的释放
     
            
            Debug.Log("WZWorld  OnDestroy>>>");
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("WZWorld  OnDestroyPostProcess>>>");
            ZMGC.Hall.HallWorld.EnterHallWorldFormGame(args);
        }
    }
}