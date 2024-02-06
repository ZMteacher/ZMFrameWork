/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAssetFrameWork
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZM.AssetFrameWork
{
    public class HotUpdateManager : Singleton<HotUpdateManager>
    {
        private Main main;
        private HotAssetsWindow mHotAssetsWindow;
        /// <summary>
        /// 热更并且解压热更模块
        /// </summary>
        /// <param name="bundleModule"></param>
        public void HotAndUnPackAssets(BundleModuleEnum bundleModule,Main main)
        {
            if (this.main == null)
            {
                this.main = main;
            }
            mHotAssetsWindow = InstantiateResourcesObj<HotAssetsWindow>("HotAssetsWindow");
            //开始解压游戏内嵌资源
           IDecompressAssets decompress= ZMAssetsFrame.StartDeCompressBuiltinFile(bundleModule,()=> {
               //说明资源开启解压了
               if (Application.internetReachability == NetworkReachability.NotReachable)
               {
                   InstantiateResourcesObj<UpdateTipsWindow>("UpdateTipsWindow").InitView("当前无网络，请检测网络重试？", () => { NotNetButtonClick(bundleModule); }, () => { NotNetButtonClick(bundleModule); });
                   return;
               }
               else
               {
                   if (BundleSettings.Instance.bundleHotType == BundleHotEnum.Hot)
                        CheckAssetsVersion(bundleModule);
                   else
                   {
                       //如果不需要热更，说明用户已经热更过了，资源是最新的，直接进入游戏 TODO
                       OnHotFinishCallBack(bundleModule);
                   }
                }
            });
            //更新解压进度
            mHotAssetsWindow.ShowDecompressProgress(decompress);
        }

        public void NotNetButtonClick(BundleModuleEnum bundleModule)
        {
            //如果么有网络，弹出弹窗提示，提示用户没有网络请重试
            if (Application.internetReachability!= NetworkReachability.NotReachable)
            {
                CheckAssetsVersion(bundleModule);
            }
            else
            {

            }

        }
        public void CheckAssetsVersion(BundleModuleEnum bundleModule)
        {
            ZMAssetsFrame.CheckAssetsVersion(bundleModule,(isHot,sizem)=> {
                if (isHot )
                {
                    //当用户使用是流量的时候呢，需要询问用户是否需要更新资源
                    if (Application.internetReachability== NetworkReachability.ReachableViaCarrierDataNetwork||Application.platform==
                    RuntimePlatform.WindowsEditor||Application.platform==RuntimePlatform.OSXEditor)
                    {
                        //弹出选择弹窗，让用户决定是否更新
                        InstantiateResourcesObj<UpdateTipsWindow>("UpdateTipsWindow").
                        InitView("当前有"+sizem.ToString("F2")+"m,资源需要更新，是否更新",()=> {
                            //确认更新回调
                            StartHotAssets(bundleModule);
                        },
                        ()=> {
                            //退出游戏回调
                            Application.Quit();
                        });
                    }
                    else
                    {
                        StartHotAssets(bundleModule);
                    }
                }
                else
                {
                    //如果不需要热更，说明用户已经热更过了，资源是最新的，直接进入游戏 TODO
                    OnHotFinishCallBack(bundleModule);
                }
            });
        }
        /// <summary>
        /// 开始热更资源
        /// </summary>
        /// <param name="bundleModule"></param>
        public void StartHotAssets(BundleModuleEnum bundleModule)
        {
            ZMAssetsFrame.HotAssets(bundleModule, OnStartHotAssetsCallBack, OnHotFinishCallBack,null,false);
            //更新热更进度
            mHotAssetsWindow.ShowHotAssetsProgress(ZMAssetsFrame.GetHotAssetsModule(bundleModule));
        }
        /// <summary>
        /// 热更完成回调
        /// </summary>
        public void OnHotFinishCallBack(BundleModuleEnum bundleModule)
        {
            Debug.Log("OnHotFinishCallBack.....");
            mHotAssetsWindow.SetLoadGameEvn();
            main.StartCoroutine(InitGameEnv());
        }

        public void OnStartHotAssetsCallBack(BundleModuleEnum bundleModule)
        {

        }
        /// <summary>
        /// 初始化游戏环境
        /// </summary>
        /// <returns></returns>
         private IEnumerator InitGameEnv()
        {
            for (int i = 0; i < 100; i++)
            {
                mHotAssetsWindow.SetLoadGameEvn();
                mHotAssetsWindow.progressSlider.value = i / 100.0f;
                if (i==1)
                {
                    mHotAssetsWindow.progressText.text = "加载本地资源...";
                }
                else if (i==20)
                {
                    mHotAssetsWindow.progressText.text = "加载配置文件...";
                }
                else if (i == 70)
                {
                    mHotAssetsWindow.progressText.text = "加载AssetBUndle配置文件...";
                    //举例： AssetBundleManager.Instance.LoadAssetBundleConfig(BundleModuleEnum.Game);
                }
                else if (i == 90)
                {
                    mHotAssetsWindow.progressText.text = "加载游戏配置文件...";
                    LoadGameConfig();
                }
                else if (i == 99)
                {
                    mHotAssetsWindow.progressText.text = "加载地图场景...";
                }
                yield return null;

            }
            main.StartGame();
            GameObject.Destroy(mHotAssetsWindow.gameObject);
        }
        public void LoadGameConfig()
        {

        }
        public T InstantiateResourcesObj<T>(string prefabName)
        {
           return  GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(prefabName)).GetComponent<T>();
        }
    }
}