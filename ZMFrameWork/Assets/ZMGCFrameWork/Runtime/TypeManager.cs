using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TypeManager 
{
    private static IBehaviourExecution mBehaviourExecution;
    public static void InitlizateWorldAssemblies(World world, IBehaviourExecution  behaviourExecution)
    {
        mBehaviourExecution = behaviourExecution;
        //获取Unity和我们创建的脚本所在的程序集
        Assembly[] assemblyArr= AppDomain.CurrentDomain.GetAssemblies();
        Assembly worldAssembly = null;
        //获取当前脚本运行的程序集
        foreach (var assembly in assemblyArr)
        {
            if (assembly.GetName().Name=="Assembly-CSharp")
            {
                worldAssembly = assembly;
                break;
            }
        }

        if (worldAssembly==null)
        {
            Debug.LogError("worldAssembly is Null Plase check Create Assembly!");
            return;
        }
        //先获取当前游戏世界的命名空间
        //然后获取该命名空间下的所有脚本
        //判断当前脚本是否继承了Behaviour 如果继承就是框架脚本，就需要维护创建和销毁的任务
        string NameSpace = world.GetType().Namespace;
        Type logicType = typeof(ILogicBehaviour);
        Type dataType = typeof(IDataBehaviour);
        Type msgType = typeof(IMsgBehaviour);
        //获取当前程序集下的所有的类
        Type[] typeArr= worldAssembly.GetTypes();
        List<TypeOrder> logicBehaviourList = new List<TypeOrder>();
        List<TypeOrder> dataBehaviourList = new List<TypeOrder>();
        List<TypeOrder> msgBehaviourList = new List<TypeOrder>();
        foreach (var type in typeArr)
        {
            string space = type.Namespace;
            if (type.Namespace==NameSpace)
            {
                if (type.IsAbstract)
                    continue;
                if (logicType.IsAssignableFrom(type))
                {
                    //获取当前类的初始化顺序
                    int order = GetLogicBehaviourOrderIndex(type);
                    TypeOrder typeOrder = new TypeOrder(order,type);
                    logicBehaviourList.Add(typeOrder);

                }
                else if (dataType.IsAssignableFrom(type))
                {
                    int order = GetDataBehaviourOrderIndex(type);
                    TypeOrder typeOrder = new TypeOrder(order, type);
                    dataBehaviourList.Add(typeOrder);
                }
                else if (msgType.IsAssignableFrom(type))
                {
                    int order = GetMsgBehaviourOrderIndex(type);
                    TypeOrder typeOrder = new TypeOrder(order, type);
                    msgBehaviourList.Add(typeOrder);
                }
            }
        }
        //最的小的排在前面
        logicBehaviourList.Sort((a,b)=>a.order.CompareTo(b.order));
        dataBehaviourList.Sort((a, b) => a.order.CompareTo(b.order));
        msgBehaviourList.Sort((a, b) => a.order.CompareTo(b.order));

        //初始化数据层脚本、消息层脚本、逻辑层脚本
        for (int i = 0; i < dataBehaviourList.Count; i++)
        {
           IDataBehaviour data= Activator.CreateInstance(dataBehaviourList[i].type) as IDataBehaviour;
            world.AddDataMgr(data);
        }
        for (int i = 0; i < msgBehaviourList.Count; i++)
        {
            IMsgBehaviour msg = Activator.CreateInstance(msgBehaviourList[i].type) as IMsgBehaviour;
            world.AddMsgMgr(msg);
        }
        for (int i = 0; i < logicBehaviourList.Count; i++)
        {
            ILogicBehaviour logic = Activator.CreateInstance(logicBehaviourList[i].type) as ILogicBehaviour;
            world.AddLogicCtrl(logic);
        }

        logicBehaviourList.Clear();
        dataBehaviourList.Clear();
        msgBehaviourList.Clear();
        mBehaviourExecution = null;
    }

    private static int GetLogicBehaviourOrderIndex(Type type)
    {
        if (mBehaviourExecution==null)
            return 999;

        Type[] logicTypes = mBehaviourExecution.GetLogicBehaviourExecution();
        for (int i = 0; i < logicTypes.Length; i++)
        {
            if (logicTypes[i]==type)
                return i;
        }
        return 999;
    }
    private static int GetDataBehaviourOrderIndex(Type dataType)
    {
        if (mBehaviourExecution == null)
            return 999;
        Type[] dataTypes = mBehaviourExecution.GetDataBehaviourExecution();
        for (int i = 0; i < dataTypes.Length; i++)
        {
            if (dataTypes[i] == dataType)
                return i;
        }
        return 999;
    }
    private static int GetMsgBehaviourOrderIndex(Type msgType)
    {
        if (mBehaviourExecution == null)
            return 999;
        Type[] msgTypes = mBehaviourExecution.GetMsgBehaviourExecution();
        for (int i = 0; i < msgTypes.Length; i++)
        {
            if (msgTypes[i] == msgType)
                return i;
        }
        return 999;
    }
}
