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
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace ZM.ZMAsset
{
    /// <summary>
    /// 缓存对象
    /// </summary>
    public class CacheObejct
    {
        public uint crc;
        public string path;
        public int insid;
        public GameObject obj;
        public OriginData originData;
        public void Release()
        {
            crc = 0;
            insid = 0;
            path = "";
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
            originData=null;
            obj = null;
        }
    }
    /// <summary>
    /// 加载对象回调
    /// </summary>
    public class LoadObjectCallBack
    {
        public string path;
        public uint crc;
        public object param1;
        public object param2;
        public System.Action<GameObject, object, object> loadResult;
    }
    /// <summary>
    /// 资源请求
    /// </summary>
    public class AssetsRequest
    {
        public GameObject obj;
        public object param1;
        public object param2;
        public object param3;

        public void Release()
        {
            if (obj != null)
                ZMAsset.Release(obj);
            ZMAsset.Release(this);
            param1 = null;
            param2 = null;
            param3 = null;
            obj = null;
          
        }
    }

    public class ResourceManager : IResourceInterface, IAddressableAssetInterface
    {
        /// <summary>
        /// 已经加载过的资源字典 key为资源路径Crc vluae 为资源对象
        /// </summary>
        private Dictionary<uint, BundleItem> mAlreayLoadAssetsDic = new Dictionary<uint, BundleItem>();
        /// <summary>
        /// 对象池字典
        /// </summary>
        private Dictionary<uint, List<CacheObejct>> mObjectPoolDic = new Dictionary<uint, List<CacheObejct>>();
        /// <summary>
        /// 所有对象字典
        /// </summary>
        private Dictionary<int, CacheObejct> mAllObjectDic = new Dictionary<int, CacheObejct>();
        /// <summary>
        /// 缓存对象类对象池
        /// </summary>
        private ClassObjectPool<CacheObejct> mCacheObejctPool = new ClassObjectPool<CacheObejct>(150);
        /// <summary>
        /// 缓存资源请求对象池
        /// </summary>
        private ClassObjectPool<AssetsRequest> mAssetsRequestPool = new ClassObjectPool<AssetsRequest>(50);
        /// <summary>
        /// 异步加载任务列表
        /// </summary>
        private List<long> mAsyncLoadingTaskList = new List<long>();
        /// <summary>
        /// 异步加载任务唯一id
        /// </summary>
        private long asyncGuid;
        /// <summary>
        ///  异步加载任务唯一id
        /// </summary>
        private long mAsyncTaskGuid { get { if (asyncGuid > long.MaxValue) asyncGuid = 0; return asyncGuid++; } }

        /// <summary>
        /// 加载对象回调
        /// </summary>
        private Dictionary<long, LoadObjectCallBack> mLoadObjectCallBackDic = new Dictionary<long, LoadObjectCallBack>();

        /// <summary>
        /// 等待加载的资源列表
        /// </summary>
        private List<HotFileInfo> mWaitLoadAssetsList = new List<HotFileInfo>();
        /// <summary>
        /// 所有图集图片的集合
        /// </summary>
        protected readonly Dictionary<string, UnityEngine.Object[]> mAllAssetObjectDic = new Dictionary<string, UnityEngine.Object[]>();
        public void Initlizate()
        {
            HotAssetsManager.DownLoadBundleFinish += AssetsDownLoadFinish;
        }

        #region 对象加载
        /// <summary>
        /// AssetBundle资源下载完成回调
        /// </summary>
        /// <param name="info"></param>
        private void AssetsDownLoadFinish(HotFileInfo info)
        {
            if (mWaitLoadAssetsList.Count==0) return;
            // Debug.Log("ResourceManager   AssetsDownLoadFinish:" + info.abName);
            //处理比AssetBunle配置文件先下载下来的AssetBunle的加载
            if (info.abName.Contains("bundleconfig"))
            {
                // Debug.Log("Handler waitLoadLsit Count:" + mWaitLoadAssetsList.Count);
                HotFileInfo[] hotFileArray = mWaitLoadAssetsList.ToArray();
                mWaitLoadAssetsList.Clear();
                foreach (var item in hotFileArray)
                {
                    AssetsDownLoadFinish(item);
                }
                return;
            }
            //如果回调字典长度大于0 才需要去处理回调
            if (mLoadObjectCallBackDic.Count > 0)
            {
                //根据对象的路径查找对象所在的AB包，以及这个AB下的所有的资源
                List<BundleItem> assetsItemList = AssetBundleManager.Instance.GetBundleItemByABName(info.abName);
                //如果assetsItemList.Count==0 则说明配置文件未加载，资源下载是多线程下，
                //有可能会出现 AssetBundle下载速度比AssetBundleConfig配置文件快，这种情况我们的AB配置文件就处于未加载的状态
                if (assetsItemList.Count == 0)
                {
                    for (int i = 0; i < mWaitLoadAssetsList.Count; i++)
                    {
                        //去重
                        if (mWaitLoadAssetsList[i].abName == info.abName)
                        {
                            return;
                        }
                    }
                    mWaitLoadAssetsList.Add(info);
                    return;
                }

                List<long> removeList = new List<long>();
                //遍历对象加载回调，触发资源加载
                foreach (var item in mLoadObjectCallBackDic)
                {
                    if (ListContainsAsset(assetsItemList, item.Value.crc))
                    {
                        Debug.Log("ResourceManager AssetsDownLoadFinish Load Obj path:" + item.Value.path);
                        item.Value.loadResult?.Invoke(Instantiate(item.Value.path, null, Vector3.zero, Vector3.one, Quaternion.identity),
                            item.Value.param1, item.Value.param1);
                        removeList.Add(item.Key);
                    }
                }
                //移除字典中的回调
                for (int i = 0; i < removeList.Count; i++)
                {
                    mLoadObjectCallBackDic.Remove(removeList[i]);
                }
            }
        }
        public bool ListContainsAsset(List<BundleItem> assetsItemList, uint crc)
        {
            foreach (var item in assetsItemList)
            {
                if (item.crc == crc)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        public void PreLoadObj(string path, int count = 1)
        {
            List<GameObject> preLoadObjList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                preLoadObjList.Add(Instantiate(path, null, Vector3.zero, Vector3.one, Quaternion.identity));
            }
            //回收对象到对象池
            foreach (var obj in preLoadObjList)
            {
                Release(obj);
            }
        }
        public async UniTask PreLoadObjAsync(string path, int count = 1)
        {
            List<GameObject> preLoadObjList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                AssetsRequest request = await InstantiateAsync(path);
                preLoadObjList.Add(request.obj);
                request.Release();
            }
            //回收对象到对象池
            foreach (var obj in preLoadObjList)
            {
                Release(obj);
            }
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
        public GameObject Instantiate(string path, Transform parent, Vector3 localPoition, Vector3 localScale, Quaternion quaternion)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            //先从对象池中查询这个对象，如果存在就直接使用
            CacheObejct cacheObj = GetCacheObjFromPools(Crc32.GetCrc32(path),false);
            if (cacheObj != null && cacheObj.obj != null)
            {
                GameObject poolObject = cacheObj.obj;
                poolObject.transform.SetParent(parent);
                //重置数据
                SetObjectTransData(poolObject, localPoition, localScale, quaternion);
                return cacheObj.obj;
            }
            //加载该对象
            GameObject obj = LoadResource<GameObject>(path);
            if (obj != null)
            {
                GameObject nObj = InstantiateObject(path, obj, parent);
                SetObjectTransData(nObj, localPoition, localScale, quaternion);
                return nObj;
            }
            else
            {
                Debug.LogError("GameObject load failed,path is null...");
                return null;
            }
        }

        private void SetObjectTransData(GameObject obj,Vector3 localPoition, Vector3 localScale, Quaternion quaternion)
        {
            obj.transform.localPosition = localPoition;
            obj.transform.localScale = localScale;
            obj.transform.rotation = quaternion;
        }

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private GameObject InstantiateObject(string path, GameObject obj, Transform parent)
        {
            obj = GameObject.Instantiate(obj, parent, false);
            CacheObejct cacheObejct = mCacheObejctPool.Spawn();
            cacheObejct.obj = obj;
            cacheObejct.path = path;
            cacheObejct.crc = Crc32.GetCrc32(path);
            if (obj !=null)
            {
                cacheObejct.insid = obj.GetInstanceID();
                cacheObejct.originData = obj.GetComponent<OriginData>();
                //重置原始数据
                if (!ReferenceEquals(cacheObejct.originData, null))
                {
                    cacheObejct.originData.ResetOriginData();
                }
            }
            mAllObjectDic.Add(cacheObejct.insid, cacheObejct);
            return obj;
        }
    
        /// <summary>
        /// 异步克隆对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loadAsync">异步加载回调</param>
        /// <param name="param1">异步加载参数1</param>
        /// <param name="param2">异步加载参数2</param>
        public async void InstantiateAsync(string path, System.Action<GameObject, object, object> loadAsync, object param1 = null, object param2 = null)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            //先从对象池中查询这个对象，如果存在就直接使用
            CacheObejct cacheObj = GetCacheObjFromPools(Crc32.GetCrc32(path),true);
            if (cacheObj != null && cacheObj.obj != null)
            {
                loadAsync?.Invoke(cacheObj.obj, param1, param2);
                return;
            }
            //获取异步加载任务唯一id
            long guid = mAsyncTaskGuid;
            mAsyncLoadingTaskList.Add(guid);
            //开始异步加载资源
            GameObject obj = await LoadResourceAsync<GameObject>(path);
            
            //异步加载完成
            if (obj != null)
            {
                if (mAsyncLoadingTaskList.Contains(guid))
                {
                    mAsyncLoadingTaskList.Remove(guid);
                    GameObject nObj = InstantiateObject(path, obj, null);
                    loadAsync?.Invoke(nObj, param1, param2);
                }
            }
            else
            {
                mAsyncLoadingTaskList.Remove(guid);
                Debug.LogError("Async Load GameObject is Null Path:" + path);
            }
            
        }
        /// <summary>
        /// 异步克隆对象 可通过await进行等待
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loadAsync">异步加载回调</param>
        /// <param name="param1">异步加载参数1</param>
        /// <param name="param2">异步加载参数2</param>
        public async UniTask<AssetsRequest> InstantiateAsync(string path,  object param1 = null, object param2 = null,object param3=null)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            AssetsRequest request = mAssetsRequestPool.Spawn();
            request.param1 = param1;
            request.param2 = param2;
            request.param3 = param3;
            //先从对象池中查询这个对象，如果存在就直接使用
            CacheObejct cacheObj = GetCacheObjFromPools(Crc32.GetCrc32(path),true);
            if (cacheObj != null && cacheObj.obj != null)
            {
                request.obj = cacheObj.obj;
                return request;
            }
            //获取异步加载任务唯一id
            long guid = mAsyncTaskGuid;
            mAsyncLoadingTaskList.Add(guid);
            //开始异步加载资源
            GameObject loadObj= await LoadResourceAsync<GameObject>(path);
            if (loadObj == null)
            {
                Debug.LogError("Load GameObject Failed Path：" + path);
                request.obj = new GameObject("Laod ErrorObj");//创建空物体，增加鲁棒性，防止报空后的游戏逻辑阻塞
                return request;
            }
            if (mAsyncLoadingTaskList.Contains(guid))
            {
                mAsyncLoadingTaskList.Remove(guid);
                GameObject nObj = InstantiateObject(path,loadObj, null);
                request.obj = nObj;
                return request;
            }
            else
            {
                Debug.LogError("Async Task already Cancel Load invalid! Path:" + path);
                request.obj = new GameObject("Laod ErrorObj");//创建空物体，增加鲁棒性，防止报空后的游戏逻辑阻塞
                return request;
            }
            
        }
        /// <summary>
        /// 异步克隆可寻址资源对象,可通过await进行等待
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param1"></param>
        /// <param name="param21"></param>
        /// <param name="param2"></param>
        /// <param name="moduleEnum"></param>
        /// <returns></returns>
        public async UniTask<AssetsRequest> InstantiateAsyncFormPoolAas(string path, BundleModuleEnum moduleEnum, object param1, object param2, object param3)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            AssetsRequest request = mAssetsRequestPool.Spawn();
            request.param1 = param1;
            request.param2 = param2;
            request.param3 = param3;
            //先从对象池中查询这个对象，如果存在就直接使用
            CacheObejct cacheObj = GetCacheObjFromPools(Crc32.GetCrc32(path),true);
            if (cacheObj != null && cacheObj.obj != null)
            {
                request.obj = cacheObj.obj;
                return request;
            }
            //获取异步加载任务唯一id
            long guid = mAsyncTaskGuid;
            mAsyncLoadingTaskList.Add(guid);
            //开始异步加载资源
            GameObject loadObj = await LoadResourceAsyncAas<GameObject>(path, moduleEnum);
            if (mAsyncLoadingTaskList.Contains(guid))
            {
                mAsyncLoadingTaskList.Remove(guid);
                GameObject nObj = InstantiateObject(path, loadObj, null);
                request.obj = nObj;
                return request;
            }
            else
            {
                Debug.LogError("Async Task already Cancel Load invalid! Path:" + path);
                return request;
            }
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
        public long InstantiateAndLoad(string path, System.Action<GameObject, object, object> loadAsync, System.Action loading, object param1 = null, object param2 = null)
        {
            path = path.EndsWith(".prefab") ? path : path + ".prefab";
            //先从对象池中查询这个对象，如果存在就直接使用
            CacheObejct cacheObj = GetCacheObjFromPools(Crc32.GetCrc32(path),true);
            long loadid = -1;
            if (cacheObj != null && cacheObj.obj != null)
            {
                loadAsync?.Invoke(cacheObj.obj, param1, param2);
                return loadid;
            }

            GameObject obj = Instantiate(path, null, Vector3.zero, Vector3.one, Quaternion.identity);

            if (obj != null)
            {
                loadAsync?.Invoke(obj, param1, param2);
            }
            else
            {
                //资源没有下载完成，本地没有这个资源
                loadid = mAsyncTaskGuid;
                loading?.Invoke();
                mLoadObjectCallBackDic.Add(loadid, new LoadObjectCallBack
                {
                    path = path,
                    crc = Crc32.GetCrc32(path),
                    loadResult = loadAsync,
                    param1 = param1,
                    param2 = param2
                });
            }
            return loadid;
        }

        /// <summary>
        /// 从对象池中取出对象
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        private CacheObejct GetCacheObjFromPools(uint crc,bool reSetData)
        {
            mObjectPoolDic.TryGetValue(crc, out var objList);
            if (objList != null && objList.Count > 0)
            {
                //直接取对象池中的第0个对象
                CacheObejct obj = objList[^1];
                objList.Remove(obj);
                //重置数据
                if (!ReferenceEquals(obj.originData,null))
                {
                    obj.originData.ResetOriginData();
                }
                return obj;
            }
            return null;
        }

   



        #endregion

        #region 资源加载
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public void PreLoadResource<T>(string path) where T : UnityEngine.Object
        {
            LoadResource<T>(path);
        }
        public   AsyncOperation LoadSceceAsync(string path,LoadSceneMode loadSceneMode= LoadSceneMode.Additive)
        {
            if (!path.EndsWith(".unity")) path += ".unity";
            string sceneName= System.IO.Path.GetFileNameWithoutExtension(path);
#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                bool isContain = false;
                foreach (UnityEditor.EditorBuildSettingsScene sceneItem in UnityEditor.EditorBuildSettings.scenes)
                {
                    if (sceneItem.path.Contains(sceneName))
                    {
                        isContain = true;
                        break;
                    }
                }
                if (isContain == false)
                {
                    Debug.Log($"BuildSetting In Scene list not Find {sceneName} Scence,Plase Add {sceneName} to scene list!");
                    return null;
                }
                else
                {
                    return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                }
            }
#endif
                uint crc = Crc32.GetCrc32(path);
                //从缓存中获取我们Bundleitem
                BundleItem item = GetCacheItemFormAssetDic(crc);
                if (item == null || item.assetBundle ==null)
                {
                    item= AssetBundleManager.Instance.LoadAssetBundle(crc);
                    if (item != null)
                    {
                        item.path = path;
                        item.crc = crc;
                        item.refCount++;
                        //缓存已经加载过的资源
                        mAlreayLoadAssetsDic.Add(crc, item);
                    }
                }
                return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
         

        }
        public T LoadScriptableObject<T>(string path) where T : UnityEngine.Object
        {
            if (!path.EndsWith(".asset")) path += ".asset";
            return LoadResource<T>(path);
        }
        /// <summary>
        /// 同步加载资源，外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path is Null , return null!");
                return null;
            }
            uint crc = Crc32.GetCrc32(path);
            //从缓存中获取我们Bundleitem
            BundleItem item = GetCacheItemFormAssetDic(crc);

            //如果BundleItem中的对象已经加载过，就直接返回该对象
            if (item.obj != null)
            {
                return item.obj as T;
            }

            //声明新对象
            T obj = null;
#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                obj = LoadAssetsFormEditor<T>(path);
            }
#endif
            if (obj == null)
            {
                //加载该路径对应的AssetBundle
                item = AssetBundleManager.Instance.LoadAssetBundle(crc);
                if (item != null)
                {
                    if (item.assetBundle != null)
                    {
                        obj = item.obj != null ? item.obj as T : item.assetBundle.LoadAsset<T>(item.assetName);
                    }
                    else
                    {
                        Debug.LogError("item.AssetBundle Is Null!");
                    }
                }
                else
                {
                    Debug.LogError("item is null ...Path:" + path);
                    return null;
                }
            }

            item.obj = obj;
            item.path = path;
            item.refCount++;
            //缓存已经加载过的资源
            mAlreayLoadAssetsDic.Add(crc, item);
            
            return obj;
        }

        /// <summary>
        /// 同步加载所有资源，外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T[] LoadAllResource<T>(string path) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path is Null , return null!");
                return null;
            }
            uint crc = Crc32.GetCrc32(path);
            //从缓存中获取我们Bundleitem
            BundleItem item = GetCacheItemFormAssetDic(crc);

            //如果BundleItem中的对象已经加载过，就直接返回该对象
            if (item.obj != null)
            {
                return item.objArr as T[];
            }

            //声明新对象
            UnityEngine.Object[] objArr = null;
#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                objArr = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
            }
#endif
            if (objArr == null)
            {
                //加载该路径对应的AssetBundle
                item = AssetBundleManager.Instance.LoadAssetBundle(crc);
                if (item != null)
                {
                    if (item.assetBundle != null)
                    {
                        objArr = item.objArr != null ? item.objArr : item.assetBundle.LoadAllAssets<T>();
                    }
                    else
                    {
                        Debug.LogError("item.AssetBundle Is Null!");
                    }
                }
                else
                {
                    Debug.LogError("item is null ...Path:" + path);
                    return null;
                }
            }

            item.objArr = objArr;
            item.path = path;
            item.refCount++;
            //缓存已经加载过的资源
            mAlreayLoadAssetsDic.Add(crc, item);

            return objArr as T[];
        }
        /// <summary>
        /// 异步加载资源，外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public void LoadResourceAsync<T>(string path, System.Action<UnityEngine.Object> loadFinish) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path is Null , return null!");
                loadFinish?.Invoke(null);
                return;
            }
            uint crc = Crc32.GetCrc32(path);
            //从缓存中获取我们Bundleitem
            BundleItem item = GetCacheItemFormAssetDic(crc);

            //如果BundleItem中的对象已经加载过，就直接返回该对象
            if (item.obj != null)
            {
                loadFinish?.Invoke(item.obj as T);
                return;
            }

            //声明新对象
            T obj = null;
#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                obj = LoadAssetsFormEditor<T>(path);
                loadFinish?.Invoke(obj);
            }
#endif
            if (obj == null)
            {
                //加载该路径对应的AssetBundle
                item = AssetBundleManager.Instance.LoadAssetBundle(crc);
                if (item != null)
                {
                    if (item.obj != null)
                    {
                        loadFinish?.Invoke(item.obj);
                        item.path = path;
                        item.crc = crc;
                        item.refCount++;
                        mAlreayLoadAssetsDic.Add(crc, item);
                    }
                    else
                    {
                        //通过异步方式加载AssetBudnle
                        AssetBundleRequest request = item.assetBundle.LoadAssetAsync<T>(item.assetName);
                        request.completed += (asyncOption) =>
                        {
                            //资源加载完成
                            UnityEngine.Object loadObj = (asyncOption as AssetBundleRequest).asset;
                            item.obj = loadObj;
                            item.path = path;
                            item.crc = crc;
                            item.refCount++;
                            if (!mAlreayLoadAssetsDic.ContainsKey(crc))
                            {
                                mAlreayLoadAssetsDic.Add(crc, item);
                            }
                            loadFinish?.Invoke(item.obj);
                        };

                    }
                }
                else
                {
                    Debug.LogError("item is null ...Path:" + path);
                    loadFinish?.Invoke(null);
                }
            }
            else
            {
                item.obj = obj;
                item.path = path;
                item.refCount++;
                //缓存已经加载过的资源
                mAlreayLoadAssetsDic.Add(crc, item);
            }
        }
        /// <summary>
        /// 异步加载资源，可使用await进行等待 外部直接调用，仅仅加载不需要实例化的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public async UniTask<T> LoadResourceAsyncAas<T>(string path,BundleModuleEnum moduleEnum= BundleModuleEnum.None) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("path is Null , return null!");
                return null;
            }
            uint crc = Crc32.GetCrc32(path);
            //从缓存中获取我们Bundleitem
            BundleItem item = GetCacheItemFormAssetDic(crc);

            //如果BundleItem中的对象已经加载过，就直接返回该对象
            if (item.obj != null)
            {
                return item.obj as T;
            }

            //声明新对象
            T obj = null;
#if UNITY_EDITOR
            if (BundleSettings.Instance.loadAssetType == LoadAssetEnum.Editor)
            {
                obj = LoadAssetsFormEditor<T>(path);
                if (obj==null)
                {
                    Debug.LogError("Load Object is null, Path:"+ path);
                    return null;
                }
                item.obj = obj;
                item.path = path;
                //缓存已经加载过的资源
                if (!mAlreayLoadAssetsDic.ContainsKey(crc))
                    mAlreayLoadAssetsDic.Add(crc, item);
                return obj;
            }
#endif
            if (obj == null)
            {
                //加载该路径对应的AssetBundle
                if (moduleEnum== BundleModuleEnum.None)
                {
                    item = AssetBundleManager.Instance.LoadAssetBundle(crc);
                }
                else
                {
                    item = await AssetBundleManager.Instance.LoadAssetBundleAddressable(crc, moduleEnum);
                }
                
                if (item == null)
                {
                    Debug.LogError("item is null ...Path:" + path);
                    return null;
                }
                
                if (item.obj != null)
                {
                    item.path = path;
                    item.crc = crc;
                    item.refCount++;
                    mAlreayLoadAssetsDic.Add(crc, item);
                    return obj;
                }
                //通过异步方式加载AssetBudnle
                T loadObj = await item.assetBundle.LoadAssetAsync<T>(item.assetName) as T;
                item.obj = loadObj;
                item.path = path;
                item.crc = crc;
                item.refCount++;
                mAlreayLoadAssetsDic.TryAdd(crc, item);
                return loadObj;
            }
            return null;
        }

        public async UniTask<T> LoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
            return await LoadResourceAsyncAas<T>(path, BundleModuleEnum.None);
        }
        /// <summary>
        /// 从缓存中获取我们Bundleitem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        private BundleItem GetCacheItemFormAssetDic(uint crc)
        {
            mAlreayLoadAssetsDic.TryGetValue(crc, out var item);
            if (item==null)
            {
                return new BundleItem { crc = crc ,refCount = 1};
            }
            item.refCount++;
            return item;;
        }

#if UNITY_EDITOR
        public T LoadAssetsFormEditor<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
        /// <summary>
        /// 移除对象加载回调
        /// </summary>
        /// <param name="loadid"></param>
        public void RemoveObjectLoadCallBack(long loadid)
        {
            if (loadid == -1)
            {
                return;
            }
            if (mLoadObjectCallBackDic.ContainsKey(loadid))
            {
                mLoadObjectCallBackDic.Remove(loadid);
            }
        }
        /// <summary>
        /// 释放对象占用内存
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroyCache"></param>
        public void Release(GameObject obj, bool destroyCache = false)
        {
            
            int insid = obj.GetInstanceID();
            mAllObjectDic.TryGetValue(insid, out var cacheObejct);
            //通过Gameobject.Instantiate 不支持回收，因为对象池中没有记录
            if (cacheObejct == null)
            {
                Debug.LogError("Recycl Obj failed,obj is Gameobject.Instantiate...");
                return;
            }
            if (destroyCache)
            {
                GameObject.Destroy(obj);
                mAllObjectDic.Remove(insid);
                //获取该物体所在对象池
                mObjectPoolDic.TryGetValue(cacheObejct.crc, out var objectPoolList);
                if (objectPoolList != null)
                {
                    //从对象池中移除缓存对象
                    if (objectPoolList.Contains(cacheObejct))
                    {
                        objectPoolList.Remove(cacheObejct);
                    }
                    cacheObejct.Release();
                    mCacheObejctPool.Recycl(cacheObejct);
                    return;
                }
                
                //如果该对象在对象池中不存在，或者已经全部释放了，就卸载该对象AssetBundle的资源占用
                if (mAlreayLoadAssetsDic.TryGetValue(cacheObejct.crc, out BundleItem item))
                {
                    item.refCount--;
                    if (item.refCount == 0)
                    {
                        AssetBundleManager.Instance.ReleaseAssets(item, true);
                        mAlreayLoadAssetsDic.Remove(cacheObejct.crc); 
                    }
                }
                else
                {
                    Debug.LogError("mAlreayLoadAssetsDic not find BundleItem Path:" + cacheObejct.path + " isnid:" + insid);
                }
                cacheObejct.Release();
                mCacheObejctPool.Recycl(cacheObejct);
                //Debug.Log(mCacheObejctPool.PoolCount);
            }
            else
            {
                //回收到对象池
                List<CacheObejct> objList = null;
                mObjectPoolDic.TryGetValue(cacheObejct.crc, out objList);
                //字典中没有该对象池
                if (objList == null)
                {
                    //创建对象池
                    objList = new List<CacheObejct>();
                    objList.Add(cacheObejct);
                    mObjectPoolDic.Add(cacheObejct.crc, objList);
                }
                else
                {
                    //回收到对象池
                    objList.Add(cacheObejct);
                }
                //会受到对象回收节点下
                if (cacheObejct.obj != null)
                {
                    cacheObejct.obj?.transform.SetParent(ZMAsset.RecyclObjPool);
                }
                else
                {
                    Debug.LogError("cacheObejct.obj is Null Release Failed!");
                }
            }
        }
        /// <summary>
        /// 释放图片所占用的内存
        /// </summary>
        /// <param name="texture"></param>
        public void Release(Texture texture)
        {
            Resources.UnloadAsset(texture);
        }
        /// <summary>
        /// 加载图片资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string path)
        {
            if (path.EndsWith(".png") == false) path += ".png";
            return LoadResource<Sprite>(path);
        }
        /// <summary>
        /// 加载Texture图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Texture LoadTexture(string path)
        {
            if (path.EndsWith(".jpg") == false) path += ".jpg";
            return LoadResource<Texture>(path);
        }
        /// <summary>
        /// 加载音频文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AudioClip LoadAudio(string path)
        {
            return LoadResource<AudioClip>(path);
        }
        /// <summary>
        /// 加载Text资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TextAsset LoadTextAsset(string path)
        {
            return LoadResource<TextAsset>(path);
        }
        /// <summary>
        /// 从图集中加载指定的图片
        /// </summary>
        /// <param name="atlasPath"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadAtlasSprite(string atlasPath, string spriteName)
        {
            if (atlasPath.EndsWith(".spriteatlas") == false) atlasPath += ".spriteatlas";
            return LoadSpriteFormAltas(LoadResource<SpriteAtlas>(atlasPath), spriteName);
        }
        /// <summary>
        /// 从图集中加载指定名称的图片
        /// </summary>
        /// <param name="spriteAtlas"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Sprite LoadSpriteFormAltas(SpriteAtlas spriteAtlas, string name)
        {
            if (spriteAtlas == null)
            {
                Debug.LogError("Not find spriteAtlas Name:" + name);
                return null;
            }

            //从图集中获取指定名称的图片
            Sprite sprite = spriteAtlas.GetSprite(name);
            if (sprite != null)
            {
                return sprite;
            }
            Debug.LogError("Not find Sprite  Name:" + name);
            return null;
        }


        /// <summary>
        /// 加载tpsheet图集
        /// </summary>
        /// <param name="path"></param>
        public Sprite LoadPNGAtlasSprite(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (!path.EndsWith(".png"))
            {
                path += ".png";
            }
            UnityEngine.Object[] objectArr = null;
            //优先从缓存中读取
            if (mAllAssetObjectDic.TryGetValue(path, out objectArr) && objectArr != null)
            {
                return LoadSpriteFormAltas(objectArr, name);
            }
            //通过Asset Bundle加载该文件中的所有资源
            UnityEngine.Object[] objects = LoadAllResource<UnityEngine.Object>(path);
            //缓存至图集列表中
            mAllAssetObjectDic.Add(path, objects);
            return LoadSpriteFormAltas(objects, name);
        }
        /// <summary>
        /// 从图集中加载图片
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Sprite LoadSpriteFormAltas(UnityEngine.Object[] objects, string name)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (string.Equals(name, objects[i].name))
                {
                    return objects[i] as Sprite;
                }
            }
            Debug.LogError("没有找到名字为" + name + "的图片 请检查图片名称是否正确！");
            return null;
        }



        /// <summary>
        /// 异步加载图片
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loadAsync">异步加载回调</param>
        /// <param name="param1">参数1</param>
        /// <returns></returns>
        public long LoadTextureAsync(string path, Action<Texture, object> loadAsync, object param1 = null)
        {
            if (path.EndsWith(".jpg") == false) path += ".jpg";

            long guid = mAsyncTaskGuid;
            mAsyncLoadingTaskList.Add(guid);
            LoadResourceAsync<Texture>(path, (obj) =>
            {

                if (obj != null)
                {
                    if (mAsyncLoadingTaskList.Contains(guid))
                    {
                        mAsyncLoadingTaskList.Remove(guid);
                        loadAsync?.Invoke(obj as Texture, param1);
                    }
                }
                else
                {
                    mAsyncLoadingTaskList.Remove(guid);
                    Debug.LogError("Async Load texture is Null,Path:" + path);
                }
            });
            return guid;
        }
        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="image">Inage组件</param>
        /// <param name="setNativeSize">是否设置未美术图的原始尺寸</param>
        /// <param name="loadAsync">加载完成的回调</param>
        /// <returns></returns>
        public long LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> loadAsync = null)
        {
            if (path.EndsWith(".png") == false) path += ".png";

            long guid = mAsyncTaskGuid;
            mAsyncLoadingTaskList.Add(guid);
            LoadResourceAsync<Sprite>(path, (obj) =>
            {
                if (obj != null)
                {
                    if (mAsyncLoadingTaskList.Contains(guid))
                    {
                        Sprite sprite = obj as Sprite;
                        if (image != null)
                        {
                            image.sprite = sprite;
                            if (setNativeSize)
                            {
                                image.SetNativeSize();
                            }
                        }
                        mAsyncLoadingTaskList.Remove(guid);
                        loadAsync?.Invoke(sprite);
                    }
                }
                else
                {
                    mAsyncLoadingTaskList.Remove(guid);
                    Debug.LogError("Async Load Sprite is Null,Path:" + path);
                }
            });
            return guid;
        }
        /// <summary>
        /// 清理所有异步加载任务
        /// </summary>
        public void ClearAllAsyncLoadTask()
        {
            mAsyncLoadingTaskList.Clear();
        }
        /// <summary>
        /// 清理加载的资源，释放内存
        /// </summary>
        /// <param name="absoluteCleaning">深度清理：true：销毁所有由AssetBUnle加载和生成的对象，彻底释放内存占用
        /// 深度清理 false：销毁对象池中的对象，但不销毁由AssetBundle克隆出并在使用的对象，具体的内存释放根据内存引用计数选择性释放</param>
        public void ClearResourcesAssets(bool absoluteCleaning)
        {
            if (absoluteCleaning)
            {
                foreach (var item in mAllObjectDic)
                {
                    if (item.Value.obj != null)
                    {
                        //销毁Gameobject对象，回收缓存类对象，等待下次复用
                        GameObject.Destroy(item.Value.obj);
                        item.Value.Release();
                        mCacheObejctPool.Recycl(item.Value);
                    }
                }
                //清理列表
                mAllObjectDic.Clear();
                mObjectPoolDic.Clear();
                ClearAllAsyncLoadTask();
            }
            else
            {
                foreach (var objList in mObjectPoolDic.Values)
                {
                    if (objList != null)
                    {
                        foreach (var cacheObejct in objList)
                        {
                            if (cacheObejct != null)
                            {
                                //销毁Gameobject对象，回收缓存类对象，等待下次复用
                                GameObject.Destroy(cacheObejct.obj);
                                cacheObejct.Release();
                                mCacheObejctPool.Recycl(cacheObejct);
                            }
                        }
                    }
                }

                mObjectPoolDic.Clear();
            }
            //释放AssetBundle 及里面的资源所占用的内存
            foreach (var item in mAlreayLoadAssetsDic)
            {
                AssetBundleManager.Instance.ReleaseAssets(item.Value, absoluteCleaning);
            }

            //清理列表
            mLoadObjectCallBackDic.Clear();
            mAlreayLoadAssetsDic.Clear();
            mAllAssetObjectDic.Clear();
            //释放未使用的资源 (未使用的资源指的是 没有被引用的资源)
            Resources.UnloadUnusedAssets();
            //触发GC垃圾回收
            System.GC.Collect();
        }

        public void Release(AssetsRequest request)
        {
            mAssetsRequestPool.Recycl(request);
        }
        /// <summary>
        /// 初始化资源模块
        /// </summary>
        /// <param name="bundleModule">模块类型</param>
        /// <param name="isAddressableAsset">是否是寻址资源</param>
        /// <returns></returns>
        public async UniTask<bool> InitAssetModule(BundleModuleEnum bundleModule, bool isAddressableAsset = false)
        {
            if (!isAddressableAsset)
            {
               return await AssetBundleManager.Instance.InitAssetModule(bundleModule);
            }
            else
            {
               return await AddressableAssetSystem.Instance.InitAddressableModule(bundleModule, AddressableAssetSystem.Instance.GetAddressableModule(bundleModule));
            }
        }
        #endregion
    }
}