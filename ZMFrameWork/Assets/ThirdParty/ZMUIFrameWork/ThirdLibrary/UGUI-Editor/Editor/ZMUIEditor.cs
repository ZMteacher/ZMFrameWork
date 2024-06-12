using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using U3DExtends;
using System;
using UnityEngine.UI;

public class ZMUIEditor : Editor
{
    [InitializeOnLoadMethod]
    private static void Init()
    {
        Action OnEvent = delegate  { ChangeDefaultFont(); };
        EditorApplication.hierarchyChanged += delegate () {OnEvent();};
    }
    private static void ChangeDefaultFont()
    {
        LoadWindowUICamera();
        HanderTextOrImageRaycast();
    }
    private static void HanderTextOrImageRaycast()
    {
        if (Selection.activeGameObject==null )
        {
            return;
        }
        if (Selection.activeGameObject.name.Contains("Text"))
        {
            Text text= Selection.activeGameObject.GetComponent<Text>();
            if (text!=null)
            {
                text.raycastTarget = false;
            }
        }
        else if (Selection.activeGameObject.name.Contains("Image"))
        {
            Image image = Selection.activeGameObject.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = false;
            }
        }
    }
    public static void LoadWindowUICamera()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject uiRoot = GameObject.Find("UIRoot");
            Camera uiCamera = null;
            if (uiRoot != null)
            {
                Transform uiCameraTsf = uiRoot.transform.Find("UICamera");
                if (uiCameraTsf != null)
                {
                    uiCamera = uiCameraTsf.GetComponent<Camera>();
                }
            }
            if (uiCamera == null)
            {
                return;
            }
            if (Selection.activeGameObject.name.Contains("Window"))
            {
                Canvas canvas = Selection.activeGameObject.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.worldCamera = uiCamera;
                }
            }
        }
    }
    [MenuItem("GameObject/添加参考图", false, -1)]
    public static void LoadRefertoTexture()
    {
        Transform parent = null;
        if (Selection.activeObject.name == "UIContent")
        {
            parent = Selection.activeTransform;
        }
        else if (Selection.activeGameObject.name.Contains("Window"))
        {
            parent = Selection.activeTransform.Find("UIContent");
        }
        if (parent == null)
        {
            return;
        }

        GameObject image = CreateRefertoImage();
        image.transform.SetParent(parent);
        image.transform.localScale = Vector3.one;
        image.transform.rotation = Quaternion.identity;
        image.transform.localPosition = Vector3.zero;
        image.transform.SetAsFirstSibling();
        if (U3DExtends.UIEditorHelper.SelectPicForDecorate(image.GetComponent<Decorate>()) == false)
        {
            GameObject.DestroyImmediate(image);
        }
    }
    private static GameObject CreateRefertoImage()
    {
        GameObject image = new GameObject("RefertoImage", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        image.AddComponent<Decorate>();
        return image;
    }
    [MenuItem("GameObject/优化Batch",false,-2)]
    public static void OptimizationUIBatch()
    {
        UILayoutTool.OptimizeBatchForMenu();
    }
}
