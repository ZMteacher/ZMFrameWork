using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
