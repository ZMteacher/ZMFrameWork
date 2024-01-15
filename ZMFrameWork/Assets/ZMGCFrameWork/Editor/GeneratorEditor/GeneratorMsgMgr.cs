using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GeneratorMsgCtrl
{

    [MenuItem("GameObject/生成网络层脚本(Shift+N) #N", false, 0)]
    public static void GeneratorMsgController()
    {
        var gameObject = Selection.activeObject;

        if (!gameObject)
        {
            Debug.LogWarning("需要选择 GameObject");
            return;
        }
 
        string msgName = gameObject.name.Replace("Window", "")+ "MsgMgr";
        string viewStr = GeneratorMsgScrpts(msgName);
        GeneratorModuleWindow.ShowWindow(viewStr, msgName + ".cs");
    }
    public static string GeneratorMsgScrpts(string scriptsName)
    {
        string nameSpaceName="ZMGC.Hall";
        StringBuilder sb = new StringBuilder();
        //var writer = File.CreateText(scriptFile);
        sb.AppendLine("/*--------------------------------------------------------------------------------------");
        sb.AppendLine("* Title: 网络消息层脚本自动生成工具");
        sb.AppendLine("* Author: 铸梦xy");
        sb.AppendLine("* Date:" + System.DateTime.Now);
        sb.AppendLine("* Description:网络消息层,主要负责游戏网络消息的收发");
        sb.AppendLine("* Modify:");
        sb.AppendLine("* 注意:以下文件为自动生成，强制再次生成将会覆盖");
        sb.AppendLine("----------------------------------------------------------------------------------------*/");


        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }


        sb.AppendLine($"\tpublic  class {scriptsName} : IMsgBehaviour");
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
 