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
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName ="AssetBundle",fileName = "BuildBundleConfigura",order =4)]
public class BuildBundleConfigura : ScriptableObject
{
    public static BuildBundleConfigura _instance;

    public static BuildBundleConfigura Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<BuildBundleConfigura>("Assets/" +BundleSettings.Instance.ZMAssetRootPath+"/Config/BuildBundleConfigura.asset");
            }
            return _instance;
        } 
    }

    /// <summary>
    /// 模块资源配置
    /// </summary>
    [SerializeField]
    public List<BundleModuleData> AssetBundleConfig = new List<BundleModuleData>();

    /// <summary>
    /// 根据模块名称获取模块数据
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public BundleModuleData GetBundleDataByName(string moduleName)
    {
        foreach (var item in AssetBundleConfig)
        {
            if (string.Equals(item.moduleName,moduleName))
            {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 通过模块名称移除模块资源
    /// </summary>
    /// <param name="moduleName"></param>
    public void RemoveModuleByName(string moduleName)
    {
        for (int i = 0; i < AssetBundleConfig.Count; i++)
        {
            if (AssetBundleConfig[i].moduleName==moduleName)
            {
                AssetBundleConfig.Remove(AssetBundleConfig[i]);
                break;
            }
        }

    }
    /// <summary>
    /// 储存新的模块资源
    /// </summary>
    /// <param name="moduleData"></param>
    public void SaveModuleData(BundleModuleData moduleData)
    {
        if (AssetBundleConfig.Contains(moduleData))
        {
            for (int i = 0; i < AssetBundleConfig.Count; i++)
            {
                if (AssetBundleConfig[i]==moduleData)
                {
                    AssetBundleConfig[i] = moduleData;
                    break;
                }
            }
        }
        else
        {
            AssetBundleConfig.Add(moduleData);
        }
     
        Save();
    }
    public void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
