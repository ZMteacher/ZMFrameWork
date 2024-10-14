using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GeneratorLogicCtrl
{

    [MenuItem("GameObject/生成业务逻辑层脚本(Shift+L) #L", false, 0)]
    public static void GeneratorLogicController()
    {
        var gameObject = Selection.activeObject;

        if (!gameObject)
        {
            Debug.LogWarning("需要选择 GameObject");
            return;
        }
 
        string logicName = gameObject.name.Replace("Window", "")+ "LogicCtrl";
        string viewStr = GeneratorLogicScrpts(logicName);
        GeneratorModuleWindow.ShowWindow(viewStr, logicName+".cs");
    }
    public static string GeneratorLogicScrpts(string scriptsName)
    {
        string nameSpaceName= "ZMGC.Hall";
        StringBuilder sb = new StringBuilder();
        //var writer = File.CreateText(scriptFile);
        sb.AppendLine("/*--------------------------------------------------------------------------------------");
        sb.AppendLine("* Title: 业务逻辑脚本自动生成工具");
        sb.AppendLine("* Author: 铸梦xy");
        sb.AppendLine("* Date:" + System.DateTime.Now);
        sb.AppendLine("* Description:业务逻辑层,主要负责游戏的业务逻辑处理");
        sb.AppendLine("* Modify:");
        sb.AppendLine("* 注意:以下文件为自动生成，强制再次生成将会覆盖");
        sb.AppendLine("----------------------------------------------------------------------------------------*/");


        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }


        sb.AppendLine($"\tpublic  class {scriptsName} : ILogicBehaviour");
        sb.AppendLine("\t{");
        sb.AppendLine("\t");

        sb.AppendLine("\t\t" + " public  void OnCreate()");
        sb.AppendLine("\t\t" + " {");
        sb.AppendLine("\t\t");
        sb.AppendLine("\t\t" + " }");
        sb.AppendLine("\t\t");

        //sb.AppendLine("\t\t" + " public  void OnUpdate()");
        //sb.AppendLine("\t\t" + " {");
        //sb.AppendLine("\t\t");
        //sb.AppendLine("\t\t" + " }");
        //sb.AppendLine("\t\t");

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
 