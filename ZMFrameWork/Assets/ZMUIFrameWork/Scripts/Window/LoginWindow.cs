/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/1/15 15:59:57
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using ZMGC.Hall;

public class LoginWindow : WindowBase
{

    public LoginWindowDataComponent dataCompt;
    public LoginLogicCtrl mLoginLogic;
    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<LoginWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
        mLoginLogic = HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>();
    }
    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        //事件需要放到OnShow中监听，否则窗口隐藏时该事件监听仍会触发
        UIEventControl.AddEvent(UIEventEnum.LoginSuccess, OnLoginResult);
    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
        UIEventControl.RemoveEvent(UIEventEnum.LoginSuccess, OnLoginResult);
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion
    #region API Function
    public void OnLoginResult(object data)
    {

    }
    #endregion
    #region UI组件事件
    public void OnLoginButtonClick()
    {
        mLoginLogic.AccountLogin(dataCompt.AccountInputField.text, dataCompt.PassInputField.text);
    }
    public void OnAccountInputChange(string text)
    {

    }
    public void OnAccountInputEnd(string text)
    {

    }
    public void OnPassInputChange(string text)
    {

    }
    public void OnPassInputEnd(string text)
    {

    }
    #endregion
}
