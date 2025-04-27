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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Plastic.Antlr3.Runtime.Misc;

public class ScriptDisplayWindow : EditorWindow
{
    private string scriptContent;
    private string filePath;
    private string mFileName;
    private Vector2 scroll = new Vector2();
    /// <summary>
    /// 显示代码展示窗口
    /// </summary>
    public static void ShowWindow(string content, string filePath, Dictionary<string, string> _insertDic = null,List<EditorObjectData> fieldList=null)
    {
        //创建代码展示窗口
        ScriptDisplayWindow window = (ScriptDisplayWindow)GetWindowWithRect(typeof(ScriptDisplayWindow), new Rect(100, 50, 800, 700), true, "Window生成界面");
        window.scriptContent = content;
        window.filePath = filePath;
        window.mFileName = Path.GetFileName(filePath);
        //处理代码新增
        string originScript = string.Empty;
        bool isInsterSuccess = false;
        
        if (File.Exists(window.filePath) && (_insertDic!=null || fieldList!=null))
        {
            originScript = File.ReadAllText(window.filePath);
            
            if (string.IsNullOrEmpty(originScript) == false)
            {
                if (fieldList!=null)
                {
                    //插入字段(生成item脚本时使用)
                    foreach (var item in fieldList)
                    {
                        if (!originScript.Contains($"{item.fieldName}{item.fieldType}"))
                        {
                            string insterArrayType = item.dataList!=null?"[]":"";
                            string insterArray = item.dataList!=null?"Array":"";
                            //插入新增的数据
                            originScript = window.scriptContent = originScript.Insert(window.GetInsertFieldIndex(originScript)
                                , $"public { item.fieldType }{insterArrayType} {item.fieldName}{item.fieldType}{insterArray};\n\n\t\t");
                            isInsterSuccess = true;
                        }
                    }
                }
                if (_insertDic != null)
                {
                    //插入方法
                    foreach (var item in _insertDic)
                    {
                        if (!originScript.Contains(item.Key))
                        {
                            int insterIndex = window.GetInsertMethodIndex(originScript);
                            //插入新增的数据
                            originScript = window.scriptContent = originScript.Insert(insterIndex,"\n"+ item.Value+"\n\t\t");
                            isInsterSuccess = true;
                        }
                    }
                }


                if (fieldList!=null)
                {
                 
                    //插入事件(生成item脚本时使用)
                    foreach (var item in fieldList)
                    {  
                        string field = $"{item.fieldName}{item.fieldType}";
                        string type = item.fieldType;
                        string methodName = "On" + item.fieldName;
                        string suffix = "";
                        StringBuilder sb=new StringBuilder();
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
                        else
                        {
                            continue;
                        }
                        if (!originScript.Contains($"AddListener({methodName}{suffix})"))
                        {
                            sb.Insert(0,"//按钮事件自动注册绑定\n");
                            originScript = window.scriptContent = originScript.Replace("//按钮事件自动注册绑定", $"{sb.ToString()}");
                            isInsterSuccess = true;
                        }
                    }
                }
            }
            
            if (isInsterSuccess == false)
            {
                window.scriptContent = originScript;
            }
        }

        originScript = null;
        _insertDic = null;
        window.Show();
    }
    public void OnGUI()
    {
        //绘制ScroView
        scroll = EditorGUILayout.BeginScrollView(scroll,GUILayout.Height(600),GUILayout.Width(800));
        EditorGUILayout.TextArea(scriptContent);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        //绘制脚本生成路径
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea("脚本生成路径："+filePath);
        if (GUILayout.Button("选择路径",GUILayout.Width(80)))
        {
            filePath= EditorUtility.OpenFolderPanel("脚本生成路径", filePath, "ZMUI")+"/"+mFileName;
            EditorPrefs.SetString("GeneratorClassPath", filePath);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        //绘制按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成脚本",GUILayout.Height(30)))
        {
            //按钮事件
            ButtonClick();
        }
        EditorGUILayout.EndHorizontal();

    }
    public void ButtonClick()
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        
        StreamWriter writer = File.CreateText(filePath);
        writer.Write(scriptContent);
        writer.Close();
        writer.Dispose();
        scriptContent = string.Empty;
        Debug.Log("Create Code finish! Cs path:" + filePath);
        AssetDatabase.Refresh();
        if (EditorUtility.DisplayDialog("自动化工具", "生成脚本成功！", "确定"))
        {
            Close();
        } 
     }
    /// <summary>
    /// 获取插入代码的下标
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public int GetInsertMethodIndex(string content)
    {
        //找到UI事件组件下面的第一个public 所在的位置 进行插入
        Regex regex = new Regex("UI组件事件");
        Match match = regex.Match(content);
        return match.Index+6;
    }
    public int GetInsertFieldIndex(string content)
    {
        //找到UI事件组件下面的第一个public 所在的位置 进行插入
        Regex regex = new Regex("自定义字段");
        Match match = regex.Match(content);
        Regex regex1 = new Regex("public");
        MatchCollection matchColltion = regex1.Matches(content);

        for (int i = 0; i < matchColltion.Count; i++)
        {
            if (matchColltion[i].Index > match.Index)
            {
                return matchColltion[i].Index;
            }
        }
        return -1;
    }
     
}
