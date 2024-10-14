# if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
public enum GeneratorType
{
# if ODIN_INSPECTOR
    [LabelText("组件自动查找", SdfIconType.Fingerprint)]
#endif
    Find,//组件查找
#if ODIN_INSPECTOR
    [LabelText("组件自动绑定")]
#endif
    Bind,//组件绑定
}
public enum ParseType
{
#if ODIN_INSPECTOR
    [LabelText("名称解析 [Button]Start")]
#endif
    Name,
#if ODIN_INSPECTOR
    [LabelText("标签Tag解析(例:节点右上角TAG)")]
#endif
    Tag
}
[CreateAssetMenu(fileName = "UISetting", menuName = "UISetting", order = 0)]
public class UISetting : ScriptableObject
{
    private static UISetting _instance;
    public static UISetting Instance { get { if (_instance == null) { _instance = Resources.Load<UISetting>("UISetting"); } return _instance; } }
#if ODIN_INSPECTOR

    [Title("窗口遮罩模式", "True：开启单遮罩模式(多个窗口叠加只有一个Mask遮罩，透明度唯一)" +
        "False:开启叠着模式(一个窗口一个单独的Mask遮罩，透明度叠加)")]
    [LabelText("是否启用单遮模式")]
#endif
    public bool SINGMASK_SYSTEM;//是否启用单遮模式

#if ODIN_INSPECTOR

    [EnumToggleButtons, HideLabel, BoxGroup("代码自动化生成设置  建议使用名称解析+组件绑定方式 (兼容性好，性能好)"), Title("组件解析方式"),OnValueChanged("OnParseTypeEnumChang")] 
#endif
    public ParseType ParseType = ParseType.Name;
#if ODIN_INSPECTOR
    [EnumToggleButtons, HideLabel, BoxGroup("代码自动化生成设置  建议使用名称解析+组件绑定方式 (兼容性好，性能好)"), Title("代码生成方式"),OnValueChanged("OnBindEnumChang")]//LabelText("组件解析方式")
#endif
    public GeneratorType GeneratorType = GeneratorType.Bind;

#if ODIN_INSPECTOR
    [TitleGroup("脚本自动化生成路径配置", "自定义生成路径"), LabelText("组件绑定脚本生成路径"), FolderPath]
#endif
    public string BindComponentGeneratorPath =  "Assets/ThirdParty/ZMUIFrameWork/Scripts/BindCompoent";
#if ODIN_INSPECTOR
    [TitleGroup("脚本自动化生成路径配置", "自定义生成路径"), LabelText("组件查找脚本生成路径"), FolderPath,ShowIf("mShowFindComponentGeneratorPath")]
#endif
    public string FindComponentGeneratorPath = "Assets/ThirdParty/ZMUIFrameWork/Scripts/FindCompoent";
#if ODIN_INSPECTOR
    [TitleGroup("脚本自动化生成路径配置", "自定义生成路径"), LabelText("窗口交互脚本生成路径"), FolderPath]
#endif
    public string WindowGeneratorPath ="Assets/ThirdParty/ZMUIFrameWork/Scripts/Window";
#if ODIN_INSPECTOR
    [TitleGroup("窗口预制体加载路径配置","框架根据以下路径自动计算加载路径，新增窗口无需手动配置"), LabelText("窗口预制体存放路径"), FolderPath]
#endif
    public string[] WindowPrefabFolderPathArr;
#if ODIN_INSPECTOR
    private bool mShowFindComponentGeneratorPath;
    public void OnBindEnumChang(GeneratorType type)
    {
        mShowFindComponentGeneratorPath = type == GeneratorType.Find;
        Save();
    }
    public void OnParseTypeEnumChang(ParseType type)
    {
        Save();
    }
#endif
    public void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
#endif
    }

}