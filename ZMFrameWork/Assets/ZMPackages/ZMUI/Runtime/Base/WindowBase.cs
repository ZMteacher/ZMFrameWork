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
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehaviour
{
    private CanvasGroup mUIMaskCanvasGroup;
    private CanvasGroup mCanvasGroup;
    protected Transform mUIContent;

    private List<Toggle> mToggleList = new List<Toggle>();//所有的Toggle列表
    private List<Button> mAllButtonList = new List<Button>();//所有Button列表
    private List<InputField> mInputList = new List<InputField>();//所有的输入框列表

    protected bool mDisableAnim = false;//禁用动画

    /// <summary>
    /// 初始化基类组件
    /// </summary>
    private void InitializeBaseComponent()
    {
        mCanvasGroup = transform.GetComponent<CanvasGroup>();
        mUIMaskCanvasGroup = transform.Find("UIMask").GetComponent<CanvasGroup>();
        mUIContent = transform.Find("UIContent").transform;
    }
    #region 生命周期
    public override void OnAwake()
    {
        base.OnAwake();
        InitializeBaseComponent();

    }
    public override void OnShow()
    {
        base.OnShow();
        ShowAnimation();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

    }

    public override void OnHide()
    {
        base.OnHide();

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveAllButtonListener();
        RemoveAllToggleListener();
        RemoveAllInputListener();
        mAllButtonList.Clear();
        mToggleList.Clear();
        mInputList.Clear();
    }
    #endregion

    #region 动画管理
    public void ShowAnimation()
    {
        //基础弹窗不需要动画
        if (Canvas.sortingOrder > 90 && mDisableAnim == false)
        {
            //Mask动画
            mUIMaskCanvasGroup.alpha = 0;
            mUIMaskCanvasGroup.DOFade(1, 0.2f);
            //缩放动画
            mUIContent.localScale = Vector3.one * 0.8f;
            mUIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }
    public void HideAnimation()
    {
        if (Canvas.sortingOrder > 90 && mDisableAnim == false)
        {
            mUIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                UIModule.Instance.HideWindow(Name);
            });
        }
        else
        {
            UIModule.Instance.HideWindow(Name);
        }
    }
    #endregion
    public void HideWindow()
    {
        HideAnimation();
        //UIModule.Instance.HideWindow(Name);
    }
    public override void SetVisible(bool isVisble)
    {
        if (mCanvasGroup == null)
        {
            Debug.LogError("CanvasGroup is Null!" + Name);
            return;
        }
 
        Visible = mCanvasGroup.interactable = mCanvasGroup.blocksRaycasts = isVisble;
        mCanvasGroup.alpha = isVisble ? 1 : 0;
        if (isVisble && PopStack)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
 
    public void SetMaskVisible(bool isVisible)
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM) return;
        mUIMaskCanvasGroup.alpha = isVisible ? 1 : 0;
        mUIMaskCanvasGroup.blocksRaycasts = isVisible;
        //特殊情况下进行窗口同层级重绘渲染
        if (isVisible && PopStack)
        {
            mUIMaskCanvasGroup.gameObject.SetActive(false);
            mUIMaskCanvasGroup.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 伪隐藏触发接口
    /// </summary>
    /// <param name="value"></param>
    public virtual void PseudoHidden(int value)
    {
        mUIMaskCanvasGroup.alpha = mCanvasGroup.alpha = value;
        mUIMaskCanvasGroup.interactable = mCanvasGroup.interactable = value == 1 ? true : false;
        mUIMaskCanvasGroup.blocksRaycasts = mCanvasGroup.blocksRaycasts = value == 1 ? true : false;

    }
    public void PopUpWindow<T>() where T:WindowBase,new ()
    {
        UIModule.Instance.PopUpWindow<T>();
    }
    #region 事件管理
    public void AddButtonClickListener(Button btn, UnityAction action)
    {
        if (btn != null)
        {
            if (!mAllButtonList.Contains(btn))
            {
                mAllButtonList.Add(btn);
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);

        }
    }
    public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
    {
        if (toggle != null)
        {
            if (!mToggleList.Contains(toggle))
            {
                mToggleList.Add(toggle);
            }
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                action?.Invoke(isOn, toggle);
            });

        }
    }
    public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction, UnityAction<string> endAction)
    {
        if (input != null)
        {
            if (!mInputList.Contains(input))
            {
                mInputList.Add(input);
            }
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            input.onValueChanged.AddListener(onChangeAction);
            input.onEndEdit.AddListener(endAction);
        }
    }
    public void RemoveAllButtonListener()
    {
        foreach (var item in mAllButtonList)
        {
            item.onClick.RemoveAllListeners();
        }
    }

    public void RemoveAllToggleListener()
    {
        foreach (var item in mToggleList)
        {
            item.onValueChanged.RemoveAllListeners();
        }
    }

    public void RemoveAllInputListener()
    {
        foreach (var item in mInputList)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onEndEdit.RemoveAllListeners();
        }

    }
    #endregion
}
