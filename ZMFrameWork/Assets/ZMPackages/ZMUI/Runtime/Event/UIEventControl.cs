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
/// <summary>
/// UI事件派发中心
/// 由逻辑层调用，UI层接收
/// 代替直接交互，进行解耦
/// </summary>
public class UIEventControl  
{
    /// <summary>
    /// 委托事件
    /// </summary>
    /// <param name="data"></param>
    public delegate void EventHandler(object data);
    /// <summary>
    /// 事件派发注册字典
    /// </summary>
    private static Dictionary<UIEventEnum, List<EventHandler>> mEventDic = new Dictionary<UIEventEnum, List<EventHandler>>();

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventHandler"></param>
    public static void AddEvent(UIEventEnum eventType,EventHandler eventHandler)
    {
        if (!mEventDic.ContainsKey(eventType))
        {
            mEventDic.Add(eventType,new List<EventHandler>());
        }
        if (!mEventDic[eventType].Contains(eventHandler))
        {
            mEventDic[eventType].Add(eventHandler);
        }
    }
    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventHandler"></param>
    public static void RemoveEvent(UIEventEnum eventType, EventHandler eventHandler)
    {
        if (mEventDic.ContainsKey(eventType))
        {
            if (mEventDic[eventType].Contains(eventHandler))
            {
                mEventDic[eventType].Remove(eventHandler);
            }
        }
    }
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public static void DispensEvent(UIEventEnum eventType,object data=null)
    {
        List<EventHandler> eventList = null;
        if (mEventDic.ContainsKey(eventType))
        {
            eventList = mEventDic[eventType];
        }
        for (int i = 0; i < eventList.Count; i++)
        {
            eventList[i]?.Invoke(data);
        }
    }
}
