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
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class GeneratorFindComponentTool : Editor
{

    public static Dictionary<int, string> objFindPathDic; //key 物体的insid，value 代表物体的查找路径
    public static List<EditorObjectData> objDataList;//查找对象的数据

    [MenuItem("GameObject/生成组件查找脚本(Shift+U) #U",false,0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;//获取到当前选择的物体
        if (obj==null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }
        objDataList = new List<EditorObjectData>();
        objFindPathDic = new Dictionary<int, string>();

        //设置脚本生成路径
        if (!Directory.Exists(UISetting.Instance.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(UISetting.Instance.FindComponentGeneratorPath);
        }
        ////解析窗口组件数据
        PresWindowNodeData(obj.transform,obj.name);
       
        //储存字段名称
        string datalistJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, datalistJson);
        //生成CS脚本
        string csContnet= CreateCS(obj.name);
        Debug.Log("CsConent:\n"+csContnet);
        string cspath = UISetting.Instance.FindComponentGeneratorPath + "/"+obj.name+"UIComponent.cs";
        ScriptDisplayWindow.ShowWindow(csContnet,cspath);
    }
    /// <summary>
    /// 解析窗口节点数据
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="WinName"></param>
    public static void PresWindowNodeData(Transform trans, string WinName)
    {
        foreach (Transform child in trans)
        {
            GameObject obj = child.gameObject;
            string name = obj.name;
        
            if (name.Contains("[") && name.Contains("]"))
            {
                // 解析字段名和类型
                int endBracketIndex = name.IndexOf("]");
                string fieldName = name.Substring(endBracketIndex + 1); // 获取字段昵称
                string fieldType = name.Substring(1, endBracketIndex - 1); // 获取字段类型

                objDataList.Add(new EditorObjectData 
                { 
                    fieldName = fieldName,
                    fieldType = fieldType,
                    insID = obj.GetInstanceID() 
                });

                // 计算查找路径（优化关键点）
                string objPath = GetObjectPath(obj.transform, WinName);
                objFindPathDic.Add(obj.GetInstanceID(), objPath);
            }
        
            PresWindowNodeData(child, WinName); // 递归处理子节点
        }
    }
    //获取对象路径
    private static string GetObjectPath(Transform objTransform, string winName)
    {
        StringBuilder pathBuilder = new StringBuilder(objTransform.name);
        Transform parent = objTransform.parent;

        while (parent != null && !string.Equals(parent.name, winName))
        {
            pathBuilder.Insert(0, parent.name + "/");
            parent = parent.parent;
        }

        return pathBuilder.ToString();
    }
    public static void ParseWindowDataByTag(Transform trans, string WinName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            string tagName = obj.tag;
            if (GeneratorConfig.TAGArr.Contains(tagName))
            {
                string fieldName = obj.name;//获取字段昵称
                string fieldType = tagName;//获取字段类型
                objDataList.Add(new EditorObjectData { fieldName = fieldName, fieldType = fieldType, insID = obj.GetInstanceID() });
            }
            ParseWindowDataByTag(trans.GetChild(i), WinName);
        }
    }
    public static string CreateCS(string name)
    {
        StringBuilder sb = new StringBuilder();
        string nameSpaceName = "ZM.UI";
        //添加引用
        sb.AppendLine("/*---------------------------------");
        sb.AppendLine(" *Title:UI自动化组件查找代码生成工具");
        sb.AppendLine(" *Author:铸梦");
        sb.AppendLine(" *Date:" + System.DateTime.Now);
        sb.AppendLine(" *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI组件查找脚本即可");
        sb.AppendLine(" *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成");
        sb.AppendLine("---------------------------------*/");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using ZM.UGUIPro;");
        sb.AppendLine();

        //生成命名空间
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }
        sb.AppendLine($"\tpublic class {name+"UIComponent"}");
        sb.AppendLine("\t{");

        //根据字段数据列表 声明字段
        foreach (var item in objDataList)
        {
            sb.AppendLine("\t\tpublic   "+item.fieldType +"  "+item.fieldName+item.fieldType+";\n");
        }

        //声明初始化组件接口
        sb.AppendLine("\t\tpublic  void InitComponent(WindowBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t     //组件查找");
        //根据查找路径字典 和字段数据列表生成组件查找代码
        foreach (var item in objFindPathDic)
        {
            EditorObjectData itemData = GetEditorObjectData(item.Key);
            string relFieldName = itemData.fieldName + itemData.fieldType;

            if (string.Equals("GameObject",itemData.fieldType))
            {
                sb.AppendLine($"\t\t     {relFieldName} =target.transform.Find(\"{item.Value}\").gameObject;");
            }
            else if (string.Equals("Transform",itemData.fieldType))
            {
                sb.AppendLine($"\t\t     {relFieldName} =target.transform.Find(\"{item.Value}\").transform;");
            }
            else
            {
                sb.AppendLine($"\t\t     {relFieldName} =target.transform.Find(\"{item.Value}\").GetComponent<{itemData.fieldType}>();");
            }
        }
        sb.AppendLine("\t");
        sb.AppendLine("\t");
        sb.AppendLine("\t\t     //组件事件绑定");
        //得到逻辑类 WindowBase => LoginWindow
        sb.AppendLine($"\t\t     {name} mWindow=({name})target;");

        //生成UI事件绑定代码
        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string methodName = item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "Click";
                sb.AppendLine($"\t\t     target.AddButtonClickListener({methodName}{type},mWindow.On{methodName}Button{suffix});");
            }
            if (type.Contains("InputField"))
            {
                sb.AppendLine($"\t\t     target.AddInputFieldListener({methodName}{type},mWindow.On{methodName}InputChange,mWindow.On{methodName}InputEnd);");
            }
            if (type.Contains("Toggle"))
            {
                suffix = "Change";
                sb.AppendLine($"\t\t     target.AddToggleClickListener({methodName}{type},mWindow.On{methodName}Toggle{suffix});");
            }
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine("}");
        }
        return sb.ToString();
    }

    public static EditorObjectData GetEditorObjectData(int insid)
    {
        foreach (var item in objDataList)
        {
            if (item.insID==insid)
            {
                return item;
            }
        }
        return null;
    }
}
public class EditorObjectData
{
    public int insID;
    public string fieldName;
    public string fieldType;
    public List<EditorObjectData> dataList;
}