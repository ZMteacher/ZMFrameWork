using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneratorModuleWindow : EditorWindow
{

    public GeneratorModuleConfig moduleConfig; // 引用ScriptableObject配置

    private string mScriptTxt;
    private string mFilePath;
    private string mFileName;
    private Vector2 scroll = Vector2.zero;
    private Vector2 toggleScrollPos; // 滚动位置

    private bool[] mModuleToggleArr = new bool[] { };
   
    private string[] mFolderArr = new string[] { "/DataMgr/","/MsgMgr/","/LogicCtrl/"};
    private int mLastIndex = -1;
    private int mColumns =3; // 指定每行的 Toggle 个数
    private int mColumnWidth=300;
    public static void ShowWindow(string conent, string fileName)
    {
        GeneratorModuleWindow window = (GeneratorModuleWindow)GetWindow(typeof(GeneratorModuleWindow),false,"MVC脚本生成检查器");
        window.Show();
        window.mScriptTxt = conent;
        window.mFileName = fileName;
    }

    public void OnGUI()
    {
        if (moduleConfig == null)
        {
            EditorGUILayout.HelpBox("请分配 GeneratorModuleConfig 配置文件。", MessageType.Warning);
            return;
        }
        int toggleCount = moduleConfig.modules.Length;
        // 检查并初始化 moduleToggleArr 的长度
        if (mModuleToggleArr.Length != toggleCount)
        {
            mModuleToggleArr = new bool[toggleCount];
        }

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(400), GUILayout.Width(800));
        EditorGUILayout.TextArea(mScriptTxt);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        // 计算行数
        int rows = (moduleConfig.modules.Length + mColumns - 1) / mColumns;
 
        //绘制Toggle
        for (int row = 0; row < rows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < mColumns; col++)
            {
                int index = row * mColumns + col;
                if (index < toggleCount)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(mColumnWidth));
                    mModuleToggleArr[index] = EditorGUILayout.Toggle(moduleConfig.modules[index].moduleName, mModuleToggleArr[index]);
                    if (mModuleToggleArr[index] && mLastIndex != index)
                    {
                        SetFilePath(index);
                        if (mLastIndex >= 0)
                            mModuleToggleArr[mLastIndex] = false;
                        mLastIndex = index;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
 

     

        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace(); // 将按钮推到窗口的最底部
        EditorGUILayout.TextArea("脚本生成路径：" + mFilePath);
        EditorGUILayout.Space();
        if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
        {
            CreateScripts();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }
    public void SetFilePath(int index)
    {
        string nameSpace = moduleConfig.modules[index].moduleNamespace;
 
        for (int i = 0; i < moduleConfig.modules.Length; i++)
        {
            if (mScriptTxt.Contains(moduleConfig.modules[i].moduleNamespace))
            {
                mScriptTxt= mScriptTxt.Replace(moduleConfig.modules[i].moduleNamespace,nameSpace);
            }
        }
        string folder = "";
        if (mFileName.Contains("Data"))
            folder = mFolderArr[0];
        else if (mFileName.Contains("Msg"))
            folder = mFolderArr[1];
        else if (mFileName.Contains("Logic"))
            folder = mFolderArr[2];
        mFilePath = $"{Application.dataPath}/{moduleConfig.savePath}/{moduleConfig.modules[index].moduleName}{folder}{mFileName}";
    }
    public void CreateScripts()
    {
        // 检查并创建目标文件夹路径
        string directoryPath = Path.GetDirectoryName(mFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(mFilePath))
            File.Delete(mFilePath);


        StreamWriter writer = File.CreateText(mFilePath);
        writer.Write(mScriptTxt);
        writer.Close();
        writer.Dispose();
        mScriptTxt = string.Empty;
        Debug.Log("Create Code finish! Cs path:" + mFilePath);
        AssetDatabase.Refresh();
        if (EditorUtility.DisplayDialog("自动化工具", "生成脚本成功！", "确定"))
        {
            Close();
        }
    }
}
