/*----------------------------------------------------------------------------
* Title: ZMUIFrameWork 一款Mono分离式UI管理框架
*
* Author: 铸梦xy
*
* Date: 2024/09/01 14:15:58
*
* Description: 高性能、自动化、自定义生命周期工作管线是该框架的特点，该框架属于MVC中的View层架构。
* 设计简洁清晰、轻便小巧，可以对接至任意重中小型游戏项目中。
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* GitHub：https://github.com/ZMteacher?tab=repositories
----------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "WindowConfig", menuName = "WindowConfig", order = 0)]
public class WindowConfig : ScriptableObject
{
    public List<WindowData> windowDataList = new List<WindowData>();
    /// <summary>
    /// 生成窗口预制体加载路径
    /// </summary>
    public void GeneratorWindowConfig()
    {
        string[] windowRootArr = UISetting.Instance.WindowPrefabFolderPathArr;
        //检测预制体路径或名称没有改变，如果没有就不需要生成配置
        bool needUpdate = false;
        foreach (var item in windowRootArr)
        {
            string[] filePathArr =  Directory.GetFiles(Application.dataPath.Replace("Assets","") + item, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta")) continue;
                WindowData windowData= GetWindowData(Path.GetFileNameWithoutExtension(path), false);
                  
                string windowPath = windowData == null?string.Empty: windowData.path;
                //路径不存在或路径不一致
                if (string.IsNullOrEmpty(windowPath)|| (!string.IsNullOrEmpty(windowPath)&& windowPath.GetHashCode() != path.GetHashCode()))
                {
                    needUpdate = true;
                    break;
                }
            }
        }
        if (!needUpdate)
        {
            Debug.Log("预制体个数没有发生改变，不生成窗口配置");
            return;
        }

        windowDataList.Clear();
        foreach (var item in windowRootArr)
        {
            //获取预制体文件夹读取路径
            string floder = Application.dataPath.Replace("Assets", "") + item;
            //获取文件夹下的所有Prefab文件
            string[] filePathArr = Directory.GetFiles(floder,"*.prefab",SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                //获取预制体名字
                string fileName = Path.GetFileNameWithoutExtension(path);
                //计算文件读取路径 
                string filePath = item + "/" + fileName;
                WindowData data = new WindowData { name = fileName, path = filePath };
                windowDataList.Add(data);
            }
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
#endif
    }

    /// <summary>
    /// 添加窗口元数据 (在多模块资源+独立代码热更程序集时使用) 主要作用是添加热更窗口数据至AOT或热更程序集内
    /// </summary>
    public void AddAOTWindowMetadata(WindowConfig windowConfig)
    {
        foreach (var item in windowConfig.windowDataList)
        {
            if (GetWindowData(item.name, false)==null)
            {
                windowDataList.Add(item);
                Debug.Log("补充窗口元数据:"+item.name);
            }
        }
    }
    

    /// <summary>
    /// 获取窗口数据
    /// </summary>
    /// <param name="wndName">窗口名称</param>
    /// <param name="log">是否打印窗口不存在日志.</param>
    /// <returns></returns>
    public WindowData GetWindowData(string wndName,bool log=true)
    {
        foreach (var item in windowDataList)
        {
            if (string.Equals(item.name,wndName))
            {
                return item;
            }
        }
        if (log)
            Debug.LogError(wndName+"不存在在配置文件中，请检查预制体存放位置，或配置文件");
        return null;
    }
}
[System.Serializable]
public class WindowData
{
    public string name;
    public string path;
}