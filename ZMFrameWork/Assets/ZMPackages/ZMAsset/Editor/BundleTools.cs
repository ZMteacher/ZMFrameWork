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
using UnityEditor;
using System.IO;

public class BundleTools 
{
    private static string mBundleModuleEnumFilePath = Application.dataPath + "/"+BundleSettings.Instance.ZMAssetRootPath +"/Config/BundleModuleEnum.cs";

    [MenuItem("ZMFrame/GeneratorModuleEnum",false,1)]
    public static void GenerateBundleModuleEnum()
    {
        string namespaceName = "ZM.ZMAsset";
        string classname = "BundleModuleEnum";

        if (File.Exists(mBundleModuleEnumFilePath))
        {
            File.Delete(mBundleModuleEnumFilePath);
            AssetDatabase.Refresh();
        }

        var writer = File.CreateText(mBundleModuleEnumFilePath);
        writer.WriteLine("/* ----------------------------------------------");
        writer.WriteLine("/* Title:AssetBundle模块类");
        writer.WriteLine("/* Author:铸梦xy");
        writer.WriteLine("/* Data:" + System.DateTime.Now);
        writer.WriteLine("/* Description:  Represents each module which is used to download an load");
        writer.WriteLine("/* Modify:");
        writer.WriteLine("----------------------------------------------*/");

        writer.WriteLine($"namespace {namespaceName}");
        writer.WriteLine("{");
        List<BundleModuleData> moduleList = BuildBundleConfigura.Instance.AssetBundleConfig;

        if (moduleList == null || moduleList.Count <= 0)
        {
            return;
        }
        writer.WriteLine("\t" + $"public enum {classname}");
        writer.WriteLine("\t" + "{");
        writer.WriteLine("\t\tNone,");
        for (int i = 0; i < moduleList.Count; i++)
        {
            writer.WriteLine("\t\t" + moduleList[i].moduleName + ",");
        }

        writer.WriteLine("\t" + "}");

        writer.WriteLine("}");

        writer.Close();

        AssetDatabase.Refresh();

    }
}
