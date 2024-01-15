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
public class ExShopWindow : MonoBehaviour
{

    public Transform itemParent;
    //游戏道具ID列表
    public List<int> itemIDlist = new List<int>();

    public List<ExShopItem> exShopItemList = new List<ExShopItem>();

    private void Awake()
    {
        ZMAssetsFrame.HotAssets(BundleModuleEnum.GameItem, null, null, null);

    }
    private void OnEnable()
    {
        itemIDlist.Clear();
        exShopItemList.Clear();
        for (int i = 0; i < 15; i++)
        {
            itemIDlist.Add(i + 6000 + 1);
        }
        
        //生成兑换道具列表
        foreach (var id in itemIDlist)
        {
            GameObject itemObj = ZMAssetsFrame.Instantiate(AssetsPathConfig.HALL_PREFAB_PATH + "ExShopItem", itemParent);
            itemObj.SetActive(true);
            ExShopItem item = itemObj.GetComponent<ExShopItem>();
            item.SetData(id);
            exShopItemList.Add(item);
        }
    }

    public void OnDisable()
    {
        foreach (var item in exShopItemList)
        {
            item.Release();
         }
    }

    public void OnCloseButtonClick()
    {
     
        ZMAssetsFrame.Release(gameObject);
    }
}
