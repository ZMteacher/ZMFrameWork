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
        public static string FramworkPath = Application.dataPath + "/Scripts/";
        public static string[] mModuleNameArr = new string[] { "HallWorld", "BattleWorld", "SKWorld", "WZWorld" };
        public static string[] mMudileNameSpaceArr = new[] { "ZMGC.Hall", "ZMGC.Battle", "ZMGC.SK", "ZMGC.WZ" };
        public static string[] folderArr = new string[] { "/DataMgr/", "/MsgMgr/", "/LogicCtrl/" };
    }
}
