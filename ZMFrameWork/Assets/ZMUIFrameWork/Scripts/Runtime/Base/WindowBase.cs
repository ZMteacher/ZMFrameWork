using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehaviour
{
    private List<Button> mAllButtonList = new List<Button>();//所有Button列表
    private List<Toggle> mToggleList = new List<Toggle>();//所有的Toggle列表
    private List<InputField> mInputList = new List<InputField>();//所有的输入框列表

    private CanvasGroup mUIMask;
    private CanvasGroup mCanvasGroup;
    protected Transform mUIContent;
    protected bool mDisableAnim = false;//禁用动画
    /// <summary>
    /// 初始化基类组件
    /// </summary>
    private void InitializeBaseComponent()
    {
        mCanvasGroup = transform.GetComponent<CanvasGroup>();
        mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
        mUIContent = transform.Find("UIContent").transform;
    }
    #region 声明周期
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
            mUIMask.alpha = 0;
            mUIMask.DOFade(1, 0.2f);
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
        //gameObject.SetActive(isVisble);//临时代码
        mCanvasGroup.alpha = isVisble ? 1 : 0;
        mCanvasGroup.blocksRaycasts = isVisble;
        Visible = isVisble;
    }
    public void SetMaskVisible(bool isVisble)
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM)
        {
            return;
        }
        mUIMask.alpha = isVisble ? 1 : 0;
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
