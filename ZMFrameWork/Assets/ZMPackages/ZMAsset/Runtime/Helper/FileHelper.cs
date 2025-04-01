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
using System.IO;
using UnityEngine;

public class FileHelper
{
    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="folderPath"></param>
    public static void DeleteFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            string[] pathsArr = Directory.GetFiles(folderPath, "*");
            foreach (var path in pathsArr)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            Directory.Delete(folderPath);
        }
    }
    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    public static void WriteFile(string filePath,byte[] data)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        FileStream stream= File.Create(filePath);
        stream.Write(data,0,data.Length);
        stream.Dispose();
        stream.Close();
    }
    /// <summary>
    /// 异步写入文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    public static async void WriteFileAsync(string filePath,string data)
    {
        await File.WriteAllTextAsync(filePath, data);
    }
}
