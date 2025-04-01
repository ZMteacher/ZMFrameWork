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
using ZM.ZMAsset;

public interface IResourceInterface  
{
    void Initlizate();

    UniTask<bool> InitAssetModule(BundleModuleEnum bundleModule,bool isAddressableAsset = false);

    void PreLoadObj(string path,int count=1);

    UniTask PreLoadObjAsync(string path, int count = 1);

    void PreLoadResource<T>(string path) where T : UnityEngine.Object;

    GameObject Instantiate(string path, Transform parent, Vector3 localPoition, Vector3 localScale, Quaternion quaternion);

    void InstantiateAsync(string path, Action<GameObject, object, object> loadAsync, object param1, object param2);

    UniTask<AssetsRequest> InstantiateAsync(string path,  object param1, object param21, object param2);

    //Task<AssetsRequest> InstantiateAsyncFormPoolAas(string path, object param1, object param2, object param3,BundleModuleEnum moduleEnum);

    long InstantiateAndLoad(string path, Action<GameObject, object, object> loadAsync, Action loading, object param1, object param2);
    UniTask<T> LoadResourceAsync<T>(string path) where T : UnityEngine.Object;

    //Task<T> LoadResourceAsyncAas<T>(string path, BundleModuleEnum moduleEnum ) where T : UnityEngine.Object;
    void RemoveObjectLoadCallBack(long loadid);

    void Release(GameObject obj, bool destroyCache = false); 

    void Release(Texture texture);

    void Release(AssetsRequest request);

    Sprite LoadSprite(string path);

    Texture LoadTexture(string path);

    AudioClip LoadAudio(string path);

    TextAsset LoadTextAsset(string path);
    AsyncOperation  LoadSceceAsync(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Additive);
    T LoadScriptableObject<T>(string path) where T : UnityEngine.Object;
 
    UnityEngine.Sprite LoadAtlasSprite(string atlasPath, string spriteName);

    UnityEngine.Sprite LoadPNGAtlasSprite(string atlasPath, string spriteName);

    long LoadTextureAsync(string path, Action<Texture, object> loadAsync, object param1 = null);

    long LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> loadAsync = null);

    void ClearAllAsyncLoadTask();

    void ClearResourcesAssets(bool absoluteCleaning);//是否深度清理


}
