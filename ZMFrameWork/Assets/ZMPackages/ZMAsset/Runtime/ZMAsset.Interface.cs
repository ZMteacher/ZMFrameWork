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
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZM.ZMAsset
{
    public partial class ZMAsset
    {
        #region 框架初始化
        public static void InitFrameWork()
        {
           Instance.Initialize();
        }
        #endregion

        #region 对象实例化 API
        /// <summary>
        /// 同步克隆物体
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="localPoition"></param>
        /// <param name="localScale"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static GameObject InstantiateObject(string path, Transform parent)
        {
            return Instance.mResource.Instantiate(path, parent, Vector3.zero, Vector3.one, Quaternion.identity);
        }
        /// <summary>
        /// 同步克隆物体
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="localPoition"></param>
        /// <param name="localScale"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static GameObject InstantiateObject(string path, Transform parent, Vector3 localPoition, Vector3 localScale, Quaternion quaternion)
        {
            return Instance.mResource.Instantiate(path,parent,localPoition,localScale,quaternion);
        }
      
       
        /// <summary>
        /// 异步克隆对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loadAsync">异步加载回调</param>
        public static void InstantiateObjectAsync(string path, Transform parent,System.Action<GameObject, object, object> loadAsync, object param1 = null, object param2 = null)
        {
            Instance.mResource.InstantiateAsync(path,parent,loadAsync,param1,param2);
        }
        /// <summary>
        /// 可等待异步实例化对象
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="param1">透传参数1 (回调触发时返回)</param>
        /// <param name="param2">透传参数2 (回调触发时返回)</param>
        /// <param name="param3">透传参数3 (回调触发时返回)</param>
        public static async UniTask<AssetsRequest> InstantiateObjectAsync(string path,Transform parent, object param1 = null, object param2 = null, object param3 = null)
        {
          return await Instance.mResource.InstantiateAsync(path,parent, param1, param2, param3);
        }
     
        /// <summary>
        /// 克隆并且等待资源下载完成克隆
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadAsync"></param>
        /// <param name="loading"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static long InstantiateObjectAndLoad(string path, Transform parent, System.Action<GameObject, object, object> loadAsync, System.Action loading, object param1 = null, object param2 = null)
        {
            return Instance.mResource.InstantiateAndLoad(path, parent ,loadAsync, loading, param1, param2);
        }

        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        public static void PreLoadObjct(string path, int count = 1)
        {
             Instance.mResource.PreLoadObj(path,count);
        }
        public static async UniTask PreLoadObjectAsync<T>(string path, int count = 1) 
        {
            await Instance.mResource.PreLoadObjAsync(path, count);
        }
        #endregion

        #region 资源加载 API
        /// <summary> 
        /// 预加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public static void PreLoadResource<T>(string path) where T : UnityEngine.Object
        {
            Instance.mResource.PreLoadResource<T>(path);
        }

        public static async UniTask<T> PreLoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
          return await Instance.mResource.LoadResourceAsync<T>(path);
        }


        /// <summary>
        /// 移除对象加载回调
        /// </summary>
        /// <param name="loadid"></param>
        public static void RemoveObjectLoadCallBack(long loadid)
        {
            Instance.mResource.RemoveObjectLoadCallBack(loadid);
        }
        /// <summary>
        /// 释放对象占用内存
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroy"></param>
        public static void Release(GameObject obj, bool destroy = false)
        {
            Instance.mResource.Release(obj,destroy);
        }
        /// <summary>
        /// 释放图片所占用的内存(存在危险性，确定该图片资源不用时进行调用)
        /// </summary>
        /// <param name="texture"></param>
        public static void Release(Texture texture)
        {
            Instance.mResource.Release(texture);
        }
        public static void Release(AssetsRequest request)
        {
            Instance.mResource.Release(request);
        }
        /// <summary>
        /// 加载图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sprite LoadSprite(string path)
        {
            return Instance.mResource.LoadSprite(path);
        }
        /// <summary>
        /// 加载Texture图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture LoadTexture(string path)
        {
            if (!path.EndsWith(".jpg"))
            {
                path += ".jpg";
            }
            return Instance.mResource.LoadTexture(path);
        }
        /// <summary>
        /// 加载音频文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip LoadAudio(string path)
        {
            return Instance.mResource.LoadAudio(path);
        }
        /// <summary>
        /// 加载Text资源
        /// </summary>
        /// <param name="path">绝对路径</param>
        /// <returns></returns>
        public static TextAsset LoadTextAsset(string fullPath )
        {
            return Instance.mResource.LoadTextAsset(fullPath);
        }
        /// <summary>
        /// 可能等待的异步加载Text资源
        /// </summary>
        /// <param name="path">绝对路径</param>
        /// <returns></returns>
        public static async UniTask<TextAsset> LoadTextAssetAsync(string fullPath)
        {
            return await Instance.mResource.LoadResourceAsync<TextAsset>(fullPath);
        }
        /// <summary>
        /// 异步加载场景 Editor模式下需要把场景添加到File-BuildSetting Scene列表 AssetBundle则不需要
        /// </summary>
        /// <param name="fullPath">场景文件路径</param>
        /// <returns></returns>
        public static  AsyncOperation  LoadSceneAsync(string fullPath, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            return   Instance.mResource.LoadSceceAsync(fullPath, loadSceneMode);
        }
      
        /// <summary>
        /// 加载可编写脚本对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">绝对路径</param>
        /// <returns></returns>
        public static T LoadScriptableObject<T>(string fullPath) where T : UnityEngine.Object
        {
            return Instance.mResource.LoadScriptableObject<T>(fullPath);
        }
        /// <summary>
        /// 从图集中加载指定的图片
        /// </summary>
        /// <param name="atlasPath"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite LoadAtlasSprite(string atlasPath, string spriteName)
        {
           return Instance.mResource.LoadAtlasSprite(atlasPath, spriteName);
        }
        /// <summary>
        /// 从tpsheet(TexturePacker)图集中加载指定的图片
        /// </summary>
        /// <param name="atlasPath"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite LoadPNGAtlasSprite(string atlasPath, string spriteName)
        {
            return Instance.mResource.LoadPNGAtlasSprite(atlasPath, spriteName);
        }

        /// <summary>
        /// 异步加载图片
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loadAsync">异步加载回调</param>
        /// <param name="param1">参数1</param>
        /// <returns></returns>
        public static long LoadTextureAsync(string path, Action<Texture, object> loadAsync, object param1 = null)
        {
            return Instance.mResource.LoadTextureAsync(path,loadAsync,param1);
        }
        /// <summary>
        /// 可通过await进行等待的异步加载Sprite
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<Texture> LoadTextureAsync(string path)
        {
            if (!path.EndsWith(".jpg"))
            {
                path += ".jpg";
            }
            return await Instance.mResource.LoadResourceAsync<Texture>(path);
        }
        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="image">Inage组件</param>
        /// <param name="setNativeSize">是否设置未美术图的原始尺寸</param>
        /// <param name="loadAsync">加载完成的回调</param>
        /// <returns></returns>
        public static long LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> loadAsync = null)
        {
            return Instance.mResource.LoadSpriteAsync(path, image, setNativeSize,loadAsync);
        }
        /// <summary>
        /// 可通过await进行等待的异步加载Sprite
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<Sprite> LoadSpriteAsync(string path)
        {
            if (!path.EndsWith(".png"))
            {
                path += ".png";
            }
            return await Instance.mResource.LoadResourceAsync<Sprite>(path);
        }
        /// <summary>
        /// 清理所有异步加载任务
        /// </summary>
        public static void ClearAllAsyncLoadTask()
        {
            Instance.mResource.ClearAllAsyncLoadTask();
        }
        /// <summary>
        /// 清理加载的资源，释放内存
        /// </summary>
        /// <param name="absoluteCleaning">深度清理：true：销毁所有由AssetBUnle加载和生成的对象(物体和资源)，彻底释放内存占用
        /// 深度清理 false：销毁对象池中的对象，但不销毁由AssetBundle克隆出并在使用的物体和资源对象，具体的内存释放根据内存引用计数选择性释放</param>
        public static void ClearResourcesAssets(bool absoluteCleaning)
        {
            Instance.mResource.ClearResourcesAssets(absoluteCleaning);
        }
        #endregion

        #region 资源热更API
        /// <summary>
        /// 初始化资产模块，在资源热更完成后必须、优先调用
        /// </summary>
        /// <param name="bundleModule">初始化的资产模块</param>
        public static async UniTask<bool> InitAssetsModule(BundleModuleEnum bundleModule)
        {
            return await Instance.mResource.InitAssetModule(bundleModule,false);
        }
        /// <summary>
        /// 开始热更
        /// </summary>
        /// <param name="bundleModule">热更模块</param>
        /// <param name="startHotCallBack">开始热更回调</param>
        /// <param name="hotFinish">热更完成回调</param>
        /// <param name="waiteDownLoad">等待下载的回调</param>
        /// <param name="isCheckAssetsVersion">是否需要检测资源版本</param>
        public  static  void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack, Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waiteDownLoad, bool isCheckAssetsVersion = true) 
        {
            Instance.mHotAssets.HotAssets(bundleModule, startHotCallBack, hotFinish, waiteDownLoad, isCheckAssetsVersion);
        }
        /// <summary>
        /// 检测资源版本是否需要热更，获取需要热更资源的大小
        /// </summary>
        /// <param name="bundleModule">热更模块类型</param>
        /// <param name="callBack">检测完成回调</param>
        public static void CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack) 
        {
            Instance.mHotAssets.CheckAssetsVersion(bundleModule, callBack);
        }
       
        /// <summary>
        /// 获取热更模块
        /// </summary>
        /// <param name="bundleModule">热更模块类型</param>
        /// <returns></returns>
        /// <summary>
        public static HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule) 
        {
           return Instance.mHotAssets.GetHotAssetsModule(bundleModule);
        }

        #endregion

        #region 资源解压 API
        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <returns></returns>
        public static IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModule, Action callBack) 
        {
            return Instance.mDecompressAssets.StartDeCompressBuiltinFile(bundleModule, callBack);
        }
        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        public static float GetDecompressProgress()
        {
            return Instance.mDecompressAssets.GetDecompressProgress();
        }
        #endregion
    }

}