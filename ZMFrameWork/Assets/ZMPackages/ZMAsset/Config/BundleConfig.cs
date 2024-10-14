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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BundleConfig  
{
    /// <summary>
    /// 所有AssetBundle的信息列表
    /// </summary>
    public List<BundleInfo> bundleInfoList;
}
[System.Serializable]
/// <summary>
/// AssetBundle信息
/// </summary>
public class BundleInfo
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public string path;
    /// <summary>
    /// Crc
    /// </summary>
    public uint crc;
    /// <summary>
    /// AssetBundle名称
    /// </summary>
    public string bundleName;
    /// <summary>
    /// 资源名字
    /// </summary>
    public string assetName;
    /// <summary>
    /// AB模块
    /// </summary>
    public string bundleModule;
    /// <summary>
    /// 是否寻址资源
    /// </summary>
    public bool isAddressableAsset;
    /// <summary>
    /// 依赖项
    /// </summary>
    public List<string> bundleDependce;
}
/// <summary>
/// 内嵌的AssetBundle的信息
/// </summary>
public class BuiltinBundleInfo
{
    public string fileName;

    public string md5;//校验本地以解压文件是否与包内文件一致，如果不一致，说明本地文件被篡改，
                      //我们需要进行重新解压（需要进行校验的前提是 当前解压的模块没有开启热更）

    public float size;//文件大小 用来计算文件解压进度显示
}