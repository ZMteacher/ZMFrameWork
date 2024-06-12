using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum WorldEnum
{
    LoginWorld,
    HallWorld,
    BattleWorld,
    SKWorld,
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
    /// 是否大厅世界
    /// </summary>
    public static bool IsHallWorld { get { return CurWorldEnum == WorldEnum.HallWorld; } }
    /// <summary>
    /// 是否游戏世界
    /// </summary>
    public static bool IsBattleWorld { get { return CurWorldEnum == WorldEnum.BattleWorld; } }
    /// <summary>
    /// 是否双扣游戏世界
    /// </summary>
    public static bool IsSKWorld { get { return CurWorldEnum == WorldEnum.SKWorld; } }

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
        DefaultGameWorld = world;
        //初始化当前游戏世界的程序集脚本
        TypeManager.InitlizateWorldAssemblies(world, GetBehaviourExecution(world));
        world.OnCretae();
        mWorldList.Add(world);
        OnCreateWorldSuccessListener?.Invoke(CurWorldEnum);

        if (!Builder)
            InitWorldUpdater();
        Builder = true;
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
        if (world.GetType().Name == "BattleWorld")
        {
            CurWorldEnum = WorldEnum.BattleWorld;
            return new HallWorldScriptExecutionOrder();
        }
        if (world.GetType().Name == "SKWorld")
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
        //Debug.Log("WorldManger Update");
        for (int i = 0; i < mWorldList.Count; i++)
        {
            mWorldList[i].OnUpdate();
        }
    }
    /// <summary>
    /// 初始化世界更新程序
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
            if (mWorldList[i].GetType().Name == typeof(T).Name)
            {
                mWorldList[i].DestoryWorld(typeof(T).Namespace, args);
                mWorldList.Remove(mWorldList[i]);
                break;
            }
        }
    }
}
