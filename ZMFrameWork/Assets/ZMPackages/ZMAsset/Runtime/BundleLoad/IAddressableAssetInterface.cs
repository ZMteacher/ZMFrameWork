using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZM.ZMAsset 
{
    public interface IAddressableAssetInterface
    {
          Task<AssetsRequest> InstantiateAsyncFormPoolAas(string path, BundleModuleEnum bundleModule, object param1 = null, object param2 = null, object param3 = null);

          Task<T> LoadResourceAsyncAas<T>(string fullPath, BundleModuleEnum moduleEnum) where T : UnityEngine.Object;
    }
}
