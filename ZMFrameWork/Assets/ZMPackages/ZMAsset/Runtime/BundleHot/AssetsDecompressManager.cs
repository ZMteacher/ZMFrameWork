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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
namespace ZM.ZMAsset
{
    public class AssetsDecompressManager : IDecompressAssets
    {
        /// <summary>
        /// 资源解压路径
        /// </summary>
        private string mDecompressPath;
        /// <summary>
        /// 资源内嵌路径
        /// </summary>
        private string mStreamingAssetsBundlePath;
        /// <summary>
        /// 需要解压的资源列表
        /// </summary>
        private List<string> mNeedDecompressFileList = new List<string>();

        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <param name="bundleModule">解压资源模块</param>
        /// <param name="callBack">解压完成回调</param>
        /// <returns></returns>
        public override IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModule, Action callBack)
        {
            if (ComputeDecompressFile(bundleModule))
            {
                IsStartDecompress = true;
                ZMAsset.Instance.StartCoroutine(UnPackToPersistentDataPath(bundleModule, callBack));
            }
            else
            {
                Debug.Log("不需要解压文件");
                callBack?.Invoke();
            }
            return this;
        }
        /// <summary>
        /// 计算需要解压的文件
        /// </summary>
        /// <param name="bundleModule"></param>
        /// <returns></returns>
        private bool ComputeDecompressFile(BundleModuleEnum bundleModule)
        {
            mStreamingAssetsBundlePath = BundleSettings.Instance.GetAssetsBuiltinBundlePath(bundleModule);
            mDecompressPath = BundleSettings.Instance.GetAssetsDecompressPath(bundleModule);
            mNeedDecompressFileList.Clear();
          
#if UNITY_ANDROID||UNITY_ISO||UNITY_EDITOR
            //如果文件夹不存在，就进行创建
            if (!Directory.Exists(mDecompressPath))
            {
                Directory.CreateDirectory(mDecompressPath);
            }

            //计算需要解压的文件，以及大小
            TextAsset textAsset = Resources.Load<TextAsset>(bundleModule + "info");
            if (textAsset != null)
            {
                List<BuiltinBundleInfo> builtinBundleInfoList = JsonConvert.DeserializeObject<List<BuiltinBundleInfo>>(textAsset.text);
                foreach (var info in builtinBundleInfoList)
                {
                    //本地文件储存路径
                    string localFilePath = mDecompressPath + info.fileName;
                    if (localFilePath.EndsWith(".meta"))
                    {
                        continue;
                    }
                    //计算出需要解压的文件
                    if (!File.Exists(localFilePath) || MD5.GetMd5FromFile(localFilePath) != info.md5)
                    {
                        mNeedDecompressFileList.Add(info.fileName);
                        TotalSizem += info.size / 1024.0f;
                    }
                }
            }
            else
            {
                Debug.LogError(bundleModule + "info" + " 不存在，请检查内嵌资源 是否内嵌！");
            }
            return mNeedDecompressFileList.Count > 0;
#else
            return false;
#endif
        }
        public override float GetDecompressProgress()
        {
            return AlreadyDecompressSizem == 0 ? 0 : AlreadyDecompressSizem / TotalSizem;
        }
        /// <summary>
        /// 解压文件到持久化目录
        /// </summary>
        /// <param name="bundleModule"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator UnPackToPersistentDataPath(BundleModuleEnum bundleModule, Action callBack)
        {
            foreach (var fileName in mNeedDecompressFileList)
            {
                string filePath = "";
#if UNITY_EDITOR_OSX || UNITY_IOS
                filePath = "file://" + mStreamingAssetsBundlePath + fileName;
#else
                filePath = mStreamingAssetsBundlePath + fileName;
#endif
                //文件不存在
                if (!File.Exists(filePath)) continue;
                
                Debug.Log("Start UnPack AssetBundle filePath:" + filePath + "\r\n UnPackPath:" + mDecompressPath);
               
                //通过 UnityWebRequest(Http) 访问本地文件 ，这个过程是不消耗流量的，相当于直接读取，所以速度是非常快的
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
                unityWebRequest.timeout = 30;
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result== UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("UnPack Error:" + unityWebRequest.error);
                }
                else
                {
                    //到了这一步，文件就已经读取完成了
                    byte[] bytes = unityWebRequest.downloadHandler.data;
                    //文件不存在或被删除掉了
                    if (bytes != null)
                    {
                        FileHelper.WriteFile(mDecompressPath + fileName, bytes);
                        AlreadyDecompressSizem += (bytes.Length / 1024f) / 1024f;
                        Debug.Log("AlreadyDecompressSizem:" + AlreadyDecompressSizem + " totalSize:" + TotalSizem);
                        Debug.Log("UnPack Finish " + mDecompressPath + fileName);
                    }
                }

                unityWebRequest.Dispose();
            }

            callBack?.Invoke();
            IsStartDecompress = false;
        }
    }
}