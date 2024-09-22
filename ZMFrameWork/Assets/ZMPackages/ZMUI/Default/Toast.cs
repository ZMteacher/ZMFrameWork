/*--------------------------------------------------------------------------------------
* Title: UI表现层脚本自动生成工具
* Author: ZM
* Date:2021/9/8 15:26:41
* Description:UI表现层，改层只负责界面的交互、表现与更新，不允许编写任何业务逻辑 
* Modify:
* 注意:以下文件为自动生成
----------------------------------------------------------------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;
 
 
public class Toast : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform ContentRootTrans;
    public Canvas canvas;
    public Text ConentTextPro;

    private float mShowTime=2;
    /// <summary>
    /// 显示提示符
    /// </summary>
    public void ShowToast(string key)
    {
        Debug.Log("ShowToast:" + key);
        if (canvas.worldCamera == null)
            canvas.worldCamera = UIModule.Instance.Camera;

        ConentTextPro.text = key;
        ContentRootTrans.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        ContentRootTrans.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        canvasGroup.alpha = 1;
        CancelInvoke();
        Invoke("HideToast", mShowTime);
    }

    public void ShowToast(int key)
    {
        //int 类型的taost 一般都是服务端返回的，直接进行查表即可
        Debug.Log("ShowSocket Toast:" + key);
        //SocketErrorData errorData = ConfigConter.GetSocketErrorDataByID(key);
        //string toast = LocalizationManager.Instance.CorrectText(errorData.value);
        ShowToast(key);
    }

    public void HideToast()
    {
        canvasGroup.alpha = 0;
    }
}
