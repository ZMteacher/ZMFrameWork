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
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BundleModuleConfig : OdinEditorWindow
{
    [PropertySpace( spaceAfter:5,spaceBefore:5)]
    [Required("请输入资源模块名称")]
    [GUIColor(0.3f ,0.8f,0.8f,1f)]
    [LabelText("资源模块名称:")]
    public string moduleName;
 
    //[GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    [LabelText("是否可寻址资源?"),InfoBox("区别:用时下载。建议在外围模块使用该功能，因为使用时下载需要Loading进行表现...")]
    public bool isAddressableAsset=false;
    [ReadOnly]
    [HideLabel]
    [TabGroup("预制体包")]
    [DisplayAsString]
    public string prefabTabel = "该文件夹下的所有预制体都会单独打成一个AssetBundle";

    [ReadOnly]
    [HideLabel]
    [TabGroup("文件夹子包")]
    [DisplayAsString]
    public string rootFolderSubBundle = "该文件夹下的所有子文件夹都会单独打成一个AssetBundle";

    [ReadOnly]
    [HideLabel]
    [TabGroup("单个补丁包")]
    [DisplayAsString]
    public string signBundle = "指定的文件夹会单独打成一个AssetBundle";


    [FolderPath]
    [TabGroup("预制体包")]
    [LabelText("预制体资源路径配置")]
    public string[] prefabPathArr = new string[] { "Path..." };


    [FolderPath]
    [TabGroup("文件夹子包")]
    [LabelText("文件夹子包路径配置")]
    public string[] rootFolderPathArr = new string[] {};

     [TabGroup("单个补丁包")]
    [LabelText("单个补丁包路径配置")]
    public BundleFileInfo[] signFolderPathArr = new BundleFileInfo[] {};

    public static void ShowWindow(string moduleName)
    {
        BundleModuleConfig window = GetWindowWithRect<BundleModuleConfig>(new Rect(0,0,600,600));
        window.Show();
        //更新窗口数据 
        BundleModuleData moduleData= BuildBundleConfigura.Instance.GetBundleDataByName(moduleName);
        if (moduleData!=null)
        {
            window.isAddressableAsset = moduleData.isAddressableAsset;
            window.moduleName = moduleData.moduleName;
            window.prefabPathArr = moduleData.prefabPathArr;
            window.rootFolderPathArr = moduleData.rootFolderPathArr;
            window.signFolderPathArr = moduleData.signFolderPathArr;
        }
    }

    /// <summary>
    /// 储存模块资源配置
    /// </summary>
    [OnInspectorGUI]
    public void DrawSaveConfigaurButton()
    {
        //绘制删除配置按钮
        GUILayout.BeginArea(new Rect(0,510,600,200));
        if (GUILayout.Button("DeleteConfiguration",GUILayout.Height(47)))
        {
            DeleteConfiguration();
            GUIUtility.ExitGUI();
        }
        GUILayout.EndArea();
        //绘制保存当前配置的按钮
        GUILayout.BeginArea(new Rect(0, 555, 600, 200));
        if (GUILayout.Button("SaveConfiguration", GUILayout.Height(47)))
        {
            SaveConfiguration();
            GUIUtility.ExitGUI();
        }
        GUILayout.EndArea();
    }
    /// <summary>
    /// 删除资源模块配置
    /// </summary>
    public void DeleteConfiguration()
    {
        BuildBundleConfigura.Instance.RemoveModuleByName(moduleName);
        UnityEditor.EditorUtility.DisplayDialog("删除成功！","配置以删除","确定");
        Close();
        BuildWindows.ShowAssetBundleWindow();
    }
    /// <summary>
    /// 储存资源模块配置
    /// </summary>
    public void SaveConfiguration()
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            UnityEditor.EditorUtility.DisplayDialog("保存失败！", "模块名称不能为空", "确定");
            return;
        }

        BundleModuleData moduleData = BuildBundleConfigura.Instance.GetBundleDataByName(moduleName);
       
        if (moduleData==null)
        {
            //添加新的模块资源
            moduleData = new BundleModuleData();
            moduleData.moduleName = this.moduleName;
            moduleData.isAddressableAsset = this.isAddressableAsset;
            moduleData.prefabPathArr = this.prefabPathArr;
            moduleData.rootFolderPathArr = this.rootFolderPathArr;
            moduleData.signFolderPathArr = this.signFolderPathArr;
            BuildBundleConfigura.Instance.SaveModuleData(moduleData);
        }
        else
        {
            moduleData.prefabPathArr = this.prefabPathArr;
            moduleData.rootFolderPathArr = this.rootFolderPathArr;
            moduleData.signFolderPathArr = this.signFolderPathArr;
            BuildBundleConfigura.Instance.SaveModuleData(moduleData);
        }
      
        UnityEditor.EditorUtility.DisplayDialog("保存成功！", "配置以储存", "确定");
        Close();
        BuildWindows.ShowAssetBundleWindow();
    }
}
