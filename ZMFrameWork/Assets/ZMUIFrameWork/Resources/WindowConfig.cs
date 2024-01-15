using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[CreateAssetMenu(fileName = "WindowConfig", menuName = "WindowConfig", order = 0)]
public class WindowConfig : ScriptableObject
{
    private string[] windowRootArr ;
    public List<WindowData> windowDataList = new List<WindowData>();

    public void GeneratorWindowConfig()
    {
        windowRootArr = new string[] { "/GameData/Battle/Prefabs/Window", "/GameData/Hall/Prefabs/Window" , "/GameData/ShuangKou/Prefabs/Window" };
        //检测预制体有没有新增，如果没有就不需要生成配置
        int count = 0;
        foreach (var item in windowRootArr)
        {
            string[] filePathArr =  Directory.GetFiles(Application.dataPath + item, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                count += 1;
            }
        }
        if (count==windowDataList.Count)
        {
            Debug.Log("预制体个数没有发生改变，不生成窗口配置");
            return;
        }

        windowDataList.Clear();
        foreach (var item in windowRootArr)
        {
            //获取预制体文件夹读取路径
            string floder = Application.dataPath + item;
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
                string filePath = "Assets"+item + "/" + fileName;
                WindowData data = new WindowData { name = fileName, path = filePath };
                windowDataList.Add(data);
            }
        }
    }
    public string GetWindowPath(string wndName)
    {
        foreach (var item in windowDataList)
        {
            if (string.Equals(item.name,wndName))
            {
                return item.path;
            }
        }
        Debug.LogError(wndName+"不存在在配置文件中，请检查预制体存放位置，或配置文件");
        return "";
    }
}
[System.Serializable]
public class WindowData
{
    public string name;
    public string path;
}