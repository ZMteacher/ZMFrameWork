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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class WindowBehaviour
{
    public GameObject gameObject { get; set; } //当前窗口物体
    public Transform transform { get; set; } //代表自己
    public Canvas Canvas { get; set; }
    public string Name { get; set; }
    public bool Visible { get; set; }
    public bool PopStack { get; set; }//是否是通过堆栈系统弹出的弹窗
    /// <summary>
    /// 全屏窗口标志(在窗口Awake接口中进行设置,智能显隐开启后当全屏弹窗弹出时，被遮挡的窗口都会通过伪隐藏隐藏掉，从而提升性能)
    /// </summary>
    public bool FullScreenWindow { get; set; }

    public Action<WindowBase> PopStackListener { get; set; }

    public virtual void OnAwake() { } //只会在物体创建时执行一次 ，与Mono Awake调用时机和次数保持一致
    public virtual void OnShow() { }  //在物体显示时执行一次，与MonoOnEnable一致
    public virtual void OnUpdate() { }
    public virtual void OnHide() { } //在物体隐藏时执行一次，与Mono OnDisable 一致
    public virtual void OnDestroy() { } //在当前界面被销毁时调用一次

    public virtual void SetVisible(bool isVisble) { }  //设置物体的可见性
}
