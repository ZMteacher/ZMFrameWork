using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace ZM.ZMAsset
{
 
    //1.确定某个模块是否为寻址资源加载
    //2.加载该资源时，若没有加载到该资源，判断该资源是否为寻址模块，若为寻址模块，进行寻址下载

    //下载流程：资源清单文件-资源配置文件-对应资源+资源依赖包，全部下载完成后进行加载。
    //疑难问题：资源及其依赖下载需要等待 资源清单文件-资源配置文件下载完成后才能进行下载。


    /// <summary>
    /// 资源可寻址加载系统
    /// </summary>
    public class AddressableAssetSystem:Singleton<AddressableAssetSystem>
    {

        /// <summary>
        /// 所有热更资源模块
        /// </summary>
        private Dictionary<BundleModuleEnum, AddressableModule> mAllAssetsModuleDic = new Dictionary<BundleModuleEnum, AddressableModule>();
 
        /// <summary>
        /// 初始化寻址资源模块
        /// </summary>
        /// <param name="module"></param>
        /// <param name="assetModule"></param>
        /// <returns></returns>
        public async UniTask<bool> InitAddressableModule(BundleModuleEnum module, AddressableModule assetModule)
        {
            if (assetModule.IsHotAsset)
            {
                //检测资源版本并计算需要热更的资源
                await assetModule.CheckAssetsVersion();
                //计算AssetBundle配置文件是否需要热更
                HotFileInfo hotInfo = assetModule.AssetIsNeedHotUpdate(BundleSettings.Instance.GetBundleCfgName(module));
                if (hotInfo != null)
                {
                    string mBundleConfigName = module.ToString().ToLower() + "bundleconfig" + BundleSettings.Instance.ABSUFFIX;
                    string mBundleConfigPath = BundleSettings.Instance.GetHotAssetsPath(module) + mBundleConfigName;
                    File.Delete(mBundleConfigPath);
                    //热更AssetBundle配置文件
                    await HotAssetBundleFile(assetModule, module, hotInfo);
                }
                assetModule.IsHotAsset = false;
            }
            return await AssetBundleManager.Instance.InitAssetModule(module);
        }
       
        public async Task<bool> LoadAddressableAsset(BundleModuleEnum module,uint crc, string bundleName)
        {
            //下载流程：资源清单文件 - 资源配置文件 - 对应资源 + 资源依赖包，全部下载完成后进行加载。
            AddressableModule assetModule= GetAddressableModule(module);

            //通过 await mTaskCompletionSouce让所有执行到这的代码全都等待mTaskCompletionSouce(资源版本检测和文件清单校验完成)任务完成
            if (assetModule.TaskCompletionSouce != null) 
                await assetModule.TaskCompletionSouce.Task;

            //1.验证资源清单文件
            if (assetModule.IsHotAsset)
            {
                //设置等待标记，让其他资源等待 AssetBundle清单文件下载成功后进行下载
                assetModule.TaskCompletionSouce = new TaskCompletionSource<bool>();
                bool initState= await InitAddressableModule(module, assetModule);
                //设置为任务完成,让所有等待的资源加载任务开始资源加载
                assetModule.TaskCompletionSouce?.SetResult(true);
                assetModule.TaskCompletionSouce = null;
                if (initState == false) return false;
            }
            if (crc==0) return true;
            //3.下载对应资源
            BundleItem item = AssetBundleManager.Instance.GetBundleItemByCrc(crc);
            if (item==null) return false;
         
            //下载资源所在Bundle包
            HotFileInfo mainHotInfo = assetModule.AssetIsNeedHotUpdate(item.bundleName);
            bool loadResult= await HotAssetBundleFile(assetModule, module, mainHotInfo);
            if (loadResult==false)  return false;

            //下载资源依赖Bundle包
            List<string> dependce= item.bundleDependce;
            foreach (string abName in dependce) {
                if (!string.Equals(abName,bundleName))
                {
                    HotFileInfo dependHotInfo = assetModule.AssetIsNeedHotUpdate(abName);
                    loadResult = await HotAssetBundleFile(assetModule, module, dependHotInfo);
                    if (loadResult == false) return false;
                }
            }
            return true;
         }
        /// <summary>
        /// 热更AssetBundle文件
        /// </summary> 
        /// <param name="asset">资源模块</param>
        /// <param name="module">资源模块类型</param>
        /// <param name="hotFileInfo">热更文件信息</param>
        /// <returns></returns>
        private async UniTask<bool> HotAssetBundleFile(AddressableModule asset,BundleModuleEnum module,HotFileInfo hotFileInfo)
        {
            //不等于Null说明许需要热更
            if (hotFileInfo != null)
            {
                DownLoadThread downLoadItem = new DownLoadThread(module, hotFileInfo, asset.HotAssetDownLoadUrl, asset.HotAssetsSavePath);
                bool downLoadSuccess = await downLoadItem.StartDownLoadAsync();
                if (downLoadSuccess)
                    asset.RemoveNeedHotAsset(hotFileInfo);
                return downLoadSuccess;
            }
            //已经热更完成
            return true;
        }

        /// <summary>
        /// 获取寻址资源模块
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public AddressableModule GetAddressableModule(BundleModuleEnum module)
        {
            mAllAssetsModuleDic.TryGetValue(module, out var asset);
            if (asset == null)
            {
                asset = new AddressableModule(module);
                mAllAssetsModuleDic.Add(module, asset);
            }
            return asset;
        }
    }
   

}
