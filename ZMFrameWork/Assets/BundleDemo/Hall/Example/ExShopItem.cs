/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAssetFrameWork
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
using ZM.AssetFrameWork;
public class ExShopItem : MonoBehaviour
{
    public Transform gameItemParent;
    public GameObject loadingObj;

    private GameObject mItemObj;
    private int mItemId;
    public void SetData(int itemid)
    {
        mItemId = itemid;
        ZMAssetsFrame.InstantiateAndLoad("Assets/BundleDemo/GameItem/" + itemid + "/" + itemid, LoadItemObjComplete, ItemObjLoading);
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
            ZMAssetsFrame.Release(mItemObj, true);
        }
        ZMAssetsFrame.Release(gameObject, true);
    }

}
