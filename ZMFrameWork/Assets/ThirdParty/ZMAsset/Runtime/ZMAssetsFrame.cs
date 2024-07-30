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
namespace ZM.AssetFrameWork
{
    public partial class ZMAsset : ZMFrameBase
    {
        public static Transform RecyclObjPool { get; private set; }

        private IHotAssets mHotAssets = null;

        private IResourceInterface mResource = null;

        private IDecompressAssets mDecompressAssets = null;
        /// <summary>
        /// 初始化框架
        /// </summary>
        private void Initialize()
        {
            GameObject recyclObjectRoot = new GameObject("RecyclObjPool");
            RecyclObjPool = recyclObjectRoot.transform;
            recyclObjectRoot.SetActive(false);
            DontDestroyOnLoad(recyclObjectRoot);
            mHotAssets = new HotAssetsManager();
            mDecompressAssets =new  AssetsDecompressManager();
            var resource=new ResourceManager();
            mResource = resource;
            ZMAddressableAsset.Interface = resource;
            mResource.Initlizate();
        }

        public void Update()
        {
            mHotAssets?.OnMainThreadUpdate();
        }
        private void OnApplicationQuit()
        {
            mResource.ClearResourcesAssets(true);
        }

    }
}