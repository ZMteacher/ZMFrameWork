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
using UnityEngine.UI;
using ZM.ZMAsset;
using ZMGC.Battle;
using ZMGC.SK;
 
using ZMGC.WZ;

public class GameModeItem : MonoBehaviour
{
    public Button button;
    public Image downSliderIamge;
    public Text downLoadSpeedText;//下载速度 3m/s
    public Text downLoadRatioText;//下载百分比进度  60%
    public Text downLoadProgressText;//下载进度 30m/100m
    public Text downLoadTips;//开始下载的提示
    public GameObject updateRoot; //热更总结点

    public BundleModuleEnum gameType;

    private HotAssetsModule mHotModule;
    private float lastTime;
    private float lastDownLoadSize;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnGameButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (mHotModule!=null)
        {
            downLoadProgressText.text = string.Format("{0}m/{1}m",
                mHotModule.AssetsDownLoadSizeM.ToString("F0"),mHotModule.AssetsMaxSizeM.ToString("F0"));
            downLoadRatioText.text = (mHotModule.AssetsDownLoadSizeM / mHotModule.AssetsMaxSizeM * 100).ToString("F0") + "%";
            downSliderIamge.fillAmount = mHotModule.AssetsDownLoadSizeM / mHotModule.AssetsMaxSizeM;
            if (Time.realtimeSinceStartup-lastTime>1)
            {
                downLoadSpeedText.text = (mHotModule.AssetsDownLoadSizeM - lastDownLoadSize).ToString("F1") + "m/s";
                lastDownLoadSize = mHotModule.AssetsDownLoadSizeM;
                lastTime = Time.realtimeSinceStartup;
            }
        }
    }
    public void OnGameButtonClick()
    {
        ZMAsset.CheckAssetsVersion(gameType, CheckAssetCallBack);
    }
    public void CheckAssetCallBack(bool isHot,float sizem)
    {
        //如果说需要热更，我们就去下载该模块的热更资源
        if (isHot)
        {
            ZMAsset.HotAssets(gameType, OnStartHotAssets, OnHotAssetsFinish, OnHotAssetsWait);
        }
        else
        {
            //如果不需要热更，就可以直接加载对应模块资源进入游戏
            UIModule.Instance.DestroyAllWindow();
            ZMAsset.ClearResourcesAssets(true);
            //不销毁大厅，大厅一般常驻内存
            if (gameType== BundleModuleEnum.ShuangKou)
            {
                WorldManager.CreateWorld<SKWorld>();
            }
            else if (gameType == BundleModuleEnum.WuZhang)
            {
                WorldManager.CreateWorld<WZWorld>();
            }
        } 
    }
    /// <summary>
    /// 开始热更
    /// </summary>
    /// <param name="moduleType"></param>
    public void OnStartHotAssets(BundleModuleEnum moduleType)
    {
        updateRoot.SetActive(true);
        downLoadTips.text = "正在更新";
        mHotModule = ZMAsset.GetHotAssetsModule(moduleType);
    }
    /// <summary>
    /// 热更完成
    /// </summary>
    /// <param name="moduleType"></param>
    public void OnHotAssetsFinish(BundleModuleEnum moduleType)
    {
        mHotModule = null;
        updateRoot.SetActive(false);
        downLoadTips.text = "更新完成";
        Debug.Log("资源热更完成："+moduleType);
    }
    /// <summary>
    /// 热更等待中
    /// </summary>
    /// <param name="moduleType"></param>
    public void OnHotAssetsWait(BundleModuleEnum moduleType)
    {
        updateRoot.SetActive(true);
        downLoadTips.text = "等待更新";
    }
}
