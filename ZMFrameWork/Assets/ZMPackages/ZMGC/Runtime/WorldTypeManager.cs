using System;
using System.Collections.Generic;
using System.Reflection;

public class WorldTypeManager 
{
    private static IBehaviourExecution m_BehaviourExecution;
    public static void InitializeWorldAssemblies(World world, IBehaviourExecution  behaviourExecution)
    {
        m_BehaviourExecution = behaviourExecution;
        
        //获取当前脚本运行的程序集 这种方式能自动识别任何自定义程序集的World脚本
        Assembly worldAssembly = world.GetType().Assembly;
        
        //先获取当前游戏世界的命名空间
        //然后获取该命名空间下的所有脚本
        //判断当前脚本是否继承了Behaviour 如果继承就是框架脚本，就需要维护创建和销毁的任务
        string nameSpace = world.GetType().Namespace;
        Type logicType = typeof(ILogicBehaviour);
        Type dataType = typeof(IDataBehaviour);
        Type msgType = typeof(IMsgBehaviour);
        //获取当前程序集下的所有的类
        Type[] typeArr= worldAssembly.GetTypes();
        List<TypeOrder> logicBehaviourList =  new List<TypeOrder>();
        List<TypeOrder> dataBehaviourList = new List<TypeOrder>();
        List<TypeOrder> msgBehaviourList = new List<TypeOrder>();
        foreach (var type in typeArr)
        {
            if (!string.Equals(type.Namespace,nameSpace)||type.IsAbstract)
                continue;
 
            if (logicType.IsAssignableFrom(type))
            {
                logicBehaviourList.Add( new TypeOrder(GetLogicBehaviourOrderIndex(type.Name),type)); //获取当前类的初始化顺序
            }
            else if (dataType.IsAssignableFrom(type))
            {
                dataBehaviourList.Add(new TypeOrder(GetDataBehaviourOrderIndex(type.Name), type));
            }
            else if (msgType.IsAssignableFrom(type))
            {
                msgBehaviourList.Add(new TypeOrder(GetMsgBehaviourOrderIndex(type.Name), type));
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
        m_BehaviourExecution = null;
    }

    private static int GetLogicBehaviourOrderIndex(string typeName)
    {
        if (m_BehaviourExecution==null)
            return 999;

        string[] logicTypes = m_BehaviourExecution.GetLogicBehaviourExecution();
        for (int i = 0; i < logicTypes.Length; i++)
        {
            if (logicTypes[i]==typeName)
                return i;
        }
        return 999;
    }
    private static int GetDataBehaviourOrderIndex(string typeName)
    {
        if (m_BehaviourExecution == null)
            return 999;
        string[] dataTypes = m_BehaviourExecution.GetDataBehaviourExecution();
        for (int i = 0; i < dataTypes.Length; i++)
        {
            if (dataTypes[i] == typeName)
                return i;
        }
        return 999;
    }
    private static int GetMsgBehaviourOrderIndex(string typeName)
    {
        if (m_BehaviourExecution == null)
            return 999;
        string[] msgTypes = m_BehaviourExecution.GetMsgBehaviourExecution();
        for (int i = 0; i < msgTypes.Length; i++)
        {
            if (msgTypes[i] == typeName)
                return i;
        }
        return 999;
    }
}
