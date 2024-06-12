/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2022/5/16 22:10:23
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using ZM.AssetFrameWork;
using UnityEditor.UIElements;
using UnityEditor;

public class HallWindow : WindowBase
{

    public HallWindowDataComponent dataCompt;

    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        FullScreenWindow = true;
        mDisableAnim = true;
        dataCompt = gameObject.GetComponent<HallWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
    }
    //物体显示时执行
    public override void OnShow()
    {
        base.OnShow();
        //堆栈弹窗只压入，待需要弹出时进行弹出
        //UIModule.Instance.PushWindowToStack<SettingWIndow>();
        //UIModule.Instance.PushWindowToStack<ChatWIndow>();
        //UIModule.Instance.PushWindowToStack<UserInfoWIndow>();
        //UIModule.Instance.StartPopFirstStackWindow();

        //堆栈弹窗压入时立即弹出，若已弹出则只压入
        //UIModule.Instance.PushAndPopStackWindow<UserInfoWIndow>();
        //UIModule.Instance.PushAndPopStackWindow<SettingWIndow>();
        //UIModule.Instance.PushAndPopStackWindow<ChatWIndow>();

    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion
    #region API Function

    #endregion
    #region UI组件事件
    public void OnShopButtonClick()
    {
        PopUpWindow<ExShopWindow>();
    }
    public void OnServiceButtonClick()
    {

    }
    public void OnGameServiceButtonClick()
    {

    }
    public void OnBankButtonClick()
    {

    }
    public void OnShareButtonClick()
    {

    }
 
    public void OnExampleButtonClick()
    {
        ZMAsset.Instantiate(AssetsPathConfig.HALL_PREFABS_PATH + "ExampleWindow", null);
    }
    #endregion
}

