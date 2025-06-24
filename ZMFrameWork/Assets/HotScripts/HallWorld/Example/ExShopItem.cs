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

    private GameObject m_ItemObj;
    private int mItemId;
    private AssetsRequest m_AssetsRequest;
    private AssetsRequest m_itemAsset;
    public async void SetData(AssetsRequest itemAsset, int itemid)
    {
        mItemId = itemid;
        m_itemAsset = itemAsset;
        ItemObjLoading();
        AssetsRequest assets = await ZMAddressableAsset.InstantiateAsyncFormPool(AssetsPathConfig.GAME_DATA_PATH + "GameItem/" + itemid + "/" + itemid,
            null,BundleModuleEnum.GameItem,itemid);
        if ((int)assets.param1 == mItemId && assets.obj != null)
        {
            m_AssetsRequest = assets;
            m_ItemObj = assets.obj;
            m_ItemObj.SetActive(true);
            m_ItemObj.transform.SetParent(gameItemParent);
            m_ItemObj.transform.localPosition = Vector3.zero;
            m_ItemObj.transform.localScale = Vector3.one;
            m_ItemObj.transform.rotation = Quaternion.identity;
            loadingObj.SetActive(false);
        }
        else
        {
            assets.Release();
            assets=null;
        }
    }

    public void ItemObjLoading()
    {
        loadingObj.SetActive(true);
    }

    public void Release()
    {
        m_AssetsRequest?.Release();
        m_itemAsset?.Release();
        m_AssetsRequest = null;
        m_itemAsset = null;
    }

}
