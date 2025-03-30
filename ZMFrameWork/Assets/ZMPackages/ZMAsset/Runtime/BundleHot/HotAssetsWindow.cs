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
public class HotAssetsWindow : MonoBehaviour
{
    public Slider progressSlider;
    public Text progressText;
    public Text rateText;

    private IDecompressAssets mDecompressAssets;

    private HotAssetsModule mHotAssetsModule;

    private bool LoadGameEnv = false;

    public GameObject updateNoticeObj;//更新公告总结点

    public Text updateNoticeText;//更新公告文本
    /// <summary>
    /// 显示解压文件进度
    /// </summary>
    /// <param name="decompress"></param>
    public void ShowDecompressProgress(IDecompressAssets decompress)
    {
        mDecompressAssets = decompress;
        progressText.text = "";
        progressSlider.value = 0;
    }
    /// <summary>
    /// 显示热更资源进度
    /// </summary>
    /// <param name="assetsModule"></param>
    public void ShowHotAssetsProgress(HotAssetsModule assetsModule)
    {
        mDecompressAssets = null;
        progressText.text = "";
        progressSlider.value = 0;
        mHotAssetsModule = assetsModule;
        updateNoticeObj.SetActive(true);
        updateNoticeText.text = assetsModule.UpdateNoticeContent.Replace("\\n","\n");
    }
    public void SetLoadGameEvn()
    {
        LoadGameEnv = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (LoadGameEnv)
        {
            return;
        }

        if (mDecompressAssets!=null&& progressSlider.value!=1.0f)
        {
            // Debug.Log("mDecompressAssets.GetDecompressProgress():"+ mDecompressAssets.GetDecompressProgress());
            progressText.text = "资源解压中,过程中不消耗流量...";
            progressSlider.value = mDecompressAssets.GetDecompressProgress();
        }
        if (mHotAssetsModule!=null && progressSlider.value != 1.0f)
        {
            Debug.Log("AssetsDownLoadSizeM:" + mHotAssetsModule.AssetsDownLoadSizeM + " AssetsMaxSizeM:"+ mHotAssetsModule.AssetsMaxSizeM);
            progressText.text = string.Format("资源下载中...{0}m/{1}m",mHotAssetsModule.AssetsDownLoadSizeM.ToString("F1"),mHotAssetsModule.AssetsMaxSizeM.ToString("F1"));
            progressSlider.value = mHotAssetsModule.AssetsDownLoadSizeM / mHotAssetsModule.AssetsMaxSizeM;
        }
    }
}
