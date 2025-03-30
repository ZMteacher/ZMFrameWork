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
# if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
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