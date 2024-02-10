using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneratorType
{
    Find,//组件查找
    Bind,//组件绑定
}
public enum ParseType
{
    Name,
    Tag
}
public class GeneratorConfig
{
    public static string BindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/BindCompoent";
    public static string FindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/FindCompoent";
    public static string WindowGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/Window";
    public static string OBJDATALIST_KEY = "objDataList";
    public static GeneratorType GeneratorType = GeneratorType.Bind;
    public static ParseType ParseType = ParseType.Name;
    public static string[] TAGArr = { "Text","TextPro", "ZM_EmojiTextPro", "Image","RawImage","Button","InputField", "Toggle","Slider","Scrollbar","DropDown",
    "Canvas","Panel","ScrollRect","LoopListView2","Transform","RectTransform","GameObject"};
}
