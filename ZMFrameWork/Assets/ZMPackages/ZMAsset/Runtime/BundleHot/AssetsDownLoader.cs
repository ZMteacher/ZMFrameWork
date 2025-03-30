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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.ZMAsset
{
    public class DownLoadEventHandler
    {
        public DownLoadEvent downLoadEvent;//回调
        public HotFileInfo hotfileInfo;
    }

    /// <summary>
    /// 下载事件
    /// </summary>
    /// <param name="hotFile"></param>
    public delegate void DownLoadEvent(HotFileInfo hotFile);
    /// <summary>
    /// 多线程资源下载器
    /// </summary>
    public class AssetsDownLoader
    {
        /// <summary>
        /// 最大下载线程个数
        /// </summary>
        public int MAX_THREAD_COUNT = 3;
        /// <summary>
        /// 资源文件下载地址
        /// </summary>
        private string mAssetsDownLoadUrl;
        /// <summary>
        /// 热更文件储存路径
        /// </summary>
        private string mHotAssetsSavePath;
        /// <summary>
        /// 当前热更的资源模块
        /// </summary>
        private HotAssetsModule mCurHotAssetsModule;
        /// <summary>
        /// 文件下载队列
        /// </summary>
        private Queue<HotFileInfo> mDownLoadQueue;
        /// <summary>
        /// 文件下载成功回调
        /// </summary>
        private DownLoadEvent OnDownLoadSuccess;
        /// <summary>
        /// 文件下载失败回调
        /// </summary>
        private DownLoadEvent OnDownLoadFailed;
        /// <summary>
        /// 所有文件下载完成的回调
        /// </summary>
        private DownLoadEvent OnDownLoadFinish;
        /// <summary>
        /// 下载回调的列表
        /// </summary>
        private Queue<DownLoadEventHandler> mDownLoadEventQueue = new Queue<DownLoadEventHandler>();
        /// <summary>
        /// 当前所有正在下载的线程列表
        /// </summary>
        private List<DownLoadThread> mAllDownLoadThreadList = new List<DownLoadThread>();

        /// <summary>
        /// 资源下载器
        /// </summary>
        /// <param name="assetModule">资源下载模块</param>
        /// <param name="downLoadQueue">资源下载队列</param>
        /// <param name="downloadUrl">资源下载地址</param>
        /// <param name="hotAssetsSavePath">热更文件储存路径</param>
        /// <param name="downLoadSuccess">文件下载成功回调</param>
        /// <param name="downLoadFailed">文件下载失败或出错的回调</param>
        /// <param name="downLoadFinish">所有文件下载完成的回调</param>
        public AssetsDownLoader(HotAssetsModule assetModule, Queue<HotFileInfo> downLoadQueue, string downloadUrl, string hotAssetsSavePath,
            DownLoadEvent downLoadSuccess, DownLoadEvent downLoadFailed, DownLoadEvent downLoadFinish)
        {
            this.mCurHotAssetsModule = assetModule;
            this.mDownLoadQueue = downLoadQueue;
            this.mAssetsDownLoadUrl = downloadUrl;
            this.mHotAssetsSavePath = hotAssetsSavePath;
            this.OnDownLoadSuccess = downLoadSuccess;
            this.OnDownLoadFailed = downLoadFailed;
            this.OnDownLoadFinish = downLoadFinish;
        }

        public void StartThreadDownLoadQueue()
        {
            //根据最大的线程下载个数，开启基本下载通道
            for (int i = 0; i < MAX_THREAD_COUNT; i++)
            {
                if (mDownLoadQueue.Count > 0)
                {
                    Debug.Log("Start DownLoad AssetBundle MAX_THREAD_COUNT:" + MAX_THREAD_COUNT);
                    StartDownLoadNextBundle();
                }
            }
        }
        /// <summary>
        /// 开始下载下一个AssetBundle
        /// </summary>
        public void StartDownLoadNextBundle()
        {
            HotFileInfo hotFileInfo = mDownLoadQueue.Dequeue();
            DownLoadThread downLoadItem = new DownLoadThread(mCurHotAssetsModule, hotFileInfo, mAssetsDownLoadUrl, mHotAssetsSavePath);
            downLoadItem.StartDownLoad(DownLoadSuccess, DownLoadFailed);
            mAllDownLoadThreadList.Add(downLoadItem);
        }
        /// <summary>
        /// 开始下载下一个AssetBundle
        /// </summary>
        public void DownLoadNextBundle()
        {
            //如果当前下载的线程个数，大于最大的限制个数，我们就关闭当前下载通道
            if (mAllDownLoadThreadList.Count>MAX_THREAD_COUNT)
            {
                Debug.Log("DownLoadNextBundle Out MaxThreadCount,Close this DownLoad Channel...");
                return;
            }
            if (mDownLoadQueue.Count>0)
            {
                StartDownLoadNextBundle();
                if (mAllDownLoadThreadList.Count<MAX_THREAD_COUNT)
                {
                    //计算出正在待机的线程下载通道，把这些下载通道全部打开
                    int idleThreadCount = MAX_THREAD_COUNT - mAllDownLoadThreadList.Count;
                    for (int i = 0; i < idleThreadCount; i++)
                    {
                        if (mDownLoadQueue.Count>0)
                        {
                            StartDownLoadNextBundle();
                        }
                    }
                }
            }
            else
            {
                //如果下载中的文件也没有了，就说明我们所有文件都下载成功了
                if (mAllDownLoadThreadList.Count==0)
                {
                    TriggerCallBackInMainThread(new DownLoadEventHandler { downLoadEvent=OnDownLoadFinish });
                }
            }
        }
        /// <summary>
        /// AssetBundle文件下载成功
        /// </summary>
        /// <param name="downLoadThread"></param>
        /// <param name="hotFileInfo"></param>
        public void DownLoadSuccess(DownLoadThread downLoadThread,HotFileInfo hotFileInfo)
        {
            RemoveDownLoadThread(downLoadThread);
            //因为我们的文件 是在子线程中进行下载，所以说回调也是在子线程中触发。
            //我们要做的事情，就是把回调放到主线程中去调用。
            TriggerCallBackInMainThread(new DownLoadEventHandler { downLoadEvent = OnDownLoadSuccess, hotfileInfo = hotFileInfo });
            DownLoadNextBundle();
        }
        /// <summary>
        /// AssetBundle文件下载失败
        /// </summary>
        /// <param name="downLoadThread"></param>
        /// <param name="hotFileInfo"></param>
        public void DownLoadFailed(DownLoadThread downLoadThread, HotFileInfo hotFileInfo)
        {
            RemoveDownLoadThread(downLoadThread);
            TriggerCallBackInMainThread(new DownLoadEventHandler { downLoadEvent = OnDownLoadFailed, hotfileInfo = hotFileInfo });
            DownLoadNextBundle();
        }

        /// <summary>
        /// 在主线程中触发回调
        /// </summary>
        /// <param name="downLoadEventHandler"></param>
        public void TriggerCallBackInMainThread(DownLoadEventHandler downLoadEventHandler)
        {
            lock (mDownLoadEventQueue)
            {
                mDownLoadEventQueue.Enqueue(downLoadEventHandler);
            }
        }
        /// <summary>
        /// 主线程更新接口
        /// </summary>
        public void OnMainThreadUpdate()
        {
            if (mDownLoadEventQueue.Count>0)
            {
                DownLoadEventHandler downLoadEventHandler= mDownLoadEventQueue.Dequeue();
                downLoadEventHandler.downLoadEvent?.Invoke(downLoadEventHandler.hotfileInfo);
            }
        }
        public void RemoveDownLoadThread(DownLoadThread downLoadThread)
        {
            lock (mAllDownLoadThreadList)
            {
                if (mAllDownLoadThreadList.Contains(downLoadThread))
                {
                    mAllDownLoadThreadList.Remove(downLoadThread);
                } 
            }
        }
    }
}
