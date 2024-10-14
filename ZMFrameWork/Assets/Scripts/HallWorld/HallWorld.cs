using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.ZMAsset;

namespace ZMGC.Hall
{
    /// <summary>
    /// 当前世界在构建完成时，该世界下的所属逻辑层、数据层、消息层脚本全都由框架统一构建，可自由获取。
    /// 当世界销毁时，该世界下的所属逻辑层、数据层、消息层脚本全都统一由框架从内存中释放，无法获取。
    /// 判断当前属于哪个世界，可通过 WorldManager.IsHallWorld/IsBattleWorld...
    /// HallWorld一般常驻内存不销毁，销毁的是游戏世界。因为大厅数据层一般存放的有用户各种数据和信息。
    /// </summary>
    public class HallWorld : World
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("HallWorld  OnCreate>>>");
            //大厅世界构建完成，弹出登录弹窗
            UIModule.PopUpWindow<LoginWindow>();
        }
        /// <summary>
        /// 从登录页进入大厅
        /// </summary>
        public static   void EnterHallWorldFormLogin()
        { 
            //从登录进入大厅流程：销毁所有弹窗，解除资源引用 =>  释放内存中无引用的资源 
            HallWorld.UIModule.DestroyAllWindow();
            ZMAsset.ClearResourcesAssets(false);
            //弹出大厅弹窗
            UIModule.PopUpWindow<HallWindow>();

            //测试从AssetBundle中加载场景
            Debug.Log("加载场景:Assets/Scenes/TestLoadScene.unity");
            AsyncOperation operation=  ZMAsset.LoadSceneAsync("Assets/Scenes/TestLoadScene.unity");
             
        }


        /// <summary>
        /// 从游戏内返回至大厅  初始化一些大厅数据和状态
        /// </summary>
        /// <param name="args">游戏退出携带的一些相关参数，如:是否在来一局，跳转至某界面</param>
        public static void EnterHallWorldFormGame(object args=null)
        {
            //退出游戏流程：销毁所有弹窗，解除资源引用 =>  释放内存中无引用的资源 
            HallWorld.UIModule.DestroyAllWindow();
            ZMAsset.ClearResourcesAssets(false);

            //弹出大厅弹窗
            HallWorld.UIModule.PopUpWindow<HallWindow>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("HallWorld  OnDestroy>>>");
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("HallWorld  OnDestroyPostProcess>>>");

        }
    }
}