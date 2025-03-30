/*----------------------------------------------------------------------------
* Title: #Title#
*
* Author: 铸梦
*
* Date: #CreateTime#
*
* Description:
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;
using HybridCLR;
using ZM.ZMAsset;


public class AOTMain : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("# AOT Main Awake");
        Debug.Log(Application.persistentDataPath);
        //初始化游戏热更框架
        ZMAsset.InitFrameWork();
        //热更大厅资源
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, OnHotAssetFinish);
        
    }
    // 在第一帧更新之前调用 Start
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void OnHotAssetFinish()
    {
        Debug.Log("# AOT OnHotAssetFinish...");
        Debug.Log($"# AOT OnHotAssetPath:{Application.persistentDataPath}");
        Assembly hotUpdateAss = null;
        // Editor环境下，已经默认加载了所有程序集，你再次加载就会出现重复加载，不需要加载，重复加载反而会出问题 例如GetComponent<?>()返回值报空问题。
#if  UNITY_EDITOR 
        //Editor下无需加载，直接查找获得HallWorld程序集
        hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HallWorld");
#else
        //加载框架热更程序集
        Assembly.Load(ZMAsset.LoadTextAsset(AssetsPathConfig.HALL_HOTFIXDLL_PATH + "ZM.UI.dll.bytes").bytes);
        Assembly.Load(ZMAsset.LoadTextAsset(AssetsPathConfig.HALL_HOTFIXDLL_PATH + "ZM.GC.dll.bytes").bytes);
        //强制注册类型，确保跨程序集继承关系正确建立
        hotUpdateAss = Assembly.Load(ZMAsset.LoadTextAsset(AssetsPathConfig.HALL_HOTFIXDLL_PATH + "HallWorld.dll.bytes").bytes);
#endif
        //通过反射调用开始游戏接口
        Type type = hotUpdateAss.GetType("HotFixEntry");
        type.GetMethod("OnStatGame").Invoke(null, null);
    }
    
    
}
