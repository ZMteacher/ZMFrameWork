﻿/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAsset
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZM.ZMAsset;
public class BuildHotPatchWindow : BundleBehaviour
{
    protected string[] buildButtonsNameArr = new string[] { "打包热更补丁", "上传资源" };
    //热更描述 热更公告
    [HideInInspector]public string patchDes = "输入本次热更描述...";
    //热更应用补丁版本
    [HideInInspector] public string hotAppVersion = "1.0.0";
    //热更补丁版本
    [HideInInspector] public string hotVersion = "1";
   
  
    public override void OGUI()
    {
        base.OGUI();
        GUILayout.BeginArea(new Rect(0,400,800,600));

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请输入本次热更公告");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        patchDes= GUILayout.TextField(patchDes,GUILayout.Width(800),GUILayout.Height(80));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        hotAppVersion = EditorGUILayout.TextField( "生效应用版本: (0.0.0表示所有版本)", hotAppVersion, GUILayout.Width(800), GUILayout.Height(24));
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        hotVersion = EditorGUILayout.TextField( "热更补丁版本:", hotVersion, GUILayout.Width(800), GUILayout.Height(24));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();


    }
    public override void DrawBuildButtons()
    {
        base.DrawBuildButtons();
        GUILayout.BeginArea(new Rect(0, 555, 800, 600));

        GUILayout.BeginHorizontal();

        for (int i = 0; i < buildButtonsNameArr.Length; i++)
        {
            GUIStyle style = UnityEditorUility.GetGUIStyle("PreButtonBlue");
            style.fixedHeight = 55;

            if (GUILayout.Button(buildButtonsNameArr[i], style, GUILayout.Height(400)))
            {
                if (i == 0)
                {
                    //打包AssetBundle按钮事件
                    BuildBundle();
                    
                }
                else
                {
                    CopyBundleToStreamingAssetsPath();
                }
                GUIUtility.ExitGUI();
            }
        }

        //打包图标绘制完成
        GUI.DrawTexture(new Rect(130, 13, 30, 30), EditorGUIUtility.IconContent(curPlatfam).image);
        //内嵌资源图标绘制完成
        GUI.DrawTexture(new Rect(545, 13, 30, 30), EditorGUIUtility.IconContent("CollabPush").image);

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public override void BuildBundle()
    {
        base.BuildBundle();
        foreach (var item in moduleDataList)
        {
            if (item.isBuild)
            {
                BuildBundleCompiler.BuildAssetBundle(item,BuildType.HotPatch,int.Parse(hotVersion),hotAppVersion, patchDes);
            }
        }
    }

    /// <summary>
    /// 内嵌资源
    /// </summary>
    public void CopyBundleToStreamingAssetsPath()
    {
        foreach (var item in moduleDataList)
        {
            if (item.isBuild)
            {
                //TODO 
            }
        }
    }
}
