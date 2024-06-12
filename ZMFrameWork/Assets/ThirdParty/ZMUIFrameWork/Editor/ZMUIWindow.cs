/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAssetFrameWork
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
# if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ZMUIWindow : OdinMenuEditorWindow
{
 

    [SerializeField]
    public ZMUIWindow uiSettingWindow ;

    [MenuItem("ZMFrame/ZMUI Setting")]
    public static void ShowAssetBundleWindow()
    {
        ZMUIWindow window = GetWindow<ZMUIWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(985,612);
        window.ForceMenuTreeRebuild();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
 
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "ZMUI Setting",UISetting.Instance,EditorIcons.SettingsCog},
        };
        return menuTree;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UISetting.Instance.Save();
    }
}
#endif