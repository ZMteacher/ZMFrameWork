using Cysharp.Threading.Tasks;

namespace ZM.ZMAsset 
{ 
    public  class ZMAddressableAsset 
    {

        public static IAddressableAssetInterface Interface;
        /// <summary>
        /// 异步实例化可寻址资源系统对象
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="bundleModule">资源模块</param>
        /// <param name="param1">透传参数1 (回调触发时返回)</param>
        /// <param name="param2">透传参数2 (回调触发时返回)</param>
        /// <param name="param3">透传参数3 (回调触发时返回)</param>
        /// <returns></returns>
        public static async UniTask<AssetsRequest> InstantiateAsyncFormPool(string path, BundleModuleEnum bundleModule, object param1 = null, object param2 = null, object param3 = null)
        {
             return await Interface.InstantiateAsyncFormPoolAas(path, bundleModule, param1, param2, param3);
        }

        /// <summary>
        /// 异步加载可寻址资源(无论资源在本地还是在网络上，该接口都能准确无误加载到)
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="moduleEnum">可寻址资源模块</param>
        /// <returns></returns>
        public static async UniTask<T> LoadResourceAsync<T>(string fullPath, BundleModuleEnum moduleEnum) where T : UnityEngine.Object
        {
            return await Interface.LoadResourceAsyncAas<T>(fullPath, moduleEnum);
        }

      
    }
}
