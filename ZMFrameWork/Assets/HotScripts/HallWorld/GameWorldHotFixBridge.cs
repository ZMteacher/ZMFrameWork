/*----------------------------------------------------------------------------
* Title: 帧同步定点数学库
*
* Author: 铸梦
*
* Date: 2025.02.20
*
* Description:基于定点数实现的一套AABB定点数学物理碰撞库，可用于客户端和服务端。
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 案例地址：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZM.ZMAsset;

public class GameWorldHotFixBridge
{
    /// <summary>
    /// 所有游戏热更程序集(程序集只允许加载一次，且无法卸载)
    /// </summary>
    private static Dictionary<WorldEnum, Assembly> m_HotFixAssemblyDic = new Dictionary<WorldEnum, Assembly>();
    
    /// <summary>
    /// 加载热更世界
    /// </summary>
    /// <param name="assemblyPath">热更程序集路径</param>
    /// <param name="worldEnum">游戏世界类型</param>
    public static async void OnLoadHotFixWorld(WorldEnum worldEnum,string nameSpace,string assemblyPath)
    { 
        m_HotFixAssemblyDic.TryGetValue(worldEnum, out var assembly);
        if (assembly == null)
        {
          
#if !UNITY_EDITOR
            assembly = Assembly.Load((await ZMAsset.LoadTextAssetAsync(assemblyPath)).bytes);
            await HotFixEntry.LoadAOTGenericMetadata();
            m_HotFixAssemblyDic.Add(worldEnum,assembly);
#else
            assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == worldEnum.ToString());
#endif
        }
        //构建游戏世界
        WorldManager.CreateWorldByReflection(assembly,$"{nameSpace}.{worldEnum.ToString()}");
    }
}
