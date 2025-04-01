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
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZM.ZMAsset
{
    /// <summary>
    /// 资源下载线程
    /// </summary>
    public class DownLoadThread
    {
        /// <summary>
        /// 下载完成回调
        /// </summary>
        private Action<DownLoadThread, HotFileInfo> OnDownLoadSuccess;

        /// <summary>
        /// 下载失败回调
        /// </summary>
        public Action<DownLoadThread, HotFileInfo> OnDownLoadFailed;
        /// <summary>
        /// 当前热更的资源模块
        /// </summary>
        private BundleModuleEnum mCurBundleModuleEnum;

        /// <summary>
        /// 当前热更的资源模块
        /// </summary>
        private HotAssetsModule mCurHotAssetsModule;

        /// <summary>
        /// 当前热更的文件信息
        /// </summary>
        private HotFileInfo mHotFileInfo;

        /// <summary>
        /// 文件下载的地址
        /// </summary>
        private string mDownLoadUrl;

        /// <summary>
        /// 下载下的文件储存的地址
        /// </summary>
        private string mFileSavePath;

        /// <summary>
        /// 下载的大小
        /// </summary>
        private float mDownLoadSizeKB;

        /// <summary>
        /// 当前下载的次数
        /// </summary>
        private int curDownLoadCount;

        /// <summary>
        /// 最大尝试下载次数
        /// </summary>
        private const int MAX_TRY_DOWNLOAD_COUNT = 3;

        /// <summary>
        /// 资源下载线程
        /// </summary>
        /// <param name="assetsModule">资源所属模块</param>
        /// <param name="hotFileInfo">需要下载热更的资源</param>
        /// <param name="downLoadUrl">资源下载地址</param>
        /// <param name="fileSavePath">文件储存地址</param>
        public DownLoadThread(HotAssetsModule assetsModule, HotFileInfo hotFileInfo, string downLoadUrl,
            string fileSavePath)
        {
            this.mCurHotAssetsModule = assetsModule;
            this.mCurBundleModuleEnum = assetsModule.CurBundleModuleEnum;
            this.mHotFileInfo = hotFileInfo;
            this.mFileSavePath = fileSavePath + "/" + hotFileInfo.abName;
            this.mDownLoadUrl = downLoadUrl + "/" + hotFileInfo.abName;
        }

        public DownLoadThread(BundleModuleEnum bundleModule, HotFileInfo hotFileInfo, string downLoadUrl,
            string fileSavePath)
        {
            this.mHotFileInfo = hotFileInfo;
            this.mCurBundleModuleEnum = bundleModule;
            this.mFileSavePath = fileSavePath + "/" + hotFileInfo.abName;
            this.mDownLoadUrl = downLoadUrl + "/" + hotFileInfo.abName;
        }

        /// <summary>
        /// 开始通过子线程下载资源
        /// </summary>
        /// <param name="downLoadSuccess">下载成功回调</param>
        /// <param name="downLoadFailed">下载失败回调</param>
        public void StartDownLoad(Action<DownLoadThread, HotFileInfo> downLoadSuccess, Action<DownLoadThread, HotFileInfo> downLoadFailed)
        {
    
            curDownLoadCount++;
            OnDownLoadSuccess = downLoadSuccess;
            OnDownLoadFailed = downLoadFailed;
            Debug.Log("StartDownLoad ModuelEnum:" + mCurHotAssetsModule.CurBundleModuleEnum + " AssetBundle URL:" + mDownLoadUrl);
            Task.Run(() =>
            {
                //这里的代码在子线程中执行
                try
                {
                    HttpWebRequest request = WebRequest.Create(mDownLoadUrl) as HttpWebRequest;
                    request.Method = "GET";
                    //发起请求
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            
                    //创建本地文件流
                    FileStream fileStream = File.Create(mFileSavePath);
            
                    using (var stream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[512]; //512 
                        //从字节流中读取字节，读取到buff数组中
                        int size = stream.Read(buffer, 0, buffer.Length); //700
                        
                        while (size > 0)
                        {
                            fileStream.Write(buffer, 0, size);
                            size = stream.Read(buffer, 0, buffer.Length);
                            //1mb=1024kb 1kb=1024字节
                            mDownLoadSizeKB += size;
                            //计算以m为单位的大小
                            mCurHotAssetsModule.AssetsDownLoadSizeM += ((size / 1024.0f) / 1024.0f);
                        }
                        fileStream.Dispose();
                        fileStream.Close();
                        //文件下载异常 或 下载完成的文件因网络问题或其他问题发生损坏 || MD5.GetMd5FromFile(mFileSavePath) != mHotFileInfo.md5
                        if (mDownLoadSizeKB == 0 || MD5.GetMd5FromFile(mFileSavePath) != mHotFileInfo.md5)
                        {
                            Debug.LogError("File DownLoad exception plase check file fileName:" + mHotFileInfo.abName + " fileUrl:" + mDownLoadUrl);
                            if (curDownLoadCount > MAX_TRY_DOWNLOAD_COUNT)
                            {
                                OnDownLoadFailed?.Invoke(this, mHotFileInfo);
                            }
                            else
                            {
                                Debug.LogError("文件下载失败，正在进行重新下载，下载次数" + curDownLoadCount);
                                StartDownLoad(OnDownLoadSuccess, OnDownLoadFailed);
                            }
                        }
                        else
                        {
                            Debug.Log("OnDownLoadSuccess ModuleEnum:" + mCurHotAssetsModule.CurBundleModuleEnum + " AssetBundleUrl:" + mDownLoadUrl + " FileSavePath:" + mFileSavePath);
                            OnDownLoadSuccess?.Invoke(this, mHotFileInfo);
                        }
                    }
                    
                }
                catch (Exception e)
                {
                    Debug.LogError("DownLoad AssetBundle Error Url:" + mDownLoadUrl + " Exception:" + e);
                    if (curDownLoadCount > MAX_TRY_DOWNLOAD_COUNT)
                    {
                        OnDownLoadFailed?.Invoke(this, mHotFileInfo);
                    }
                    else
                    {
                        Debug.LogError("文件下载失败，正在进行重新下载，下载次数" + curDownLoadCount);
                        StartDownLoad(OnDownLoadSuccess, OnDownLoadFailed);
                    }
                }
            });
        }

        public async UniTask<bool> StartDownLoadAsync()
        {
            try
            {
                curDownLoadCount++;
                //文件是否完整
                bool fileIsComplete = false;
                await Task.Run(async () =>
                {
                    Debug.Log("StartDownLoad ModuelEnum:" + mCurBundleModuleEnum + " AssetBundle URL:" + mDownLoadUrl);
                    
                    HttpWebRequest request = WebRequest.Create(mDownLoadUrl) as HttpWebRequest;
                    request.Method = "GET";
                    //发起请求
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    //创建本地文件流
                    using (FileStream fileStream = File.Create(mFileSavePath))
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[512]; //512 
                            //从字节流中读取字节，读取到buff数组中
                            int size = stream.Read(buffer, 0, buffer.Length); //700
                            while (size > 0)
                            {
                                fileStream.Write(buffer, 0, size);
                                size = stream.Read(buffer, 0, buffer.Length);
                                //1mb=1024kb 1kb=1024字节
                                mDownLoadSizeKB += size;
                            }
                        }
                        fileStream.Dispose();
                        fileStream.Close();
                    }
                   
                    //验证下载下来的文件是否完整，可能会被运营商或第三方拦截篡改
                    fileIsComplete = MD5.GetMd5FromFile(mFileSavePath) == mHotFileInfo.md5;
                });
                //文件下载异常 或 下载完成的文件因网络问题或其他问题发生损坏
                if (mDownLoadSizeKB == 0 || !fileIsComplete)
                {
                    Debug.LogError("File DownLoad exception plase check file fileName:" + mHotFileInfo.abName + " fileUrl:" + mDownLoadUrl);
                    if (curDownLoadCount >= MAX_TRY_DOWNLOAD_COUNT)
                    {
                        Debug.LogError("文件下载失败，正在进行重新下载，下载次数" + curDownLoadCount);
                        return await StartDownLoadAsync();
                                 
                    }
                    return false;
                }
                 
                Debug.Log("OnDownLoadSuccess ModuleEnum:" + mCurBundleModuleEnum + " AssetBundleUrl:" + mDownLoadUrl + " FileSavePath:" + mFileSavePath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("DownLoad AssetBundle Error Url:" + mDownLoadUrl + " Exception:" + e);
                if (curDownLoadCount > MAX_TRY_DOWNLOAD_COUNT)
                {
                    return false;
                }
                else
                {
                    Debug.LogError("文件下载失败，正在进行重新下载，下载次数" + curDownLoadCount);
                    return await StartDownLoadAsync();
                }
            }
        }
    }
}