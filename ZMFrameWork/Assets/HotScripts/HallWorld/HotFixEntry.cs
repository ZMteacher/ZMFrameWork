using System;
using System.Collections.Generic;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using ZM.ZMAsset;
using ZMGC.Hall;
using Cysharp.Threading.Tasks;
public class HotFixEntry
{
    public static async void OnStatGame()
    {
        Debug.Log("# HotFixEntry OnStatGame...");
        //加载AOT补元数据
        await LoadAOTGenericMetadata();
        //初始化UI框架
        UIModule.Instance.Initialize();
        //构建大厅世界
        WorldManager.CreateWorld<HallWorld>();
    }

    /// <summary>
    /// 加载泛型补元数据
    /// </summary>
    public static async UniTask<int> LoadAOTGenericMetadata()
    {
        List<string> aotDllList = new List<string>
        {
            "ZM.UI.dll.bytes",
            "mscorlib.dll.bytes",
            "System.dll.bytes",
            "System.Core.dll.bytes", // 如果使用了Linq，需要这个
        };
    
        foreach (var aotDllName in aotDllList)
        {
            TextAsset textAsset = await ZMAsset.LoadTextAssetAsync(AssetsPathConfig.HALL_HOTFIXDLL_PATH + aotDllName);
            if (textAsset != null)
            {
                RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly Success:{aotDllName}");
            }
            else
            {
                Debug.Log($"LoadMetadataForAOTAssembly Failed :{aotDllName}");
            }
        }
        return 0;
    }
    
    
}