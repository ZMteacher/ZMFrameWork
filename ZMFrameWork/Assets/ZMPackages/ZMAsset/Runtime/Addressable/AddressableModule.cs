using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;

namespace ZM.ZMAsset
{
    public class AddressableModule
    {
        /// <summary>
        /// 热更资源下载储存路径
        /// </summary>
        public string HotAssetsSavePath { get { return Application.persistentDataPath + "/HotAssets/" + CurBundleModuleEnum + "/"; } }
        /// <summary>
        /// 热更资源下载地址
        /// </summary>
        public string HotAssetDownLoadUrl { get => mServerHotAssetsManifest.downLoadURL; }
        /// <summary>
        /// 所有热更的资源列表
        /// </summary>
        public List<HotFileInfo> mAllHotAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 需要下载的资源列表
        /// </summary>
        public List<HotFileInfo> mNeedDownLoadAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 服务端资源清单
        /// </summary>
        private HotAssetsManifest mServerHotAssetsManifest;
        /// <summary>
        /// 本地资源清单
        /// </summary>
        private HotAssetsManifest mLocalHotAssetsManifest;
        /// <summary>
        /// 服务端资源热更清单储存路径
        /// </summary>
        private string mServerHotAssetsManifestPath;
        /// <summary>
        /// 本地资源热更清单文件储存路径
        /// </summary>
        private string mLocalHotAssetManifestPath;
        /// <summary>
        /// 当前下载的资源模块类型
        /// </summary>
        public BundleModuleEnum CurBundleModuleEnum { get; set; }
        /// <summary>
        /// 所有热更资源的一个长度
        /// </summary>
        public int HotAssetCount { get { return mAllHotAssetsList.Count; } }
        /// <summary>
        /// 是否热更资源
        /// </summary>
        public bool IsHotAsset { get; set; } = true;
        /// <summary>
        /// 异步任务等待队列
        /// </summary>
        public TaskCompletionSource<bool> TaskCompletionSouce { get;set; }

        public AddressableModule(BundleModuleEnum bundleModule )
        {
            CurBundleModuleEnum = bundleModule;
            GeneratorHotAssetsManifest();
        }

        /// <summary>
        /// 检测资源版本
        /// </summary>
        /// <param name="checkCallBack"></param>
        public async UniTask<bool> CheckAssetsVersion()
        {
            mNeedDownLoadAssetsList.Clear();
            //1.验证并下载资源清单
            if (mServerHotAssetsManifest == null)
            {
                await DownLoadHotAssetsManifest();
            }
            //2.检测当前版本是否需要热更
            if (mServerHotAssetsManifest!=null)
            {
                return await CheckModuleAssetsIsHot() ? ComputeNeedHotAssetsList() : false;
            }
            return false;
        }
        /// <summary>
        /// 检测模块资源是否需要热更
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckModuleAssetsIsHot()
        {
            //如果服务端资源清单不存，不需要热更
            if (mServerHotAssetsManifest == null)
            {
                return false;
            }
            //如果本地资源清单文件不存在，说明我们需要热更
            if (!File.Exists(mLocalHotAssetManifestPath))
            {
                return true;
            }
            //判断本地资源清单补丁版本号是否与服务端资源清单补丁版本号一致，如果一致，不需要热更， 如果不一致，则需要热更
            HotAssetsManifest localHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(await File.ReadAllTextAsync(mLocalHotAssetManifestPath));
            if (localHotAssetsManifest.hotAssetsPatchList.Count == 0 && mServerHotAssetsManifest.hotAssetsPatchList.Count != 0)
            {
                return true;
            }

            //获取本地热更补丁的最后一个补丁
            HotAssetsPatch localHotPatch = localHotAssetsManifest.hotAssetsPatchList[localHotAssetsManifest.hotAssetsPatchList.Count - 1];
            //获取服务端热更补丁的最后一个补丁
            HotAssetsPatch serverHotPatch = mServerHotAssetsManifest.hotAssetsPatchList[mServerHotAssetsManifest.hotAssetsPatchList.Count - 1];

            if (localHotPatch != null && serverHotPatch != null)
            {
                if (localHotPatch.patchVersion != serverHotPatch.patchVersion)
                {
                    return true;
                }
            }

            if (serverHotPatch != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 计算需要热更的文件列表
        /// </summary>
        /// <param name="serverAssetsPath"></param>
        /// <returns></returns>
        private bool ComputeNeedHotAssetsList()
        {
            if (!Directory.Exists(HotAssetsSavePath))
            {
                Directory.CreateDirectory(HotAssetsSavePath);
            }
            // HotAssetsPatch serverAssetsPatch = mServerHotAssetsManifest.hotAssetsPatchList[mServerHotAssetsManifest.hotAssetsPatchList.Count - 1];
            HotAssetsPatch serverAssetsPatch = mServerHotAssetsManifest.hotAssetsPatchList[^1]; //等价于上述代码
            if (File.Exists(mLocalHotAssetManifestPath))
                mLocalHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(File.ReadAllText(mLocalHotAssetManifestPath));
     
            foreach (var item in serverAssetsPatch.hotAssetsList)
            {
                //获取本地AssetBundle文件路径
                string localFilePath = HotAssetsSavePath + item.abName;
                mAllHotAssetsList.Add(item);
                //如果本地文件不存在，或者本地文件与服务端不一致，就需要热更
                
                if (!File.Exists(localFilePath) || item.md5 != MD5.GetMd5FromFile(localFilePath))
                {
                    mNeedDownLoadAssetsList.Add(item);
                }
            }

            return mNeedDownLoadAssetsList.Count > 0;
        }
        
 
        /// <summary>
        /// 下载资源热更清单
        /// </summary>
        /// <returns></returns>
        private IEnumerator DownLoadHotAssetsManifest()
        {
            string url = BundleSettings.Instance.AssetBundleDownLoadUrl + "/HotAssets/" + CurBundleModuleEnum + "AssetsHotManifest.json";
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.timeout = 30;

            Debug.Log("*** Requset HotAssetsMainfest Url:" + url);

            yield return webRequest.SendWebRequest();

            if (webRequest.result== UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("DownLoad Error:" + webRequest.error);
                yield break;
            }
            string donwloadContent = webRequest.downloadHandler.text;
            try
            {
                Debug.Log($"*** Request AssetBundle HotAssetsMainfest Url Finish Module:{CurBundleModuleEnum} txt:{donwloadContent}");
                //写入服务端资源热更清单到本地
                FileHelper.WriteFileAsync(mServerHotAssetsManifestPath, donwloadContent);
            }
            catch (Exception e)
            {
                Debug.LogError("服务端资源清单配置下载异常，文件不存在或者配置有问题，更新出错，请检查：" + e.ToString());
            }
            yield return UniTask.RunOnThreadPool(() =>
            {
                mServerHotAssetsManifest = JsonConvert.DeserializeObject<HotAssetsManifest>(donwloadContent);
            });
            webRequest.Dispose();
         }
        public void GeneratorHotAssetsManifest()
        {
            mServerHotAssetsManifestPath = Application.persistentDataPath + "/Server" + CurBundleModuleEnum + "AssetsHotManifest.json";
            mLocalHotAssetManifestPath = Application.persistentDataPath + "/Local" + CurBundleModuleEnum + "AssetsHotManifest.json";
        }
 
        public HotFileInfo AssetIsNeedHotUpdate(string bundleName)
        {
            foreach (var item in mNeedDownLoadAssetsList)
            {
                if (string.Equals(item.abName, bundleName))
                {
                    return item;
                }
            }
            return null;
        }
        public void RemoveNeedHotAsset(HotFileInfo hotInfo)
        {
            mNeedDownLoadAssetsList.Remove(hotInfo);
        }
        /// <summary>
        /// 判断热更文件是否存在
        /// </summary>
        public bool HotAssetsIsExists(string bundleName)
        {
            foreach (var item in mAllHotAssetsList)
            {
                if (string.Equals(bundleName, item.abName))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
