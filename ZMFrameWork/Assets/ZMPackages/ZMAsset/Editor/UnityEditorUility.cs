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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEditorUility 
{




    public static GUIStyle GetGUIStyle(string styleName)
    {
        GUIStyle gUIStyle = null;
        foreach (var item in GUI.skin.customStyles)
        {
            if (string.Equals(item.name.ToLower(), styleName.ToLower()))
            {
                gUIStyle = item;
                gUIStyle.font = new GUIStyle().font;
                break;
            }
        }
        return gUIStyle;
    }

}
