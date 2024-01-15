using System.Collections;
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
        if (!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
        }
        //解析窗口组件数据
        PresWindowNodeData(obj.transform,obj.name);
        //储存字段名称
        string datalistJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, datalistJson);
        //生成CS脚本
        string csContnet= CreateCS(obj.name);
        Debug.Log("CsConent:\n"+csContnet);
        string cspath = GeneratorConfig.FindComponentGeneratorPath + "/"+obj.name+"UIComponent.cs";
        UIWindowEditor.ShowWindow(csContnet,cspath);
        //生成脚本文件
        //if (File.Exists(cspath))
        //{
        //    File.Delete(cspath);
        //}
        //StreamWriter writer = File.CreateText(cspath);
        //writer.Write(csContnet);
        //writer.Close();
        //AssetDatabase.Refresh();
        //Debug.Log("cspath:" + cspath);
        //foreach (var item in objDataList)
        //{
        //    Debug.Log("fieldName: " + item.fieldName);
        //    Debug.Log("fieldType: " + item.fieldType);
        //}
        //foreach (var item in objFindPathDic)
        //{
        //    Debug.Log("查找路径："+item.Value);
        //}
    }
    /// <summary>
    /// 解析窗口节点数据
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="WinName"></param>
    public static void PresWindowNodeData(Transform trans, string WinName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            string name = obj.name;
            if (name.Contains("[")&&name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1;
                string fieldName = name.Substring(index,name.Length-index);//获取字段昵称
                string fieldType = name.Substring(1,index-2);//获取字段类型

                objDataList.Add(new EditorObjectData { fieldName=fieldName,fieldType= fieldType,insID=obj.GetInstanceID() });

                //计算该节点的查找路径
                string objPath = name;//UIContent/[Button]Close
                bool isFindOver = false;
                Transform parent = obj.transform;
                for (int k = 0; k < 20; k++)
                {
                    for (int j = 0; j <=k; j++)
                    {
                        if (k==j)
                        {
                            parent = parent.parent;
                            //如果父节点是当前窗口，说明查找已经结束
                            if (string.Equals(parent.name,WinName))
                            {
                                isFindOver = true;
                                break;
                            }
                            else
                            {
                                objPath = objPath.Insert(0,parent.name+"/");
                            }
                        }
                    }
                    if (isFindOver)
                    {
                        break;
                    }
                }
                objFindPathDic.Add(obj.GetInstanceID(),objPath);
            }
            PresWindowNodeData(trans.GetChild(i),WinName);
        }
    }

    public static string CreateCS(string name)
    {
        StringBuilder sb = new StringBuilder();
        string nameSpaceName = "ZMUIFrameWork";
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
   
}