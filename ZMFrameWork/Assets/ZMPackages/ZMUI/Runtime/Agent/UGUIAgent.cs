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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UGUI拓展方法
/// </summary>
public static class UGUIAgent  
{
    public static void SetVisible(this GameObject obj, bool visible)
    {
        obj.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

    public static void SetVisible(this Transform trans, bool visible)
    {
        trans.localScale = visible ? Vector3.one : Vector3.zero;
    }

    public static void SetVisible(this Button button, bool visible)
    {
        button.transform. localScale = visible ? Vector3.one : Vector3.zero;
    }
    public static void SetVisible(this Text text, bool visible)
    {
        text.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

    public static void SetVisible(this Slider slider, bool visible)
    {
        slider.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

    public static void SetVisible(this Toggle toggle, bool visible)
    {
        toggle.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

    public static void SetVisible(this InputField input, bool visible)
    {
        input.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    public static void SetVisible(this RawImage image, bool visible)
    {
        image.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }
    public static void SetVisible(this ScrollRect scroll, bool visible)
    {
        scroll.transform.localScale = visible ? Vector3.one : Vector3.zero;
    }

}
