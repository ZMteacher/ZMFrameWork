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
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using ZM.ZMAsset;
/// <summary>
/// AssetBundle热更模式
/// </summary>
public enum BundleHotEnum
{ 
    NoHot,
    Hot,
}
/// <summary>
/// 资源加载模式
/// </summary>
public enum LoadAssetEnum
{
    Editor,
    AssetBundle,
}

[CreateAssetMenu(menuName = "AssetsBundleSettings",fileName = "AssetsBundleSettings",order =0)]
public class BundleSettings : ScriptableObject
{
    private static BundleSettings _instance;
    public static BundleSettings Instance
    {
        get
        {

            if (_instance==null)
            {
                _instance = Resources.Load<BundleSettings>("AssetsBundleSettings");
            }
            return _instance;
        } 
    }
 



    [TitleGroup("资源加载热更设置"),LabelText("AssetBundle下载地址")]
    public string AssetBundleDownLoadUrl;

    [TitleGroup("AssetBundle打包设置")]
    [LabelText("是否加密AssetBundle")]
    public BundleEncryptToggle bundleEncrypt = new BundleEncryptToggle();

    //AssetBundle后缀 例：.ab 建议不加后缀，防止内嵌时Unity读取出错
    [TitleGroup("AssetBundle打包设置")]
    [LabelText("AssetBundle后缀")]
    public string ABSUFFIX = "";

    [TitleGroup("AssetBundle打包设置")]
    [LabelText("资源压缩格式")]
    public BuildAssetBundleOptions buildbundleOptions;

    [TitleGroup("AssetBundle打包设置")]
    [LabelText("资源打包平台")]
    public BuildTarget buildTarget;



    [TitleGroup("资源加载热更设置")]
    [LabelText("资源热更模式")]
    public BundleHotEnum bundleHotType;

    [TitleGroup("资源加载热更设置")]
    [LabelText("资源加载模式")]
    public LoadAssetEnum loadAssetType;

    [TitleGroup("资源加载热更设置")]
    [LabelText("最大下载线程数量")]
    public int MAX_THREAD_COUNT;

    [TitleGroup("框架相关配置")]
    [LabelText("ZMAsset框架路径")]
    [InfoBox("资源框架总路径节点(基于Assets目录下)，若需要修改资源框架存放位置，需要同步到这里")]
    public string ZMAssetRootPath = "ThirdParty/ZMAsset";
    [Title("AssetBundle热更文件储存路径")]
    private string HotAssetsPath { get { return Application.persistentDataPath + "/HotAssets/"; } }
    [Title("AssetBundle解压路径")]
    private string BundleDecompressPath { get { return Application.persistentDataPath + "/DecompressAssets/"; } }

    [Title("AssetBundle内嵌文件路径")]
    private string BuiltinAssetsPath { get { return Application.streamingAssetsPath + "/AssetBundle/"; } }
    /// <summary>
    /// 获取资源内嵌的路径
    /// </summary>
    /// <param name="moduleEnum"></param>
    /// <returns></returns>
    public string GetAssetsBuiltinBundlePath(BundleModuleEnum moduleEnum)
    {
        return BuiltinAssetsPath + moduleEnum + "/";
    }
    /// <summary>
    /// 获取解压文件路径(Unity2019 支持直接都streamingAssetsPath目录下Bundle)
    /// </summary>
    /// <param name="moduleEnum"></param>
    /// <returns></returns>
    public string GetAssetsDecompressPath(BundleModuleEnum moduleEnum)
    {
#if UNITY_2020_1_OR_NEWER
        return $"{Application.persistentDataPath}/DecompressAssets/{moduleEnum.ToString()}/";
#else
        return BundleDecompressPath + moduleEnum + "/";
#endif
        
    }
    /// <summary>
    /// 获取热更文件储存路径
    /// </summary>
    /// <param name="moduleEnum"></param>
    /// <returns></returns>
    public string GetHotAssetsPath(BundleModuleEnum moduleEnum)
    {
        return HotAssetsPath + moduleEnum + "/";
    }
    /// <summary>
    /// 获取配置文件名称
    /// </summary>
    /// <param name="moduleEnum"></param>
    /// <returns></returns>
    public string GetBundleCfgName(BundleModuleEnum moduleEnum)
    {
        return $"{moduleEnum.ToString().ToLower()}bundleconfig{ABSUFFIX}";
    }
}

[System.Serializable,Toggle("isEncrypt")]
public class BundleEncryptToggle
{
    //是否加密
    public bool isEncrypt;
    [LabelText("加密密钥")]
    public string encryptKey;
}

public enum BuildTarget
{
    //
    // 摘要:
    //     OBSOLETE: Use iOS. Build an iOS player.
    iPhone = -1,
    //
    // 摘要:
    //     Build a macOS standalone (Intel 64-bit).
    StandaloneOSX = 2,
    StandaloneOSXUniversal = 3,
    //
    // 摘要:
    //     Build an iOS player.
    iOS = 9,
    //
    // 摘要:
    //     Build an Android .apk standalone app.
    Android = 13,
    //
    // 摘要:
    //     Build a Linux standalone.
    StandaloneLinux = 17,
    //
    // 摘要:
    //     Build a Windows 64-bit standalone.
    StandaloneWindows64 = 19,
}

//
// 摘要:
//     Asset Bundle building options.
 
public enum BuildAssetBundleOptions
{
    //
    // 摘要:
    //     Build assetBundle without any special option.
    None = 0,
    //
    // 摘要:
    //     Don't compress the data when creating the asset bundle.
    UncompressedAssetBundle = 1,
    //
    // 摘要:
    //     Includes all dependencies.
    CollectDependencies = 2,
    //
    // 摘要:
    //     Forces inclusion of the entire asset.
    CompleteAssets = 4,
    //
    // 摘要:
    //     Do not include type information within the AssetBundle.
    DisableWriteTypeTree = 8,
    //
    // 摘要:
    //     Builds an asset bundle using a hash for the id of the object stored in the asset
    //     bundle.
    DeterministicAssetBundle = 16,
    //
    // 摘要:
    //     Force rebuild the assetBundles.
    ForceRebuildAssetBundle = 32,
    //
    // 摘要:
    //     Ignore the type tree changes when doing the incremental build check.
    IgnoreTypeTreeChanges = 64,
    //
    // 摘要:
    //     Append the hash to the assetBundle name.
    AppendHashToAssetBundleName = 128,
    //
    // 摘要:
    //     Use chunk-based LZ4 compression when creating the AssetBundle.
    ChunkBasedCompression = 256,
    //
    // 摘要:
    //     Do not allow the build to succeed if any errors are reporting during it.
    StrictMode = 512,
    //
    // 摘要:
    //     Do a dry run build.
    DryRunBuild = 1024,
    //
    // 摘要:
    //     Disables Asset Bundle LoadAsset by file name.
    DisableLoadAssetByFileName = 4096,
    //
    // 摘要:
    //     Disables Asset Bundle LoadAsset by file name with extension.
    DisableLoadAssetByFileNameWithExtension = 8192,
    //
    // 摘要:
    //     Removes the Unity Version number in the Archive File & Serialized File headers
    //     during the build.
    AssetBundleStripUnityVersion = 32768,
    EnableProtection = 65536
}