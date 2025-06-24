using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZM.ZMAsset
{
    public interface IAddressableAssetInterface
    {
        UniTask<AssetsRequest> InstantiateAsyncFormPoolAas(string path,Transform parent, BundleModuleEnum bundleModule, object param1 = null, object param2 = null, object param3 = null);

        UniTask<T> LoadResourceAsyncAas<T>(string fullPath, BundleModuleEnum moduleEnum) where T : UnityEngine.Object;
    }
}