using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.TerrainAPI;

namespace ZM.AssetFrameWork
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
        public async Task<bool> InitAddressableModule(BundleModuleEnum module, AddressableModule assetModule)
        {
            if (assetModule.IsHotAsset)
            {
                //检测资源版本并计算需要热更的资源
                await assetModule.CheckAssetsVersion();
                //计算AssetBundle配置文件是否需要热更
                HotFileInfo hotInfo = assetModule.AssetIsNeedHotUpdate(BundleSettings.Instance.GetBundleCfgName(module));
                if (hotInfo != null)
                {
                    string mBundleConfigName = module.ToString().ToLower() + "bundleconfig" + BundleSettings.ABSUFFIX;
                    string mBundleConfigPath = BundleSettings.Instance.GetHotAssetsPath(module) + mBundleConfigName;
                    File.Delete(mBundleConfigPath);
                    //热更AssetBundle配置文件
                    await HotAssetBundleFile(assetModule, module, hotInfo);
                    assetModule.IsHotAsset = false;
                }
            }
            return await AssetBundleManager.Instance.InitAssetModule(module);
        }
       
        public async Task<bool> LoadAddressableAsset(BundleModuleEnum module,uint crc, string bundleName)
        {
            //下载流程：资源清单文件 - 资源配置文件 - 对应资源 + 资源依赖包，全部下载完成后进行加载。
            AddressableModule asset= GetAddressableModule(module);

            //通过 await mTaskCompletionSouce让所有执行到这的代码全都等待mTaskCompletionSouce(资源版本检测和文件清单校验完成)任务完成
            if (asset.TaskCompletionSouce != null) 
                await asset.TaskCompletionSouce.Task;

            //1.验证资源清单文件
            if (asset.IsHotAsset)
            {
                //设置等待标记，让其他资源等待 AssetBundle清单文件下载成功后进行下载
                asset.TaskCompletionSouce = new TaskCompletionSource<bool>();
                bool initState= await InitAddressableModule(module, asset);
                //设置为任务完成,让所有等待的资源加载任务开始资源加载
                asset.TaskCompletionSouce?.SetResult(true);
                asset.TaskCompletionSouce = null;
                if (initState == false) return false;
            }
            if (crc==0) return true;
            //3.下载对应资源
            BundleItem item = AssetBundleManager.Instance.GetBundleItemByCrc(crc);
            if (item==null) return false;
         
            //下载资源所在Bundle包
            HotFileInfo mainHotInfo = asset.AssetIsNeedHotUpdate(item.bundleName);
            bool loadResult= await HotAssetBundleFile(asset, module, mainHotInfo);
            if (loadResult==false)  return false;

            //下载资源依赖Bundle包
            List<string> dependce= item.bundleDependce;
            foreach (string abName in dependce) {
                if (!string.Equals(abName,bundleName))
                {
                    HotFileInfo dependHotInfo = asset.AssetIsNeedHotUpdate(abName);
                    loadResult = await HotAssetBundleFile(asset, module, dependHotInfo);
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
        private async Task<bool> HotAssetBundleFile(AddressableModule asset,BundleModuleEnum module,HotFileInfo hotFileInfo)
        {
            if (hotFileInfo != null)
            {
                DownLoadThread downLoadItem = new DownLoadThread(module, hotFileInfo, asset.HotAssetDownLoadUrl, asset.HotAssetsSavePath);
                bool downLoadSuccess = await downLoadItem.StartDownLoadAsync();
                if (downLoadSuccess)
                    asset.RemoveNeedHotAsset(hotFileInfo);
                return downLoadSuccess;
            }
            return false;
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
