using System.Collections;
using System.Collections.Generic;
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
    private static List<World> mWorldList = new List<World>();
    /// <summary>
    /// 默认游戏世界
    /// </summary>
    public static World DefaultGameWorld { get; private set; }
    /// <summary>
    /// 当前游戏世界
    /// </summary>
    public static WorldEnum CurWorld { get; private set; }
    /// <summary>
    /// 是否大厅世界
    /// </summary>
    public static bool IsHallWorld { get { return CurWorld == WorldEnum.HallWorld; } }    
    /// <summary>
    /// 是否游戏世界
    /// </summary>
    public static bool IsBattleWorld { get { return CurWorld == WorldEnum.BattleWorld; } }
    /// <summary>
    /// 是否双扣游戏世界
    /// </summary>
    public static bool IsSKWorld { get { return CurWorld == WorldEnum.SKWorld; } }


    /// <summary>
    /// 构建一个游戏世界
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void CreateWorld<T>() where T: World,new ()
    {
        T world = new T();
        DefaultGameWorld = world;
        //初始化当前游戏世界的程序集脚本
        TypeManager.InitlizateWorldAssemblies(world, GetBehaviourExecution(world));
        world.OnCretae();
        mWorldList.Add(world);
    }
    /// <summary>
    /// 获取对应世界下指定的脚本创建优先级
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static  IBehaviourExecution GetBehaviourExecution(World world)
    {
        if (world.GetType().Name=="HallWorld")
        {
            CurWorld = WorldEnum.HallWorld;
            return new HallWorldScriptExecutionOrder();
        }
        if (world.GetType().Name == "BattleWorld")
        {
            CurWorld = WorldEnum.BattleWorld;
            return new HallWorldScriptExecutionOrder();
        }
        if (world.GetType().Name == "SKWorld")
        {
            CurWorld = WorldEnum.SKWorld;
            return new HallWorldScriptExecutionOrder();
        }
        return null;
    }

    /// <summary>
    /// 销毁指定游戏世界
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="world"></param>
    public static void DestroyWorld<T>()where T:World
    {
        for (int i = 0; i < mWorldList.Count; i++)
        {
            if (mWorldList[i].GetType().Name == typeof(T).Name)
            {
                mWorldList[i].DestoryWorld(typeof(T).Namespace);
                mWorldList.Remove(mWorldList[i]);
                break;
            }
        }
    }
}
