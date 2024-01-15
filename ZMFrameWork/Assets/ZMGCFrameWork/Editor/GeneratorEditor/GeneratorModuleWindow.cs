using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GeneratorModuleWindow : EditorWindow
{
    private string mScriptTxt;
    private string mFilePath;
    private string mFileName;
    private Vector2 scroll = Vector2.zero;

    private bool[] muduleToggleArr = new bool[4];
    private string[] mModuleNameArr=new string[] { "HallWorld", "BattleWorld", "Fish3DWorld", "MajiangWorld" };
    private string[] mMudileNameSpaceArr = new[] {"ZMGC.Hall", "ZMGC.Battle", "ZMGC.Fish3", "ZMGC.Majiang" };
    private string[] folderArr = new string[] { "/DataMgr/","/MsgMgr/","/LogicCtrl/"};
    private int mlastIndex = -1;
    private int mlastIndex2 = -1;
    public static void ShowWindow(string conent, string fileName)
    {
        GeneratorModuleWindow window = (GeneratorModuleWindow)GetWindowWithRect(typeof(GeneratorModuleWindow), new Rect(100, 50, 800, 700), true, "模块代码生成界面");
        window.Show();
        window.mScriptTxt = conent;
        window.mFileName = fileName;
    }
    public void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(600), GUILayout.Width(800));
        EditorGUILayout.TextArea(mScriptTxt);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
     
 
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 2; i++)
        {
            muduleToggleArr[i]=EditorGUILayout.Toggle(mModuleNameArr[i],muduleToggleArr[i]);
            if (muduleToggleArr[i]&& mlastIndex != i)
            {
                SetFilePath(i);
                if (mlastIndex >= 0)
                    muduleToggleArr[mlastIndex] = false;
                mlastIndex = i;
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        for (int i = 2; i < 4; i++)
        {
            muduleToggleArr[i] = EditorGUILayout.Toggle(mModuleNameArr[i],muduleToggleArr[i]);
            if (muduleToggleArr[i]&& mlastIndex != i)
            {
                SetFilePath(i);
                if (mlastIndex >= 0)
                    muduleToggleArr[mlastIndex] = false;
                mlastIndex = i;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.TextArea("脚本生成路径：" + mFilePath);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
        {
            CreateScripts();
        }
        EditorGUILayout.EndHorizontal();
    }
    public void SetFilePath(int index)
    {
        string nameSpace = mMudileNameSpaceArr[index];
 
        for (int i = 0; i < mMudileNameSpaceArr.Length; i++)
        {
            if (mScriptTxt.Contains(mMudileNameSpaceArr[i]))
            {
                mScriptTxt= mScriptTxt.Replace(mMudileNameSpaceArr[i],nameSpace);
            }
        }
        string folder = "";
        if (mFileName.Contains("Data"))
            folder = folderArr[0];
        else if (mFileName.Contains("Msg"))
            folder = folderArr[1];
        else if (mFileName.Contains("Logic"))
            folder = folderArr[2];
        mFilePath = ZM.GC.GeneratorConfig.FramworkPath + mModuleNameArr[index] + folder + mFileName;
    }
    public void CreateScripts()
    {
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
