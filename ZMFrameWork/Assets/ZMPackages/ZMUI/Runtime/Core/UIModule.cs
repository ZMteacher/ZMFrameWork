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
using System;
using System.Collections.Generic;
using UnityEngine;
using ZM.UI;
using ZM.ZMAsset;
using Object = UnityEngine.Object;

public class UIModule
{
    #region 单例
    private static UIModule _instance;
    public static UIModule Instance { get { if (_instance == null) { _instance = new UIModule(); } return _instance; } }
    #endregion

    #region 属性 字段
    /// <summary>
    /// UI摄像机
    /// </summary>
    private Camera mUICamera;
    public Camera Camera { get { return mUICamera; } }
    /// <summary>
    /// UI节点
    /// </summary>
    private Transform mUIRoot;
    /// <summary>
    /// 窗口配置表
    /// </summary>
    private WindowConfig mWindowConfig;
    /// <summary>
    /// 所有已克隆的窗口的字典 (包含显示及隐藏的窗口,不含已销毁的窗口)
    /// </summary>
    private Dictionary<string, WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();//所有窗口的Dic
    /// <summary>
    /// 所有已克隆的窗口的列表(包含显示及隐藏的窗口,不含已销毁的窗口)
    /// </summary>
    private List<WindowBase> mAllWindowList = new List<WindowBase>();
    /// <summary>
    /// //所有可见窗口的列表 
    /// </summary>
    private List<WindowBase> mVisibleWindowList = new List<WindowBase>();
    /// <summary>
    /// 队列， 用来管理弹窗的循环弹出
    /// </summary>
    private Queue<WindowBase> mWindowStack = new Queue<WindowBase>();
    /// <summary>
    /// 开始弹出堆栈的标记，可以用来处理多种情况，比如：正在出栈种有其他界面弹出，可以直接放到栈内进行弹出 等
    /// </summary>
    private bool mStartPopStackWndStatus = false;
    #endregion

    #region 智能显隐
    private bool mSmartShowHide=true;//智能显隐开关（可根据情况选择开启或关闭）
                                     //智能显隐：主要用来优化窗口叠加时被遮挡的窗口参与渲染计算，导致帧率降低的问题。
                                     //显隐规则：由程序设定某个窗口是否为全屏窗口。(全屏窗口设定方式：在窗口的OnAwake接口中设定该窗口是否为全屏窗口如 FullScreenWindow=true)
                                     //1.智能隐藏:当FullScreenWindow=true的全屏窗口打开时，框架会自动通过伪隐藏的方式隐藏所有被当前全屏窗口遮挡住的窗口，避免这些看不到的窗口参与渲染运算，
                                     //从而提高性能。
                                     //2.智能显示：当FullScreenWindow=true的全屏窗口关闭时，框架会自动找到上一个伪隐藏的窗口把其设置为可见状态，若上一个窗口为非全屏窗口，框架则会找上上个窗口进行显示，
                                     //以此类推进行循环，直到找到全屏窗口则停止智能显示流程。
                                     //注意：通过智能显隐进行伪隐藏的窗口在逻辑上仍属于显示中的窗口，可以通过GetWindow获取到该窗口。但是在表现上该窗口为不可见窗口，故称之为伪隐藏。
                                     //智能显隐逻辑与（打开当前窗口时隐藏其他所有窗口相似）但本质上有非常大的区别，
                                     //1.通过智能显隐设置为不可见的窗口属于伪隐藏窗口，在逻辑上属于显示中的窗口。
                                     //2.通过智能显隐设置为不可见的窗口可以通过关闭当前窗口，自动恢复当前窗口之前的窗口的显示。
                                     //3.通过智能显隐设置为不可见的窗口不会触发UGUI重绘、不会参与渲染计算、不会影响帧率。
                                     //4.程序只需要通过FullScreenWindow=true配置那些窗口为全屏窗口即可，智能显隐的所有逻辑均有框架自动维护处理。
    #endregion

    #region 框架初始化接口 (外部调用)
    public void Initialize()
    {
        mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        mUIRoot = GameObject.Find("UIRoot").transform;
        // mWindowConfig = Resources.Load<WindowConfig>("WindowConfig");
        mWindowConfig = ZMAsset.LoadScriptableObject<WindowConfig>(AssetsPathConfig.HALL_DATA_PATH+"WindowConfig.asset");
        AdaptationBangs.InitializeAdaptation();
        //在手机上不会触发调用
#if UNITY_EDITOR
        mWindowConfig.GeneratorWindowConfig();
#endif
    }

    /// <summary>
    /// 添加窗口元数据 (在HyBridCLR多模块资源+独立代码热更程序集时使用) 主要作用是添加热更窗口数据至AOT或热更程序集内
    /// </summary>
    /// <param name="config"></param>
    public void AddAOTWindowMetadata(WindowConfig config)
    {
        mWindowConfig.AddAOTWindowMetadata(config);
    }

    #endregion

    #region 窗口管理

    /// <summary>
    /// 只加载物体，不调用生命周期
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void PreLoadWindow<T>() where T : WindowBase, new()
    {
        System.Type type = typeof(T);
        string wndName = type.Name;
        T windowBase = new T();
        //克隆界面，初始化界面信息
        //1.生成对应的窗口预制体
        GameObject nWnd = LoadWindow(wndName);
        //2.初始出对应管理类
        if (nWnd != null)
        {
            windowBase.gameObject = nWnd;
            windowBase.transform = nWnd.transform;
            windowBase.Canvas = nWnd.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = mUICamera;
            windowBase.Name = nWnd.name;
            windowBase.OnAwake();
            windowBase.SetVisible(false);
            RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin = Vector2.zero;
            mAllWindowDic.Add(wndName, windowBase);
            mAllWindowList.Add(windowBase);
        }
        Debug.LogError("预加载窗口 窗口名字：" + wndName);
    }
    /// <summary>
    /// 弹出一个弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T PopUpWindow<T>() where T : WindowBase, new()
    {
        System.Type type = typeof(T);
        string wndName = type.Name;
        Debug.Log($"PopUpWindow:{wndName}");
        WindowBase wnd = GetWindow(wndName);
        if (wnd != null)
        {
            return ShowWindow(wndName) as T;
        }

        T t = new T();
        Debug.Log($"PopUpWindow new T:{t}");
        return InitializeWindow(t, wndName) as T;
    }
    private WindowBase PopUpWindow(WindowBase window)
    {
        System.Type type = window.GetType();
        string wndName = type.Name;
        WindowBase wnd = GetWindow(wndName);
        if (wnd != null)
        {
            return ShowWindow(wndName);
        }
        return InitializeWindow(window, wndName);
    }
    private WindowBase InitializeWindow(WindowBase windowBase, string wndName)
    {
        //1.生成对应的窗口预制体
        GameObject nWnd = LoadWindow(wndName);
        //2.初始出对应管理类
        if (nWnd != null)
        {
            windowBase.gameObject = nWnd;
            windowBase.transform = nWnd.transform;
            windowBase.Canvas = nWnd.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = mUICamera;
            windowBase.transform.SetAsLastSibling();
            windowBase.Name = nWnd.name;
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();
            RectTransform rectTrans = nWnd.GetComponent<RectTransform>();
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin = Vector2.zero;
            //增强代码鲁棒性 增加处理异常的健壮性
            if (mAllWindowDic.ContainsKey(wndName))
            {
                if (mAllWindowDic[wndName] != null && mAllWindowDic[wndName].gameObject != null)
                {
                    GameObjectDestoryWindow(mAllWindowDic[wndName].gameObject);
                    mAllWindowDic.Remove(wndName);
                }
                else
                    mAllWindowDic.Remove(wndName);
                if (mAllWindowList.Contains(windowBase))
                    mAllWindowList.Remove(windowBase);
                if (mVisibleWindowList.Contains(windowBase))
                    mVisibleWindowList.Remove(windowBase);
                Debug.LogError("mAllWindow Dic Alread Contains key:" + wndName);
            }

            mAllWindowDic.Add(wndName, windowBase);
            mAllWindowList.Add(windowBase);
            mVisibleWindowList.Add(windowBase);
            SetWidnowMaskVisible();
            ShowWindowAndModifyAllWindowCanvasGroup(windowBase, 0);
            return windowBase;
        }
        Debug.LogError("没有加载到对应的窗口 窗口名字：" + wndName);
        return null;
    }
    private WindowBase ShowWindow(string winName)
    {
        WindowBase window = null;
        if (mAllWindowDic.ContainsKey(winName))
        {
            window = mAllWindowDic[winName];
            if (window.gameObject != null && window.Visible == false)
            {
                mVisibleWindowList.Add(window);
                window.transform.SetAsLastSibling();
                window.SetVisible(true);
                SetWidnowMaskVisible();
                ShowWindowAndModifyAllWindowCanvasGroup(window, 0);
                window.OnShow();

            }
            //窗口若已经弹出，调用Onshow生命周期接口刷新界面数据
            else if (window.gameObject != null && window.Visible)
            {
                window.OnShow();
            }
            return window;
        }
        else
            Debug.LogError(winName + " 窗口不存在，请调用PopUpWindow 进行弹出");
        return null;
    }
    private WindowBase GetWindow(string winName)
    {
        if (mAllWindowDic.ContainsKey(winName))
        {
            return mAllWindowDic[winName];
        }
        return null;
    }

    /// <summary>
    /// 获取已经弹出的弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetWindow<T>() where T : WindowBase
    {
        System.Type type = typeof(T);
        foreach (var item in mVisibleWindowList)
        {
            if (item.Name == type.Name)
            {
                return (T)item;
            }
        }
        Debug.LogError("该窗口没有获取到：" + type.Name);
        return null;
    }

    public void HideWindow(string wndName)
    {
        WindowBase window = GetWindow(wndName);
        HideWindow(window);
    }
    public void HideWindow<T>() where T : WindowBase
    {
        HideWindow(typeof(T).Name);
    }
    private void HideWindow(WindowBase window)
    {
        if (window != null && window.Visible)
        {
            mVisibleWindowList.Remove(window);
            window.SetVisible(false);//隐藏弹窗物体
            SetWidnowMaskVisible();
            HideWindowAndModifyAllWindowCanvasGroup(window, 1);
            window.OnHide();
        }
        //在出栈的情况下，上一个界面隐藏时，自动打开栈种的下一个界面
        PopNextStackWindow(window);
    }

    private void DestroyWindow(string wndName)
    {
        WindowBase window = GetWindow(wndName);
        DestoryWindow(window);
    }
    public void DestroyWinodw<T>() where T : WindowBase
    {

        DestroyWindow(typeof(T).Name);
    }
    private void DestoryWindow(WindowBase window)
    {
        if (window != null)
        {
            if (mAllWindowDic.ContainsKey(window.Name))
            {
                mAllWindowDic.Remove(window.Name);
                mAllWindowList.Remove(window);
                mVisibleWindowList.Remove(window);
            }
            window.SetVisible(false);
            SetWidnowMaskVisible();
            if (window.Visible)
                window.OnHide();
            window.OnDestroy();
            GameObjectDestoryWindow(window.gameObject);
            //在出栈的情况下，上一个界面销毁时，自动打开栈种的下一个界面
            PopNextStackWindow(window);
            window=null;
        }
    }
    public void DestroyAllWindow(List<string> filterlist = null)
    {
        for (int i = mAllWindowList.Count - 1; i >= 0; i--)
        {
            WindowBase window = mAllWindowList[i];
            if (window == null || (filterlist != null && filterlist.Contains(window.Name)))
            {
                continue;
            }
            DestroyWindow(window.Name);
        }
        Resources.UnloadUnusedAssets();
    }

    private void SetWidnowMaskVisible()
    {
        if (UISetting.Instance==null)
        {
            Debug.LogError("UISetting.Instance is null");
            return;
        }
        if (!UISetting.Instance.SINGMASK_SYSTEM)
        {
            return;
        }
        WindowBase maxOrderWndBase = null;//最大渲染层级的窗口
        int maxOrder = 0;//最大渲染层级
        int maxIndex = 0;//最大排序下标 在相同父节点下的位置下标
        //1.关闭所有窗口的Mask 设置为不可见
        //2.从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
        for (int i = 0; i < mVisibleWindowList.Count; i++)
        {
            WindowBase window = mVisibleWindowList[i];
            if (window != null && window.gameObject != null)
            {
                window.SetMaskVisible(false);
                if (maxOrderWndBase == null)
                {
                    maxOrderWndBase = window;
                    maxOrder = window.Canvas.sortingOrder;
                    maxIndex = window.transform.GetSiblingIndex();
                }
                else
                {
                    //找到最大渲染层级的窗口，拿到它
                    if (maxOrder < window.Canvas.sortingOrder)
                    {
                        maxOrderWndBase = window;
                        maxOrder = window.Canvas.sortingOrder;
                    }
                    //如果两个窗口的渲染层级相同，就找到同节点下最靠下一个物体，优先渲染Mask
                    else if (maxOrder == window.Canvas.sortingOrder && maxIndex < window.transform.GetSiblingIndex())
                    {
                        maxOrderWndBase = window;
                        maxIndex = window.transform.GetSiblingIndex();
                    }
                }
            }
        }
        if (maxOrderWndBase != null)
        {
            maxOrderWndBase.SetMaskVisible(true);
        }
    }

    #endregion

    #region ****** Resouces 加载接口，可在下面接口中修改为自己的资源框架加载和释放接口 ******
    /// <summary>
    /// 加载弹窗(可在这个接口中替换自己的资源框架的加载对象接口)
    /// </summary>
    /// <param name="wndName"></param>
    /// <returns></returns>
    public GameObject LoadWindow(string wndName)
    {
        GameObject window = ResourcesLoadObj(mWindowConfig.GetWindowData(wndName).name, mUIRoot);
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = Vector3.zero;
        window.transform.rotation = Quaternion.identity;
        window.name = wndName;
        return window;
    }
    /// <summary>
    /// 销毁弹窗(可在这个接口中替换自己的资源框架的释放接口)
    /// </summary>
    /// <param name="windowObj"></param>
    public void GameObjectDestoryWindow(GameObject windowObj)
    {
        // GameObject.Destroy(windowObj);
        //可在这里中替换自己的资源框架的释放接口
        ZMAsset.Release(windowObj);
    }
    //*** Resouces 加载接口，可在下面接口中修改为自己的资源框架加载和释放接口  ***
    public GameObject ResourcesLoadObj(string wndName,Transform parent)
    {
          // return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
        //在这里替换成自己的资源加载框架 例:
        Debug.Log("LaodWindow:"+mWindowConfig.GetWindowData(wndName).path);
        return ZMAsset.InstantiateObject(mWindowConfig.GetWindowData(wndName).path, mUIRoot);
    }
    #endregion

    #region 智能显隐
    private void ShowWindowAndModifyAllWindowCanvasGroup(WindowBase window, int value)
    {
        if (!mSmartShowHide)
        {
            return;
        }
        //if (WorldManager.IsHallWorld && window.FullScreenWindow) 可以以此种方式决定智能显隐开启场景
        if (window.FullScreenWindow)
        {
            try
            {
                //当显示的弹窗是大厅是，不对其他弹窗进行伪隐藏，
                if (string.Equals(window.Name, "HallWindow"))
                {
                    return;
                }
                if (mVisibleWindowList.Count > 1)
                {
                    //处理先弹弹窗 后关弹窗的情况
                    WindowBase curShowBase = mVisibleWindowList[mVisibleWindowList.Count - 2];
                    if (!curShowBase.FullScreenWindow && window.Canvas.sortingOrder < curShowBase.Canvas.sortingOrder)
                    {
                        return;
                    }
                }
                for (int i = mVisibleWindowList.Count - 1; i >= 0; i--)
                {
                    WindowBase item = mVisibleWindowList[i];
                    if (item.Name != window.Name)
                    {
                        item.PseudoHidden(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error:" + ex);
            }

        }
    }

    private void HideWindowAndModifyAllWindowCanvasGroup(WindowBase window, int value)
    {
        if (!mSmartShowHide)
        {
            return;
        }
        //if (WorldManager.IsHallWorld && window.FullScreenWindow) 可以以此种方式决定智能显隐开启场景
        if (window.FullScreenWindow)
        {
            for (int i = mVisibleWindowList.Count - 1; i >= 0; i--)
            {
                if (i >= 0 && mVisibleWindowList[i] != window)
                {
                    mVisibleWindowList[i].PseudoHidden(1);
                    //找到上一个窗口，如果是全屏窗口，将其设置可见，终止循转。否则循环至最终
                    if (mVisibleWindowList[i].FullScreenWindow)
                    {
                        break;
                    }
                }
            }
        }

    }
    #endregion

    #region 堆栈系统

    /// <summary>
    /// 进栈一个界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="popCallBack"></param>
    public void PushWindowToStack<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
    {
        T wndBase = new T();
        wndBase.PopStackListener = popCallBack;
        mWindowStack.Enqueue(wndBase);
    }
    /// <summary>
    /// 弹出堆栈中第一个弹窗
    /// </summary>
    public void StartPopFirstStackWindow()
    {
        if (mStartPopStackWndStatus) return;
        mStartPopStackWndStatus = true;//已经开始进行堆栈弹出的流程，
        PopStackWindow();
    }
    /// <summary>
    /// 压入并且弹出堆栈弹窗，若已弹出则只压入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="popCallBack"></param>
    public void PushAndPopStackWindow<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
    {
        PushWindowToStack<T>(popCallBack);
        StartPopFirstStackWindow();
    }
    /// <summary>
    /// 弹出堆栈中的下一个窗口
    /// </summary>
    /// <param name="windowBase"></param>
    private void PopNextStackWindow(WindowBase windowBase)
    {
        if (windowBase != null && mStartPopStackWndStatus && windowBase.PopStack)
        {
            windowBase.PopStack = false;
            PopStackWindow();
        }
    }
    /// <summary>
    /// 弹出堆栈弹窗
    /// </summary>
    /// <returns></returns>
    public bool PopStackWindow()
    {
        if (mWindowStack.Count > 0)
        {
            WindowBase window = mWindowStack.Dequeue();
            WindowBase popWindow = PopUpWindow(window);
            popWindow.PopStackListener = window.PopStackListener;
            popWindow.PopStack = true;
            popWindow.PopStackListener?.Invoke(popWindow);
            popWindow.PopStackListener = null;
            return true;
        }
        else
        {
            mStartPopStackWndStatus = false;
            return false;
        }
    }
    public void ClearStackWindows()
    {
        mWindowStack.Clear();
    }
    #endregion

    #region 自定义Resources过度Loading

    private GameObject mGameLoadingWin;
    public void PopUpLoadingWindow()
    {
        if (mGameLoadingWin==null)
        {
            mGameLoadingWin = Object.Instantiate(Resources.Load("GameLoadingWindow")as GameObject);
        }
    }

    public void HideLoadingWindow()
    {
        if(mGameLoadingWin!=null)
                                                        Object.Destroy(mGameLoadingWin);
    }

    #endregion
}

