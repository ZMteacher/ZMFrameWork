/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAsset
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AssetsPathConfig  
{
   
    public const string GAME_DATA_PATH="Assets/GameData/";
    public const string GAME_ITEM_PATH = GAME_DATA_PATH + "GameItem/";
    
    
    public const string HALL_PATH = GAME_DATA_PATH + "Hall/";
    public const string HALL_PREFABS_PATH = HALL_PATH + "Prefabs/";
    public const string HALL_DYNAMICITEM_PATH = HALL_PREFABS_PATH + "DynamicItem/";
    public const string HALL_HOTFIXDLL_PATH = HALL_PATH + "HotFixDll/";
 



    public const string HALL_TEXTURE_PATH = HALL_PATH + "Textures/";

    public const string HALL_DATA_PATH = HALL_PATH+ "CfgData/";
}
