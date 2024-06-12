using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GeneratorDataCtrl
{

    [MenuItem("GameObject/生成数据层脚本(Shift+D) #D", false, 0)]
    public static void GeneratorDataController()
    {
        var gameObject = Selection.activeObject;

        if (!gameObject)
        {
            Debug.LogWarning("需要选择 GameObject");
            return;
        }
 
        string dataName = gameObject.name.Replace("Window", "") + "DataMgr";
        string viewStr = GeneratorDataScrpts(dataName);
        GeneratorModuleWindow.ShowWindow(viewStr, dataName + ".cs");
    }
    public static string GeneratorDataScrpts(string scriptsName)
    {
        string nameSpaceName= "ZMGC.Hall";
        StringBuilder sb = new StringBuilder();
        //var writer = File.CreateText(scriptFile);
        sb.AppendLine("/*--------------------------------------------------------------------------------------");
        sb.AppendLine("* Title: 数据脚本自动生成工具");
        sb.AppendLine("* Author: 铸梦xy");
        sb.AppendLine("* Date:" + System.DateTime.Now);
        sb.AppendLine("* Description:数据层,主要负责游戏数据的存储、更新和获取");
        sb.AppendLine("* Modify:");
        sb.AppendLine("* 注意:以下文件为自动生成，强制再次生成将会覆盖");
        sb.AppendLine("----------------------------------------------------------------------------------------*/");


        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }


        sb.AppendLine($"\tpublic  class {scriptsName} : IDataBehaviour");
        sb.AppendLine("\t{");
        sb.AppendLine("\t");

        sb.AppendLine("\t\t" + " public  void OnCreate()");
        sb.AppendLine("\t\t" + " {");
        sb.AppendLine("\t\t");
        sb.AppendLine("\t\t" + " }");
        sb.AppendLine("\t\t");

  

        sb.AppendLine("\t\t" + " public  void OnDestroy()");
        sb.AppendLine("\t\t" + " {");
        sb.AppendLine("\t\t");
        sb.AppendLine("\t\t" + " }");


        sb.AppendLine("\t");
        sb.AppendLine("\t}");

        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine("}");
        }
        return sb.ToString();

    }
}
 