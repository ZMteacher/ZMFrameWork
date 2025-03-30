using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public enum WorldEnum
{
    LoginWorld,
    HallWorld,
    SKWorld,
    WZWorld,
}
/// <summary>
/// 世界管理器
/// </summary>
public class WorldManager
{
    /// <summary>
    /// 构建状态
    /// </summary>
    public static bool Builder { get; private set; }
    /// <summary>
    /// 所有已构建出的世界列表
    /// </summary>
    private static List<World> mWorldList = new List<World>();
    /// <summary>
    /// 世界更新程序
    /// </summary>
    public static WorldUpdater WorldUpdater { get; private set; }
    /// <summary>
    /// 默认游戏世界
    /// </summary>
    public static World DefaultGameWorld { get; private set; }
    /// <summary>
    /// 当前游戏世界
    /// </summary>
    public static WorldEnum CurWorldEnum { get; private set; }
    
    /// <summary>
    /// 构建世界成功回调
    /// </summary>
    public static Action<WorldEnum> OnCreateWorldSuccessListener;
    
    /// <summary>
    /// 构建一个游戏世界
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void CreateWorld<T>() where T : World, new()
    {
        if (string.Equals(CurWorldEnum.ToString(), typeof(T).Name))
        {
            Debug.LogError($"重复构建游戏世界 curWorldEnum:{CurWorldEnum}，WroldName:{typeof(T).Name}");
            return;
        }
        T world = new T();
        //首个创建的世界为默认世界，按照目前的HallWorld常驻内存的设计，DefaultGameWorld一直HallWorld。
        if (DefaultGameWorld == null)
        {
            DefaultGameWorld = world;
        }
        //初始化当前游戏世界的程序集脚本
        WorldTypeManager.InitializeWorldAssemblies(world, GetBehaviourExecution(world));
        world.OnCreate();
        mWorldList.Add(world);
        OnCreateWorldSuccessListener?.Invoke(CurWorldEnum);

        if (!Builder)
            InitWorldUpdater();
        Builder = true;
    }
    
    /// <summary>
    /// Suppert HyBridCLR通过反射的方式构建对应世界
    /// </summary>
    /// <param name="worldFullName"></param>
    public static void CreateWorldByReflection(Assembly assembly, string worldFullName="ZMGC.SK.SKWorld")
    {
        Debug.Log($"Start CallCreateWorldAcrossAssemblies worldFullName:{worldFullName}");
        
        // 1. 获取类型
        Type worldType = assembly.GetType(worldFullName);
        Type worldManager = Type.GetType("WorldManager");
        Debug.Log($"worldType: {worldType}  worldManager: {worldManager}");
        
        // 2.获取并构造泛型方法
        MethodInfo createWorldMethod = worldManager.GetMethod("CreateWorld");
        MethodInfo genericMethod = createWorldMethod.MakeGenericMethod(worldType);
        
        // 3 调用方法
        object world = genericMethod.Invoke(null, null);
        Debug.Log($"成功创建World<{worldType.Name}>实例");
    }
    /// <summary>
    /// 获取对应世界下指定的脚本创建优先级
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static IBehaviourExecution GetBehaviourExecution(World world)
    {
        if (world.GetType().Name == "HallWorld")
        {
            CurWorldEnum = WorldEnum.HallWorld;
            return new HallWorldScriptExecutionOrder();
        }
        else if (world.GetType().Name == "SKWorld")
        {
            CurWorldEnum = WorldEnum.SKWorld;
            return new HallWorldScriptExecutionOrder();
        }
        return null;
    }
    /// <summary>
    /// 渲染帧更新,尽量少使用Update接口提升性能。但必要时，可以在对应World的Update中调用指定脚本的Update
    /// </summary>
    public static void Update()
    {
        //帧更新接口，若需要使用请在对应的xxxWorld脚本中实现OnUpdate接口，自行去调用逻辑层/数据层/消息层的帧更新接口
        //不制作方便使用的自动化方式执行Update的原因是为了保障性能，防止对Update接口的滥用，影响性能。
        //故不太建议在逻辑层/数据层/消息层去使用Update帧更新接口。特殊需求情况除外
        for (int i = 0; i < mWorldList.Count; i++)
        {
            mWorldList[i].OnUpdate();
        }
    }
    /// <summary>
    /// 初始化世界更新程序
    /// 
    /// </summary>
    public static void InitWorldUpdater()
    {
        GameObject worldObj = new GameObject("WorldUpdater");
        WorldUpdater = worldObj.AddComponent<WorldUpdater>();
        GameObject.DontDestroyOnLoad(worldObj);
    }


    /// <summary>
    /// 销毁指定游戏世界
    /// </summary>
    /// <typeparam name="T">要销毁的世界</typeparam>
    /// <param name="args">销毁后传出的参数，建议自定义class结构体，统一传出和管理</param>
    public static void DestroyWorld<T>(object args = null) where T : World
    {
        for (int i = 0; i < mWorldList.Count; i++)
        {
            World world= mWorldList[i];
            if (world.GetType().Name == typeof(T).Name)
            {
                world.DestroyWorld(typeof(T).Namespace, args);
                mWorldList.Remove(mWorldList[i]);
                //重设当前所处世界
                GetBehaviourExecution(DefaultGameWorld);
                //触发销毁后处理，可在对应接口中返回其他世界
                world.OnDestroyPostProcess(args);
                break;
            }
        }
    }
    
}
