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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZM.AssetFrameWork
{
    public partial class ZMAssetsFrame
    {
        /// <summary>
        /// 同步克隆物体
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="localPoition"></param>
        /// <param name="localScale"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static GameObject Instantiate(string path, Transform parent)
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
        public static GameObject Instantiate(string path, Transform parent, Vector3 localPoition, Vector3 localScale, Quaternion quaternion)
        {
            return Instance.mResource.Instantiate(path,parent,localPoition,localScale,quaternion);
        }
      
       
        /// <summary>
        /// 异步克隆对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loadAsync">异步加载回调</param>
        /// <param name="param1">异步加载参数1</param>
        /// <param name="param2">异步加载参数2</param>
        public static void InstantiateAsync(string path, System.Action<GameObject, object, object> loadAsync, object param1 = null, object param2 = null)
        {
            Instance.mResource.InstantiateAsync(path,loadAsync,param1,param2);
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
        public static long InstantiateAndLoad(string path, System.Action<GameObject, object, object> loadAsync, System.Action loading, object param1 = null, object param2 = null)
        {
            return Instance.mResource.InstantiateAndLoad(path, loadAsync, loading, param1, param2);
        }

        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        public static void PreLoadObj(string path, int count = 1)
        {
             Instance.mResource.PreLoadObj(path,count);
        }
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public static void PreLoadResource<T>(string path) where T : UnityEngine.Object
        {
            Instance.mResource.PreLoadResource<T>(path);
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
        /// 释放图片所占用的内存
        /// </summary>
        /// <param name="texture"></param>
        public static void Release(Texture texture)
        {
            Instance.mResource.Release(texture);
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
        /// 清理所有异步加载任务
        /// </summary>
        public static void ClearAllAsyncLoadTask()
        {
            Instance.mResource.ClearAllAsyncLoadTask();
        }
        /// <summary>
        /// 清理加载的资源，释放内存
        /// </summary>
        /// <param name="absoluteCleaning">深度清理：true：销毁所有由AssetBUnle加载和生成的对象，彻底释放内存占用
        /// 深度清理 false：销毁对象池中的对象，但不销毁由AssetBundle克隆出并在使用的对象，具体的内存释放根据内存引用计数选择性释放</param>
        public static void ClearResourcesAssets(bool absoluteCleaning)
        {
            Instance.mResource.ClearResourcesAssets(absoluteCleaning);
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
    }

}