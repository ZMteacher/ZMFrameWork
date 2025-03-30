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
using UnityEngine;
using ZM.ZMAsset;
/// <summary>
/// 当前脚本演示 资源下载中加载流程，加载资源-出现转圈动画-等待资源加载-加载完成-显示资源。
/// </summary>
public class ExShopItem : MonoBehaviour
{
    public Transform gameItemParent;
    public GameObject loadingObj;

    private GameObject mItemObj;
    private int mItemId;
    public void SetData(int itemid)
    {
        mItemId = itemid;
        ZMAsset.InstantiateAndLoad(AssetsPathConfig.GAME_DATA_PATH + "GameItem/" + itemid + "/" + itemid, LoadItemObjComplete, ItemObjLoading);
    }
    /// <summary>
    /// 加载游戏道具完成
    /// </summary>
    /// <param name="itemObj"></param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    public void LoadItemObjComplete(GameObject itemObj, object param1, object param2)
    {
        Debug.Log("LoadItemObjComplete:" + mItemId);
        loadingObj.SetActive(false);
        if (itemObj != null)
        {
            itemObj.SetActive(true);
            itemObj.transform.SetParent(gameItemParent);
            itemObj.transform.localPosition = Vector3.zero;
            itemObj.transform.localScale = Vector3.one;
            itemObj.transform.rotation = Quaternion.identity;
            mItemObj = itemObj;
        }
        else
        {
            Debug.Log("item obj  is Null:" + mItemId);
        }
    }

    public void ItemObjLoading()
    {
        loadingObj.SetActive(true);
    }

    public void Release()
    {
        if (mItemObj != null)
        {
            ZMAsset.Release(mItemObj, true);
        }
        ZMAsset.Release(gameObject,true);
     }

}
