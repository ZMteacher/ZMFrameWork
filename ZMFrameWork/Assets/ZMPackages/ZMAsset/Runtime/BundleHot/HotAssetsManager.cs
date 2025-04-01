/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAsset
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace ZM.ZMAsset
{
    /// <summary>
    /// 等待下载的模块
    /// </summary>
    public class WaitDownLoadModule
    {
        public BundleModuleEnum bundleModule;
        public bool checkAssetsVersion;
        public Action<BundleModuleEnum> startHot;
        public Action<BundleModuleEnum> hotFinish;
        public Action<BundleModuleEnum, float> hotAssetsProgressCallBack;
    }

    public class HotAssetsManager : IHotAssets
    {
        /// <summary>
        /// 最大并发下载线程个数
        /// </summary>
        private int MAX_THREAD_COUNT = 3;
        /// <summary>
        /// 所有热更资源模块
        /// </summary>
        private Dictionary<BundleModuleEnum, HotAssetsModule> mAllAssetsModuleDic = new Dictionary<BundleModuleEnum, HotAssetsModule>();

        /// <summary>
        /// 正在下载热更资源模块的字典
        /// </summary>
        private Dictionary<BundleModuleEnum, HotAssetsModule> mDownLoadingAssetsModuleDic = new Dictionary<BundleModuleEnum, HotAssetsModule>();
        /// <summary>
        /// 正在下载热更资源的列表
        /// </summary>
        private List<HotAssetsModule> mDownLoadAssetsModuleList = new List<HotAssetsModule>();
        /// <summary>
        /// 等待下载的队列
        /// </summary>
        private Queue<WaitDownLoadModule> mWaitDownLoadQueue = new Queue<WaitDownLoadModule>();
        /// <summary>
        /// 下载AssetBundle完成
        /// </summary>
        public static Action<HotFileInfo> DownLoadBundleFinish;

        public void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack, Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waiteDownLoad, bool isCheckAssetsVersion = true)
        {
            if (BundleSettings.Instance.bundleHotType==  BundleHotEnum.NoHot)
            {
                hotFinish?.Invoke(bundleModule);
                return;
            }

            //读取配置中的最大下载线程个数
            MAX_THREAD_COUNT = BundleSettings.Instance.MAX_THREAD_COUNT;

            HotAssetsModule assetsModule = GetOrNewAssetModule(bundleModule);
            //判断是否有闲置资源下载线程
            if (mDownLoadingAssetsModuleDic.Count<MAX_THREAD_COUNT)
            {
                if (!mDownLoadingAssetsModuleDic.ContainsKey(bundleModule))
                {
                    mDownLoadingAssetsModuleDic.Add(bundleModule,assetsModule);
                }
                if (!mDownLoadAssetsModuleList.Contains(assetsModule))
                {
                    mDownLoadAssetsModuleList.Add(assetsModule);
                }
                assetsModule.OnDownLoadAllAssetsFinish += HotModuleAssetsFinish;
                //开始热更资源
                assetsModule.StartHotAssets(()=> { MultipleThreadBalancing();startHotCallBack?.Invoke(bundleModule); },hotFinish, isCheckAssetsVersion);
            }
            else
            {
                waiteDownLoad?.Invoke(bundleModule);
                //把热更模块添加到等待下载队列
                mWaitDownLoadQueue.Enqueue(new WaitDownLoadModule { bundleModule=bundleModule,startHot=startHotCallBack,hotFinish=hotFinish, checkAssetsVersion = isCheckAssetsVersion });
            }
        }
        public HotAssetsModule GetOrNewAssetModule(BundleModuleEnum bundleModule)
        {
            HotAssetsModule assetsModule = null;
            if (mAllAssetsModuleDic.ContainsKey(bundleModule))
            {
                assetsModule = mAllAssetsModuleDic[bundleModule];
            }
            else
            {
                assetsModule = new HotAssetsModule(bundleModule,ZMAsset.Instance);
                mAllAssetsModuleDic.Add(bundleModule, assetsModule);
            }
            return assetsModule;
        }
  
        /// <summary>
        /// 检测资源版本是否需要热更
        /// </summary>
        /// <param name="bundleModule">热更模块</param>
        /// <param name="callBack">热更回调</param>
        public void  CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack)
        {
            if (BundleSettings.Instance.bundleHotType == BundleHotEnum.NoHot)
            {
                Debug.Log("NoHot加载模式，不需要热更");
                callBack?.Invoke(false, 0);
                return;
            }
            HotAssetsModule assetsModule = GetOrNewAssetModule(bundleModule);
            assetsModule.CheckAssetsVersion(async (isHot,sizem)=>
            {
               if (!isHot)
               { 
                   await ZMAsset.InitAssetsModule(bundleModule);
               }
               callBack?.Invoke(isHot, sizem);
           } );
        } 
        /// <summary>
        /// 获取热更模块
        /// </summary>
        /// <param name="bundleModule"></param>
        /// <returns></returns>
        public HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule)
        {
            if (mAllAssetsModuleDic.ContainsKey(bundleModule))
            {
                return mAllAssetsModuleDic[bundleModule];
            }
            return null;
        }
        /// <summary>
        /// 热更模块资源完成
        /// </summary>
        /// <param name="bundleModule"></param>
        private async void HotModuleAssetsFinish(BundleModuleEnum bundleModule)
        {
            //把下载完成的模块从下载中的字典中移除掉
            if (mDownLoadingAssetsModuleDic.ContainsKey(bundleModule))
            {
                HotAssetsModule assetsModule = mDownLoadingAssetsModuleDic[bundleModule];
                if (mDownLoadAssetsModuleList.Contains(assetsModule))
                {
                    mDownLoadAssetsModuleList.Remove(assetsModule);
                }
                mDownLoadingAssetsModuleDic.Remove(bundleModule);
            }

            //判断等待下载的队列中是否有等待热更的模块，如果有，就可以进行热更了，因为已经有下载线程空闲下来
            if (mWaitDownLoadQueue.Count>0)
            {
                WaitDownLoadModule downLoadModule= mWaitDownLoadQueue.Dequeue();
                HotAssets(downLoadModule.bundleModule, downLoadModule.startHot, downLoadModule.hotFinish,null,downLoadModule.checkAssetsVersion);
            }
            else
            {
                //在没有等待热更模块的情况下，并且已经有下载线程空闲下来了，
                //我们就需要把闲置下来的下载线程分配给其他正在热更的模块，增加该模块的热更速度
                MultipleThreadBalancing();
            }
            await ZMAsset.InitAssetsModule(bundleModule);
        }
        /// <summary>
        /// 多线程均衡
        /// </summary>
        public void MultipleThreadBalancing()
        {
            //获取当前正在下载热更资源模块的一个长度个数
            int count = mDownLoadingAssetsModuleDic.Count;
            //计算多线程均衡后的线程分配个数
            //以最大下载线程个数为3 举例子
            //1.  3/1=3 最大并发下载线程个数为3  （偶数）
            //2.  3/2=1.5 向上取整 2 1 （奇数）
            //3.  3/3= 1  每一个模块 都拥有一个下载线程 
            float threadCount= MAX_THREAD_COUNT * 1.0f / count;
            //主下载线程个数
            int mainThreadCount = 0;
            //通过(int) 进行强转  (int)强转：表示向下强转
            int threadBalancingCount = (int)threadCount;

            if ((int)threadCount< threadCount)
            {
                //向上取整
                mainThreadCount = Mathf.CeilToInt(threadCount);
                //向下取整
                threadBalancingCount = Mathf.FloorToInt(threadCount);
            }
            //多线程均衡
            int i = 0;
            foreach (var item in mDownLoadingAssetsModuleDic.Values)
            {
                if (mainThreadCount!=0&&i==0)
                {
                    item.SetDownLoadThreadCount(mainThreadCount);//设置主下载线程个数
                }
                else
                {
                    item.SetDownLoadThreadCount(threadBalancingCount);
                }
                i++;
            }
        }
        /// <summary>
        /// 主线程更新
        /// </summary>
        public void OnMainThreadUpdate()
        {
            for (int i = 0; i < mDownLoadAssetsModuleList.Count; i++)
            {
                mDownLoadAssetsModuleList[i].OnMainThreadUpdate();
            }
        }
    }
}