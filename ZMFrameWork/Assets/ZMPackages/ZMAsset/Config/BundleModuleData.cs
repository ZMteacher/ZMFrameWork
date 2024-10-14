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
using UnityEngine;

[System.Serializable]
public class BundleModuleData  
{
    //AssetBundle模块id
    public long bundleid;
    //模块名称
    public string moduleName;
    //是否寻址资源
    public bool isAddressableAsset;
    //是否打包
    public bool isBuild;

    //上一次点击按钮的时间
    public float lastClickBtnTime;


 
    public string[] prefabPathArr ;


    public string[] rootFolderPathArr;

    public BundleFileInfo[] signFolderPathArr;
}
[System.Serializable]
public class BundleFileInfo
{

    [HideLabel]
    public string abName="AB Name";

    [HideLabel]
    [FolderPath]
    public string bundlePath="BundlePath...";
}