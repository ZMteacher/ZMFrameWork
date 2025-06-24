/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2024/3/20 12:51:13
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;
using System.Collections.Generic;
using ZM.ZMAsset;
/// <summary>
/// 当前窗口演示 资源下载中加载流程，加载资源-出现转圈动画-等待资源加载-加载完成-显示资源。
/// </summary>
public class ExShopWindow : WindowBase
{
 
    //游戏道具ID列表
    public List<int> itemIDlist = new List<int>();

    public List<ExShopItem> exShopItemList = new List<ExShopItem>();
    
    public ExShopWindowDataComponent dataCompt;

    #region 声明周期函数
    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        mDisableAnim = true;
        dataCompt = gameObject.GetComponent<ExShopWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
    }
    //物体显示时执行
    public override async void OnShow()
    {
        base.OnShow();

        itemIDlist.Clear();
        exShopItemList.Clear();
        //添加道具id
        for (int i = 0; i < 15; i++)
        {
            itemIDlist.Add(i + 6000 + 1);
        } 
        //生成兑换道具列表
        foreach (var id in itemIDlist)
        {
            AssetsRequest assets = await ZMAddressableAsset.InstantiateAsyncFormPool(AssetsPathConfig.HALL_DYNAMICITEM_PATH + "ExShopItem", null,BundleModuleEnum.GameItem);
            GameObject itemObj = assets.obj;
            itemObj.transform.SetParent(dataCompt.ContentTransform, false);
            itemObj.transform.localScale = Vector3.one;
            itemObj.SetActive(true);
            ExShopItem item = itemObj.GetComponent<ExShopItem>();
            item.SetData(assets,id);
            exShopItemList.Add(item);
        }
    }
    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
        foreach (var item in exShopItemList)
        {
            item.Release();
        }
    }
    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion
    #region API Function

    #endregion
    #region UI组件事件
    public void OnCloseButtonClick()
    {
        HideWindow();
    }
    #endregion
}
