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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTipsWindow : MonoBehaviour
{
    //更新回调
    Action OnUpdateCallBack;
    //退出回调
    Action OnQuitCallBack;
    //内容文本
    public Text contentText;

    public void InitView(string content, Action updateCallBack, Action quitCallBack)
    {
        OnUpdateCallBack = updateCallBack;
        OnQuitCallBack = quitCallBack;
        contentText.text = content;
    }

    public void OnUpdateButtonClick()
    {
        OnUpdateCallBack?.Invoke();
        Destroy(gameObject);
    }

    public void OnQuitButtonClick()
    {
        OnQuitCallBack?.Invoke();
        Destroy(gameObject);
    }
}
