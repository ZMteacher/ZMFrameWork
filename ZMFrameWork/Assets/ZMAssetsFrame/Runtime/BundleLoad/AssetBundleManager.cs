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
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class BundleItem
    {
        /// <summary>
        /// 文件加载路径
        /// </summary>
        public string path;
        /// <summary>
        /// 文件加载路径crc
        /// </summary>
        public uint crc;
        /// <summary>
        /// AssetBundle名称
        /// </summary>
        public string bundleName;
        /// <summary>
        /// 资源名称
        /// </summary>
        public string assetName;
        /// <summary>
        /// AssetBundle所属的模块
        /// </summary>
        public BundleModuleEnum bundleModuleType;
        /// <summary>
        /// AssetBundle依赖项
        /// </summary>
        public List<string> bundleDependce;
        /// <summary>
        /// AssetBundle
        /// </summary>
        public AssetBundle assetBundle;
        /// <summary>
        /// 通过AssetBundle加载出的对象
        /// </summary>
        public UnityEngine.Object obj;
        /// <summary>
        /// 通过AssetBundle加载出的对象数组
        /// </summary>
        public UnityEngine.Object[] objArr;
    }

    /// <summary>
    /// AssetBundle缓存
    /// </summary>
    public class AssetBundleCache
    {
        /// <summary>
        /// AssetBundle对象
        /// </summary>
        public AssetBundle assetBundle;
        /// <summary>
        /// AssetBundle引用计数
        /// </summary>
        public int referenceCount;

        public void Release()
        {
            assetBundle = null;
            referenceCount = 0;
        }
    }

    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        /// <summary>
        /// 已经加载的资源模块
        /// </summary>
        private List<BundleModuleEnum> mAlreadyLoadBundleModuleList = new List<BundleModuleEnum>();
        /// <summary>
        /// 所有模块的AssetBundle的资源对象字典
        /// </summary>
        private Dictionary<uint, BundleItem> mAllBundleAssetDic = new Dictionary<uint, BundleItem>();

        /// <summary>
        /// 所有模块的已经加载过的AssetBundle的资源对象字典
        /// </summary>
        private Dictionary<string, AssetBundleCache> mAllAlreadyLoadBundleDic = new Dictionary<string, AssetBundleCache>();

        /// <summary>
        /// AssetBundle类对象池
        /// </summary>
        public ClassObjectPool<AssetBundleCache> mBundleCachePool = new ClassObjectPool<AssetBundleCache>(200);
        /// <summary>
        /// AssetBundle配置文件加载路径
        /// </summary>
        private string mBundleConfigPath;
        /// <summary>
        /// AssetBundle配置文件名称
        /// </summary>
        private string mBundleConfigName;
        /// <summary>
        /// AssetBundle配置文件名称
        /// </summary>
        private string mAssetsBundleConfigName;


        /// <summary>
        /// 生成AssetBundleConfig配置文件路径
        /// </summary>
        /// <param name="bundleModule"></param>
        /// <returns></returns>
        public bool GeneratorBundleConfigPath(BundleModuleEnum bundleModule)
        {
            mAssetsBundleConfigName = bundleModule.ToString().ToLower() + "assetbundleconfig";
            mBundleConfigName = bundleModule.ToString().ToLower() + "bundleconfig"+ BundleSettings.ABSUFFIX;
            mBundleConfigPath = BundleSettings.Instance.GetHotAssetsPath(bundleModule) + mBundleConfigName;
            //如果配置文件 存在，return true，如果不存，我们就直接从内嵌解压的资源中去加载。
            //如果内嵌解压的文件中不存在，说明用户网络有问题
            if (!File.Exists(mBundleConfigPath))
            {
                mBundleConfigPath = BundleSettings.Instance.GetAssetsDecompressPath(bundleModule) + mBundleConfigName;
                if (!File.Exists(mBundleConfigPath))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 加载AssetBundle配置文件
        /// </summary>
        /// <param name="bundleModule"></param>
        public void LoadAssetBundleConfig(BundleModuleEnum bundleModule)
        {
            try
            {
                if (mAlreadyLoadBundleModuleList.Contains(bundleModule))
                {
                    Debug.LogError("该模块配置文件已经加载："+bundleModule);
                    return;
                }
               
                //获取当前模块配置文件所在的路径
                if (GeneratorBundleConfigPath(bundleModule))
                {
                    AssetBundle bundleConfig = null;
                    //如果该AssetBundle已经加密，则需要解密
                    if (BundleSettings.Instance.bundleEncrypt.isEncrypt)
                    {
                        bundleConfig= AssetBundle.LoadFromMemory(AES.AESFileByteDecrypt(mBundleConfigPath,BundleSettings.Instance.bundleEncrypt.encryptKey));
                    }
                    else
                    {
                          bundleConfig = AssetBundle.LoadFromFile(mBundleConfigPath);
                    }

                    string bundleConfigJson = bundleConfig.LoadAsset<TextAsset>(mAssetsBundleConfigName).text;
                    BundleConfig bundleManife = JsonConvert.DeserializeObject<BundleConfig>(bundleConfigJson);
                    //把所有的AssetBundle信息存放至字典中，管理起来
                    foreach (var info in bundleManife.bundleInfoList)
                    {
                        if (!mAllBundleAssetDic.ContainsKey(info.crc))
                        {
                            BundleItem item = new BundleItem();
                            item.path = info.path;
                            item.crc = info.crc;
                            item.bundleModuleType = bundleModule;
                            item.assetName = info.assetName;
                            item.bundleDependce = info.bundleDependce;
                            item.bundleName = info.bundleName;
                            mAllBundleAssetDic.Add(item.crc, item);
                        }
                        else
                        {
                            Debug.LogError("AssetBundle Already Exists! BundleName:" + info.bundleName);
                        }
                    }
                    //释放AssetBunle配置
                    bundleConfig.Unload(false);
                    mAlreadyLoadBundleModuleList.Add(bundleModule);
                }
                else
                {
                    Debug.LogError("AssetBundleConfig Not find.  Load AssetBundle failed!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Load AssetBundleConfig Failed, Exception:"+e);
            }
          
        }
        /// <summary>
        /// 根据AssetBundle名称查询该AssetBUndle中都有那些资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public List<BundleItem> GetBundleItemByABName(string bundleName)
        {
            List<BundleItem> itemList = new List<BundleItem>();
            foreach (var item in mAllBundleAssetDic.Values)
            {
                if (string.Equals(item.bundleName,bundleName))
                {
                    itemList.Add(item);
                }
            }
            return itemList;
        }
        /// <summary>
        /// 通过资源路径的Crc加载该资源所在AssetBundle
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public BundleItem LoadAssetBundle(uint crc)
        {
            BundleItem item = null;

            //先到所有的AssetBunel资源字典中查询一下这个资源存不存在，如果存在说明该资源已经打成了AssetBundle包，这种情况下就可以直接加载了
            //如果不存在，则说明该资源 不属于AssetBUnle 给与错误提示。
            mAllBundleAssetDic.TryGetValue(crc, out item);

            if (item!=null)
            {
                //如果AssetBundle为空，说明该资源所在的AssetBundle没有加载进内存，这种情况我们就需要加载该AssetBundle
                if (item.assetBundle==null)
                {
                    item.assetBundle = LoadAssetBundle(item.bundleName,item.bundleModuleType);
                    //需要加载这个AssetBundle依赖的其他的AssetBundle
                    foreach (var bundleName in item.bundleDependce)
                    {
                        if (item.bundleName!=bundleName)
                        {
                            LoadAssetBundle(bundleName, item.bundleModuleType);
                        }
                    }
                    return item;
                }
                else
                {
                    return item;
                }
            }
            else
            {
                Debug.LogError("assets not exists AssetbundleConfig , LoadAssetBundle failed! Crc:"+crc);
                return null;
            }
        }
        /// <summary>
        /// 通过AssetBundle Name加载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="bundleModuleType"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string bundleName, BundleModuleEnum bundleModuleType)
        {
            AssetBundleCache bundle = null;
            mAllAlreadyLoadBundleDic.TryGetValue(bundleName,out bundle);

            if (bundle==null||(bundle!=null&&bundle.assetBundle==null))
            {
                //从类对象池中取出一个AssetBundleCache
                bundle= mBundleCachePool.Spawn();
                //计算出AssetBundle加载路径
                string hotFilePath = BundleSettings.Instance.GetHotAssetsPath(bundleModuleType)+bundleName;
                //获取热更模块
                HotAssetsModule module= ZMAssetsFrame.GetHotAssetsModule(bundleModuleType);
                bool isHotPath = module==null?(File.Exists(hotFilePath)?true:false):(module.HotAssetCount==0?(File.Exists(hotFilePath) ? true : false):module.HotAssetsIsExists(bundleName));
                //通过是否是热更路径 计算出AssetBundle加载的路径
                string bundlePath = isHotPath ? hotFilePath : BundleSettings.Instance.GetAssetsDecompressPath(bundleModuleType) + bundleName;

                //判断AssetBUndle是否加密，如果加密了，则需要解密
                if (BundleSettings.Instance.bundleEncrypt.isEncrypt)
                {
                    byte[] bytes= AES.AESFileByteDecrypt(bundlePath, BundleSettings.Instance.bundleEncrypt.encryptKey);
                    bundle.assetBundle= AssetBundle.LoadFromMemory(bytes);
                }
                else
                {
                    //通过LoadFromFile 加载AssetBundle 是最快的
                    bundle.assetBundle = AssetBundle.LoadFromFile(bundlePath);
                }
                if (bundle.assetBundle==null)
                {
                    Debug.LogError("AssetBundle load failed bundlePath:"+ bundlePath);
                    return null;
                }
                //AssetBundle引用计数增加
                bundle.referenceCount++;
                mAllAlreadyLoadBundleDic.Add(bundleName,bundle);
            }
            else
            {
                //AssetBunle已经加载过了
                bundle.referenceCount++;
            }
            return bundle.assetBundle;
        }


        /// <summary>
        /// 释放AssetBundle 并且释放AssetBundle占用的内存资源
        /// </summary>
        /// <param name="assetitem"></param>
        /// <param name="unLoad"></param>
        public void ReleaseAssets(BundleItem assetitem,bool unLoad)
        {
            //AssetBUndle释放策略一般有两种
            //1.第一种：
            // 以AssetBundle.UnLoad(false) 为主
            // 对于非对象资源，比如 text texture audio等 ,资源加载完成后，就可以直接通过AssetBundle.UnLoad(false)释放AssetBundle的镜像文件
            // 对于对象资源 比如Gameobject 我们需要在上层做一个引用计数的对象池，obj在加载出来之后就可以使用AssetBundle.UnLoad(false)释放AssetBundle的镜像文件
            // 因为后续我们访问的对象都是对象池中的物体了

            //2.第二种
            //以AssetBundle.UnLoad(true) 为主
            // 在加载AssetBundle 时做一个缓存，后续加载的所有的资源对象全部通过缓存的AssetBUndle进行加载
            // 在跳转场景的时候 通过 AssetBundle.UnLoad(true) 彻底释放所有的资源与内存占用

            //AssetBundle assetBundle = null;
            if (assetitem!=null)
            {
                if (assetitem.obj!=null)
                {
                    assetitem.obj = null;
                }
                if (assetitem.objArr!=null)
                {
                    assetitem.objArr = null;
                }

                ReleaseAssetBundle(assetitem,unLoad);

                if (assetitem.bundleDependce!=null)
                {
                    foreach (var bundleName in assetitem.bundleDependce)
                    {
                        //根据内存引用计数释放AssetBundle
                        ReleaseAssetBundle(null,unLoad, bundleName);
                    }
                }
            }
            else
            {
                Debug.LogError(" assetitem is null, release Assets failed!");
            }
        }
        /// <summary>
        /// 释放AssetBundle所占用的资源
        /// </summary>
        /// <param name="assetitem"></param>
        /// <param name="unLoad"></param>
        /// <param name="bundleName"></param>
        public void ReleaseAssetBundle(BundleItem assetitem, bool unLoad,string bundleName="")
        {
            string assetBudnleName = "";
            if (assetitem == null)
            {
                assetBudnleName = bundleName;
            }
            else
            {
                assetBudnleName = assetitem.bundleName;
            }
            AssetBundleCache bundleCacheItem = null;
            //如果该AssetBUndle的名字不为空，与我们的这个AssetBundle已经加载过了
            if (!string.IsNullOrEmpty(assetBudnleName) && mAllAlreadyLoadBundleDic.TryGetValue(assetBudnleName, out bundleCacheItem))
            {
                if (bundleCacheItem.assetBundle != null)
                {
                    bundleCacheItem.referenceCount--;
                    //如果该AssetBundle内存引用小于等于0 就说明没有人引用了，就可以直接释放了
                    if (bundleCacheItem.referenceCount <= 0)
                    {
                        bundleCacheItem.assetBundle.Unload(unLoad);
                        mAllAlreadyLoadBundleDic.Remove(assetBudnleName);
                        //回收BundleCacheitem类对象
                        bundleCacheItem.Release();
                        mBundleCachePool.Recycl(bundleCacheItem);
                    }
                }
            }
        }
    }

}