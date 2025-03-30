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

using System.Collections.Generic;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CompileAndCopyHyBridCLR  
{
    [MenuItem("HybridCLR/Compile And Copy/HallWorld")]
    public static void CompileAndCopy()
    {
       CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.development);
       CopyHotFixDllToTargetDir(Path.Combine(Application.dataPath,"GameData/Hall/HotFixDll"),new List<string>());
       CopyAOTMetadataDllToTargetDir(Path.Combine(Application.dataPath,"GameData/Hall/HotFixDll"));
    }
    
    [MenuItem("HybridCLR/Compile And Copy/SKWorld")]
    public static void CompileSkWorldAndCopy()
    {
        CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.development);
        CopyHotFixDllToTargetDir(Path.Combine(Application.dataPath,"GameData/ShuangKou/HotFixDll"),new List<string>{"ZM.GC","ZM.UI","HallWorld","ZM.WZWorld"});
        CopyAOTMetadataDllToTargetDir(Path.Combine(Application.dataPath,"GameData/ShuangKou/HotFixDll"));
    }
    
    [MenuItem("HybridCLR/Compile And Copy/WZWorld")]
    public static void CompileWZWorldAndCopy()
    {
        CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.development);
        CopyHotFixDllToTargetDir(Path.Combine(Application.dataPath,"GameData/WuZhang/HotFixDll"),new List<string>{"ZM.GC","ZM.UI","HallWorld","ZM.SKWorld"});
        CopyAOTMetadataDllToTargetDir(Path.Combine(Application.dataPath,"GameData/WuZhang/HotFixDll"));
    }
    /// <summary>
    /// 拷贝热更Dll到目标文件夹
    /// </summary>
    /// <param name="targetDir"></param>
    public static void CopyHotFixDllToTargetDir(string targetDir,List<string> filterDll)
    {
        string dllOutPutDir = Path.Combine(Application.dataPath.Replace("Assets",""),HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir);
        Debug.Log("dllOutPutDir:"+dllOutPutDir);
        foreach (var item in HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions)
        {
            if(filterDll.Contains(item.name)) continue;
             Debug.Log("CopyTo: " + Path.Combine(targetDir,$"{item.name}.dll.bytes"));
             File.Copy(Path.Combine(dllOutPutDir,EditorUserBuildSettings.activeBuildTarget.ToString(),$"{item.name}.dll"), 
                 Path.Combine(targetDir,$"{item.name}.dll.bytes"), true);
        }
        AssetDatabase.Refresh();
        Debug.Log("Copy Finished!");
    }
    /// <summary>
    /// 拷贝补元数据到目标文件夹
    /// </summary>
    /// <param name="targetDir"></param>
    public static void CopyAOTMetadataDllToTargetDir(string targetDir)
    {
        string dllOutPutDir = Path.Combine(Application.dataPath.Replace("Assets",""),HybridCLRSettings.Instance.strippedAOTDllOutputRootDir);
        Debug.Log("dllOutPutDir:"+dllOutPutDir);
        foreach (var aotPatchName in HybridCLRSettings.Instance.patchAOTAssemblies)
        {
            Debug.Log("CopyTo: " + Path.Combine(targetDir,$"{aotPatchName}.dll.bytes"));
            File.Copy(Path.Combine(dllOutPutDir,EditorUserBuildSettings.activeBuildTarget.ToString(),$"{aotPatchName}.dll"),
                Path.Combine(targetDir,$"{aotPatchName}.dll.bytes"), true);
        }
        AssetDatabase.Refresh();
        Debug.Log("Copy Finished!");
    }
}
