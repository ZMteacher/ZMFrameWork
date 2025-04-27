/*---------------------------------------------------------------------------------------------------------------------------------------------
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
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class BundleBehaviour 
{
    /// <summary>
    /// 模块配置列表
    /// </summary>
    protected List<BundleModuleData> moduleDataList;
    /// <summary>
    /// 模块配置行列表
    /// </summary>
    protected List<List<BundleModuleData>> rowModuleDataList;
    
    protected Vector2 scrollPosition;

    protected string curPlatfam;

    private GUIContent content;
    
    private int contentHeight;
    public virtual void Initzation(int height)
    {
        contentHeight=height;
        //获取多模块资源配置列表
        moduleDataList = BuildBundleConfigura.Instance.AssetBundleConfig;
        rowModuleDataList = new List<List<BundleModuleData>>();
       
        for (int i = 0; i < moduleDataList.Count; i++)
        {
            //计算模块绘制的行数索引
            int rowIndex = Mathf.FloorToInt(i/6);
            if (rowModuleDataList.Count<rowIndex+1)
            {
                rowModuleDataList.Add(new List<BundleModuleData>());
            }
            //往行列表中添加数据
            rowModuleDataList[rowIndex].Add(moduleDataList[i]);
        }

        if (moduleDataList.Count%6==0)
        {
            rowModuleDataList.Add(new List<BundleModuleData>{new BundleModuleData{ isAddModule = true}});
        }
        else
        {
            rowModuleDataList[moduleDataList.Count/6].Add(new BundleModuleData{ isAddModule = true});
        }
#if UNITY_IOS
        curPlatfam = "BuildSettings.iPhone";
#else
        curPlatfam = "BuildSettings.Android";
#endif
        //获取Unity Logo图标
        content = EditorGUIUtility.IconContent("SceneAsset Icon".Trim() ,"测试文字显示");
        content.tooltip = "单击可选中和取消\n快速双击可打开配置窗口";
    }

    [OnInspectorGUI]
    public virtual void OGUI()
    {
        if (rowModuleDataList==null)
        {
            return;
        }
        
        // 添加滚动视图
        scrollPosition = GUILayout.BeginScrollView(scrollPosition,GUILayout.Height(contentHeight));
        // 外层使用垂直布局（默认）
        GUILayout.BeginVertical();
        for (int i = 0; i < rowModuleDataList.Count; i++)
        {
            //开始横向绘制
            GUILayout.BeginHorizontal();

            for (int j = 0; j < rowModuleDataList[i].Count; j++)
            {
                BundleModuleData moduleData = rowModuleDataList[i][j];
                if (moduleData.isAddModule)
                {
                    GUIContent addContent = EditorGUIUtility.IconContent("CollabCreate Icon".Trim(), "");
                    if (GUILayout.Button(addContent, GUILayout.Width(130), GUILayout.Height(170)))
                    {
                        BundleModuleConfig.ShowWindow("");
                        GUIUtility.ExitGUI();
                    }
                    continue;
                }
                //绘制按钮
                if (GUILayout.Button(content, GUILayout.Width(130), GUILayout.Height(170)))
                {
                    moduleData.isBuild = moduleData.isBuild == false ? true : false;
                    //检测按钮是否是双击
                    if (Time.realtimeSinceStartup-moduleData.lastClickBtnTime<=0.18f)
                    {
                        BundleModuleConfig.ShowWindow(moduleData.moduleName);
                    }
                    moduleData.lastClickBtnTime = Time.realtimeSinceStartup;
                    GUIUtility.ExitGUI();
                }
                GUI.Label(new Rect(j==0?10:(j+1)*16+(j*116),150*(i+1)+(i*20),115,20),moduleData.moduleName,new GUIStyle { alignment =TextAnchor.MiddleCenter});
                //绘制按钮选中的高亮效果
                if (moduleData.isBuild)
                {
                    GUIStyle style= UnityEditorUility.GetGUIStyle("LightmapEditorSelectedHighlight");
                    style.contentOffset = new Vector2(100,-70);
                    // GUI.Toggle(new Rect(10+(j*132f),-160+1*(i+1)+((i+1)*167.5f),120,160),true,EditorGUIUtility.IconContent("Collab"), style);
                    GUI.Toggle(new Rect(10+(j*132f),(i+1)*7+(i*165),120,160),true,EditorGUIUtility.IconContent("Collab"), style);

                }
            }
            //结束横向绘制
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        DrawBuildButtons();
    }


    /// <summary>
    /// 绘制打包按钮
    /// </summary>
    public virtual void DrawBuildButtons()
    {

    }

    /// <summary>
    /// 打包资源
    /// </summary>
    public virtual void BuildBundle()
    {

    }
    
}
