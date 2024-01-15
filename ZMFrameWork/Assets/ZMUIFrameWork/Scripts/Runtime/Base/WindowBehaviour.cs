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
    public Action<WindowBase> PopStackListener { get; set; }
    public virtual void OnAwake() { } //只会在物体创建时执行一次 ，与Mono Awake调用时机和次数保持一致
    public virtual void OnShow() { }  //在物体显示时执行一次，与MonoOnEnable一致
    public virtual void OnUpdate() { }
    public virtual void OnHide() { } //在物体隐藏时执行一次，与Mono OnDisable 一致
    public virtual void OnDestroy() { } //在当前界面被销毁时调用一次

    public virtual void SetVisible(bool isVisble) { }  //设置物体的可见性
}
