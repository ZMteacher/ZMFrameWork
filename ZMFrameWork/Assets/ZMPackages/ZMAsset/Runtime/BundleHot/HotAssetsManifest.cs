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

namespace ZM.ZMAsset
{
    /// <summary>
    /// 热更资源清单
    /// </summary>
    public class HotAssetsManifest
    {
        /// <summary>
        /// 热更公告
        /// </summary>
        public string updateNotice;
        /// <summary>
        /// 下载地址
        /// </summary>
        public string downLoadURL;
        /// <summary>
        /// 热更资源补丁列表
        /// </summary>
        public List<HotAssetsPatch> hotAssetsPatchList = new List<HotAssetsPatch>();
    }
    /// <summary>
    /// 热更资源补丁
    /// </summary>
    public class HotAssetsPatch
    {
        /// <summary>
        /// 补丁版本
        /// </summary>
        public int patchVersion;
        /// <summary>
        /// 热更资源信息列表
        /// </summary>
        public List<HotFileInfo> hotAssetsList = new List<HotFileInfo>();
    }

    /// <summary>
    /// 热更文件信息
    /// </summary>
    public class HotFileInfo
    {
        public string abName;//AssetBundle名字

        public string md5;//文件的Md5

        public float size;//文件的大小
    }
}