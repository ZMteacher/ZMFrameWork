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
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildWindows : OdinMenuEditorWindow
{
    [SerializeField]
    public BuildBundleWindow buildBundleWindow = new BuildBundleWindow();

    [SerializeField]
    public BuildHotPatchWindow buildHotWindow = new BuildHotPatchWindow();

    [SerializeField]
    public BundleSettings settingWindow ;

    [MenuItem("ZMFrame/Build BundleWindow",false,0)]
    public static void ShowAssetBundleWindow()
    {
        try
        {
            BuildWindows window = GetWindow<BuildWindows>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1010, 612);
            window.ForceMenuTreeRebuild();
        }
        catch (System.Exception)
        {

       
        }
       
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        buildBundleWindow.Initzation(550);
        buildHotWindow.Initzation(390);
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Build",null,EditorIcons.House},
            { "Build/AssetBundle",buildBundleWindow,EditorIcons.UnityLogo},
            { "Build/HotPatch",buildHotWindow,EditorIcons.UnityLogo},
            { "Bundle Setting",BundleSettings.Instance,EditorIcons.SettingsCog},
        };
        return menuTree;
    }
}
