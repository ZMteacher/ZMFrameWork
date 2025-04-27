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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorBindItemsComponentTool : Editor
{
    public static List<EditorObjectData> objDataList; //查找对象的数据
    static readonly Dictionary<string, string> _mMethodDic = new Dictionary<string, string>();

    [MenuItem("GameObject/生成Item脚本(Shift+I) #I", false, 0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject; //获取到当前选择的物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }

        objDataList = new List<EditorObjectData>();

        //设置脚本生成路径
        if (!Directory.Exists(UISetting.Instance.ItemScriptsGeneratorPath))
        {
            Directory.CreateDirectory(UISetting.Instance.ItemScriptsGeneratorPath);
        }

        string scriptName = obj.name.Replace("#", "");
        //解析窗口组件数据
        if (UISetting.Instance.ParseType == ParseType.Tag)
            AnalysisComponentDataTool.AnalysisWindowDataByTag(ref objDataList, obj.transform, scriptName);
        else
            AnalysisComponentDataTool.AnalysisWindowNodeData(ref objDataList, obj.transform, scriptName);


        //储存字段名称
        string datalistJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, datalistJson);
        //生成CS脚本
        string scriptContent = GenerateScripts(scriptName);
        string scriptFilePath = $"{UISetting.Instance.ItemScriptsGeneratorPath}/{scriptName}.cs";

        Debug.Log("Script Content:\n" + scriptContent);

        ScriptDisplayWindow.ShowWindow(scriptContent, scriptFilePath, _mMethodDic, objDataList);

        EditorPrefs.SetString("itemGeneratorClassPath", scriptFilePath);
    }

    public static string GenerateScripts(string name)
    {
        _mMethodDic.Clear();

        StringBuilder sb = new StringBuilder();
        string nameSpaceName = "ZM.UI";
        //添加引用
        sb.AppendLine("/*---------------------------------");
        sb.AppendLine(" *Title:UI自动化组件生成代码生成工具");
        sb.AppendLine(" *Author:铸梦");
        sb.AppendLine(" *Date:" + System.DateTime.Now);
        sb.AppendLine(" *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可");
        sb.AppendLine(" *注意:以下文件是自动生成的，再次生成后会以代码追加的形式新增,若手动修改后,尽量避免自动生成");
        sb.AppendLine("---------------------------------*/");
        foreach (string nameSpace in UISetting.Instance.UsingNameSpaceArr)
        {
            sb.AppendLine($"using {nameSpace};");
        }

        sb.AppendLine();

        //生成命名空间
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"\tpublic class {name}:MonoBehaviour");
        sb.AppendLine("\t{");

        //Scripts Field 
        sb.AppendLine($"\t\t#region 自定义字段");

        //根据字段数据列表 声明字段
        foreach (var item in objDataList)
        {
            if (item.dataList != null)
            {
                sb.AppendLine($"\t\tpublic   {item.fieldType}[]    {item.fieldName}{item.fieldType}Array;\n");
            }
            else
            {
                sb.AppendLine("\t\tpublic   " + item.fieldType + "  " + item.fieldName + item.fieldType + ";\n");
            }
        }

        sb.AppendLine($"\t\t#endregion");

        sb.AppendLine($"\n");

        sb.AppendLine($"\t\t#region 生命周期");

        //声明初始化组件接口
        sb.AppendLine($"\t\t//脚本初始化接口 (为保证生命周期的执行顺序，请在View层调用该接口确保需要初始化的数据正常执行)");
        sb.AppendLine("\t\tpublic void OnInitialize()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\t//按钮事件自动注册绑定");
        foreach (var item in objDataList)
        {
            string field = $"{item.fieldName}{item.fieldType}";
            string type = item.fieldType;
            string methodName = "On" + item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "ButtonClick";
                sb.AppendLine($"\t\t\t{field}.onClick.AddListener({methodName}{suffix});");
            }
            else if (type.Contains("InputField"))
            {
                suffix = "InputChange";
                sb.AppendLine($"\t\t\t{field}.onValueChanged.AddListener({methodName}{suffix});");
                suffix = "InputEnd";
                sb.AppendLine($"\t\t\t{field}.onEndEdit.AddListener({methodName}{suffix});");
            }
            else if (type.Contains("Toggle"))
            {
                suffix = "ToggleChange";
                sb.AppendLine($"\t\t\t{field}.onValueChanged.AddListener({methodName}{suffix});");
            }
        }

        sb.AppendLine("\t\t}");

        sb.AppendLine($"\t\t//物体设置数据接口 (请自定以你的参数，方便外部调用传参)");
        sb.AppendLine("\t\tpublic  void SetItemData()");
        sb.AppendLine("\t\t{");

        sb.AppendLine("\t\t}");

        //OnDestroy
        sb.AppendLine($"\t\t//物体销毁时执行 (为保证生命周期的执行顺序，请在View层调用该接口确保需要释放时的接口正常调用)");
        sb.AppendLine("\t\tpublic  void OnDispose()");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t}");

        sb.AppendLine($"\t\t#endregion");
        sb.AppendLine($"\n");
        sb.AppendLine($"\t\t#region UI组件事件");
        //生成UI事件绑定代码
        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string methodName = "On" + item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "ButtonClick";
                CreateMethod(ref sb, methodName + suffix);
            }
            else if (type.Contains("InputField"))
            {
                suffix = "InputChange";
                CreateMethod(ref sb, methodName + suffix, "string text");
                suffix = "InputEnd";
                CreateMethod(ref sb, methodName + suffix, "string text");
            }
            else if (type.Contains("Toggle"))
            {
                suffix = "ToggleChange";
                CreateMethod(ref sb, methodName + suffix, "bool state");
            }
        }

        sb.AppendLine($"\t\t #endregion");
        sb.AppendLine($"\n");
        sb.AppendLine("\t}");

        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }


    /// <summary>
    /// 生成UI事件方法
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="methodDic"></param>
    /// <param name="modthName"></param>
    /// <param name="param"></param>
    public static void CreateMethod(ref StringBuilder sb, string methodName, string param = "")
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"\t\tprivate void {methodName}({param})");
        builder.AppendLine("\t\t{");
        builder.AppendLine("\t\t");
        builder.AppendLine("\t\t}");

        sb.AppendLine(builder.ToString());

        _mMethodDic.Add(methodName, builder.ToString());
    }

    /// <summary>
    /// 编译完成系统自动调用
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void AddComponentToItem()
    {
        //如果当前不是生成数据脚本的回调，就不处理
        string scriptPath = EditorPrefs.GetString("itemGeneratorClassPath").Replace(Application.dataPath, "Assets/");
        if (string.IsNullOrEmpty(scriptPath))
        {
            return;
        }

        //1.通过反射的方式，从程序集中找到这个脚本，把它挂在到当前的物体上
        //获取所有的程序集
        System.Type targetScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath)?.GetClass();
        if (targetScript == null)
        {
            Debug.Log($"Failed to load script! Path:{scriptPath}");
            return;
        }

        //获取要挂载的那个物体
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
            return;

        //先获取现窗口上有没有挂载该数据组件，如果没挂载在进行挂载
        Component compt = selectedObject.GetComponent(targetScript);
        if (compt == null)
        {
            compt = selectedObject.AddComponent(targetScript);
        }

        //2.通过反射的方式，遍历数据列表 找到对应的字段，赋值
        //获取对象数据列表
        string datalistJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
        List<EditorObjectData> objDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(datalistJson);
        //获取脚本所有字段
        FieldInfo[] fieldInfoList = targetScript.GetFields();

        foreach (var item in fieldInfoList)
        {
            foreach (var objData in objDataList)
            {
                if (item.Name == $"{objData.fieldName}{objData.fieldType}"||item.Name == $"{objData.fieldName}{objData.fieldType}Array")
                {
                    //根据Insid找到对应的对象
                    GameObject uiObject = EditorUtility.InstanceIDToObject(objData.insID) as GameObject;
                    //设置该字段所对应的对象
                    if (objData.dataList == null)
                    {
                        //设置该字段所对应的对象
                        if (string.Equals(objData.fieldType, "GameObject"))
                        {
                            item.SetValue(compt, uiObject);
                        }
                        else
                        {
                            Debug.Log("targetComponent:"+uiObject.GetComponent(objData.fieldType) +" type:"+objData.fieldType);
                            item.SetValue(compt, uiObject.GetComponent(objData.fieldType));
                        }
                    }
                    else
                    {
                        if (objData.fieldType.Contains("GameObject"))
                        {
                            GameObject[] newArray = new GameObject[objData.dataList.Count];
                            for (int i = 0; i < objData.dataList.Count; i++)
                            {
                                newArray[i] = EditorUtility.InstanceIDToObject(objData.dataList[i].insID) as GameObject;
                            }

                            item.SetValue(compt, newArray);
                        }
                        else
                        {
                            // 获取数组类型
                            Type arrayType = item.FieldType;
                            // 获取数组元素类型
                            Type elementType = arrayType.GetElementType();
                            //获取该节点下的所有的物体
                            Component[] components = uiObject.GetComponentsInChildren(elementType);
                            // 创建目标数组
                            Array targetArray = Array.CreateInstance(elementType, components.Length);

                            // 将组件赋值给目标数组
                            for (int i = 0; i < components.Length; i++)
                            {
                                if (components[i] != null && elementType.IsAssignableFrom(components[i].GetType()))
                                {
                                    targetArray.SetValue(components[i], i);
                                }
                                else
                                {
                                    Debug.LogError($"Element at index {i} is not of type {elementType.Name}!");
                                }
                            }

                            // 设置字段的值
                            item.SetValue(compt, targetArray);
                        }
                    }

                    break;
                }
            }
        }

        // PrefabUtility.ApplyPrefabInstance(selectedObject, InteractionMode.AutomatedAction);
        //自动保存预制体
        EditorPrefs.DeleteKey("itemGeneratorClassPath");
        PlayerPrefs.DeleteKey(GeneratorConfig.OBJDATALIST_KEY);
        UnityEditor.EditorUtility.SetDirty(compt); // 标记对象为“脏”以刷新
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(compt);
    }
}