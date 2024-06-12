using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ZM.GC
{
    public class GeneratorConfig
    {
        public static string FramworkPath = Application.dataPath + "/ThirdParty/ZMGCFramework/";
        public static string[] mModuleNameArr = new string[] { "HallWorld", "BattleWorld", "Fish3DWorld", "MajiangWorld" };
        public static string[] mMudileNameSpaceArr = new[] { "ZMGC.Hall", "ZMGC.Battle", "ZMGC.Fish3", "ZMGC.Majiang" };
        public static string[] folderArr = new string[] { "/DataMgr/", "/MsgMgr/", "/LogicCtrl/" };
    }
}
