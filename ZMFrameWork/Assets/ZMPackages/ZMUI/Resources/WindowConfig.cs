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
using UnityEngine;
[CreateAssetMenu(fileName = "WindowConfig", menuName = "WindowConfig", order = 0)]
public class WindowConfig : ScriptableObject
{
    public List<WindowData> windowDataList = new List<WindowData>();

    public void GeneratorWindowConfig()
    {
        string[] windowRootArr = UISetting.Instance.WindowPrefabFolderPathArr;
        //检测预制体路径或名称没有改变，如果没有就不需要生成配置
        bool needUpdate = false;
        foreach (var item in windowRootArr)
        {
            string[] filePathArr = Directory.GetFiles(Application.dataPath.Replace("Assets", "") + item, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta")) continue;
                WindowData windowData = GetWindowData(Path.GetFileNameWithoutExtension(path), false);

                string windowPath = windowData == null ? string.Empty : windowData.path;
                //路径不存在或路径不一致
                if (string.IsNullOrEmpty(windowPath) || (!string.IsNullOrEmpty(windowPath) && windowPath.GetHashCode() != path.GetHashCode()))
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
            string[] filePathArr = Directory.GetFiles(floder, "*.prefab", SearchOption.AllDirectories);
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
    }
    public WindowData GetWindowData(string wndName, bool log = true)
    {
        foreach (var item in windowDataList)
        {
            if (string.Equals(item.name, wndName))
            {
                return item;
            }
        }
        Debug.LogError(wndName + "不存在在配置文件中，请检查预制体存放位置，或配置文件");
        return null;
    }
}
[System.Serializable]
public class WindowData
{
    public string name;
    public string path;
}