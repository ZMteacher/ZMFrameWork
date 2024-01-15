using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRebuildTest : MonoBehaviour
{
    IList<ICanvasElement> mLayoutRebuildQueue;
    IList<ICanvasElement> mGraphicRebuildQueue;
    void Start()
    {
        Type type = typeof(CanvasUpdateRegistry);
        FieldInfo field = type.GetField("m_LayoutRebuildQueue",BindingFlags.NonPublic|BindingFlags.Instance);
        mLayoutRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);

        field = type.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        mGraphicRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
    }

    
    void Update()
    {
        for (int i = 0; i < mLayoutRebuildQueue.Count; i++)
        {
            var rebuild = mLayoutRebuildQueue[i];
            if (ObjectValidForUpdate(rebuild)&& rebuild.transform.GetComponent<Graphic>()!=null && rebuild.transform.GetComponent<Graphic>().canvas != null)
            {
                Debug.LogFormat("{0}引起{1}网格重建", rebuild.transform.name, rebuild.transform.GetComponent<Graphic>().canvas.name);
            }
        }
        for (int i = 0; i < mGraphicRebuildQueue.Count; i++)
        {
            var rebuild = mGraphicRebuildQueue[i];
            if (ObjectValidForUpdate(rebuild)&& rebuild.transform.GetComponent<Graphic>()!=null&& rebuild.transform.GetComponent<Graphic>().canvas!=null)
            {
                Debug.LogFormat("{0}引起{1}网格重建", rebuild.transform.name, rebuild.transform.GetComponent<Graphic>().canvas.name);
            }
        }
    }
    private bool ObjectValidForUpdate(ICanvasElement element)
    {
        var valid = element != null;
        var isUnityObject = element is UnityEngine.Object;
        if (isUnityObject)
        {
            valid = (element as object) != null;
        }
        return valid;
    }
}
