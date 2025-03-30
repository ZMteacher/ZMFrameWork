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
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIEditor : Editor
{
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        //监听hierarchy发生改变的委托
        EditorApplication.hierarchyChanged += HanderTextOrImageRaycast;
        EditorApplication.hierarchyChanged += LoadWindowCamera;
    }
    private static void HanderTextOrImageRaycast()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            if (obj.name.Contains("Text"))
            {
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    text.raycastTarget = false;
                }
            }
            else if (obj.name.Contains("Image"))
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = false;
                }
                else
                {
                    RawImage rawImage = obj.GetComponent<RawImage>();
                    if (rawImage != null)
                    {
                        rawImage.raycastTarget = false;
                    }
                }
            }

        }
    }

    private static void LoadWindowCamera()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject uiCameraObj = GameObject.Find("UICamera");
            if (uiCameraObj != null)
            {
                Camera camera = uiCameraObj.GetComponent<Camera>();
                if (Selection.activeGameObject.name.Contains("Window"))
                {
                    Canvas canvas = Selection.activeGameObject.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.worldCamera = camera;
                    }
                }
            }
        }
    }
}
