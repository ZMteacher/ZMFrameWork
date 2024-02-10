using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class UIWindowEditor : EditorWindow
{
    private string scriptContent;
    private string filePath;
    private Vector2 scroll = new Vector2();
    private Dictionary<string, string> mMethodDic = new Dictionary<string, string>();
    /// <summary>
    /// 显示代码展示窗口
    /// </summary>
    public static void ShowWindow(string content, string filePath, Dictionary<string, string> insterDic = null)
    {
        //创建代码展示窗口
        UIWindowEditor window = (UIWindowEditor)GetWindowWithRect(typeof(UIWindowEditor), new Rect(100, 50, 800, 700), true, "Window生成界面");
        window.scriptContent = content;
        window.filePath = filePath;
        //处理代码新增
        window.mMethodDic = insterDic;
        string originScript = string.Empty;
        bool isInsterSuccess = false;
        if (File.Exists(window.filePath)&&insterDic!=null)
        {
            originScript = File.ReadAllText(window.filePath);
            if (string.IsNullOrEmpty(originScript) == false)
            {
                foreach (var item in insterDic)
                {
                    if (!originScript.Contains(item.Key))
                    {
                        int insterIndex = window.GetInserIndex(originScript);
                        //插入新增的数据
                        originScript = window.scriptContent = originScript.Insert(insterIndex, item.Value + "\t\t");
                        isInsterSuccess = true;
                    }
                }
            }
            if (isInsterSuccess == false)
            {
                window.scriptContent = originScript;
            }
        }

        originScript = null;
        insterDic = null;
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
        mMethodDic = null;
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
    public int GetInserIndex(string content)
    {
        //找到UI事件组件下面的第一个public 所在的位置 进行插入
        Regex regex = new Regex("UI组件事件()");
        Match match = regex.Match(content);
        Regex regex1 = new Regex("public");
        MatchCollection matchColltion = regex1.Matches(content);

        for (int i = 0; i < matchColltion.Count; i++)
        {
            if (matchColltion[i].Index > match.Index)
            {
                //Debug.Log(matchColltion[i].Index);
                return matchColltion[i].Index;
            }
        }
        return -1;

    }
}
