using UnityEngine;
using ZM.ZMAsset;
using ZMGC.Hall;

namespace ZMGC.SK
{
    /// <summary>
    /// ˫����Ϸ����
    /// </summary>
    public class SKWorld : World
    {
        private SKDataMgr mSKData;
        private SKMsgMgr mSKMsg;
        private SKLogicCtrl mSKLogic;
        public override void OnCreate()
        {
            base.OnCreate();
            //热更模块补充主热更程序中窗口数据
            var skWindCfg = ZMAsset.LoadScriptableObject<WindowConfig>("Assets/GameData/ShuangKou/CfgData/SKWindowConfig.asset");
            //在手机上不会触发调用
#if UNITY_EDITOR
            skWindCfg.GeneratorWindowConfig();
#endif
            UIModule.Instance.AddAOTWindowMetadata(skWindCfg);
            
            mSKData = GetExitsDataMgr<SKDataMgr>();
            mSKMsg = GetExitsMsgMgr<SKMsgMgr>();
            mSKLogic = GetExitsLogicCtrl<SKLogicCtrl>();
           
            UIModule.Instance.PopUpWindow<SKWindow>();
            SKGameDataMgr gameData = GetExitsDataMgr<SKGameDataMgr>();
            Debug.Log(gameData.userName);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            //进入大厅世界
            HallWorld.EnterHallWorldFormGame(args);
        }
    }
}