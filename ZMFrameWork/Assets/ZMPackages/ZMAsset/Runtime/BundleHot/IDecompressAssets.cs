using System;
using System.Collections;
using System.Collections.Generic;
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
using UnityEngine;

namespace ZM.ZMAsset
{
    public abstract class IDecompressAssets
    {
        /// <summary>
        /// 需要解压的资源的总大小
        /// </summary>
        public float TotalSizem { get; protected set; }
        /// <summary>
        /// 已经解压的大小
        /// </summary>
        public float AlreadyDecompressSizem { get; protected set; }
        /// <summary>
        /// 是否开始解压
        /// </summary>
        public bool IsStartDecompress { get; protected set; }

        /// <summary>
        /// 开始解压内嵌文件
        /// </summary>
        /// <returns></returns>
        abstract public IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModule, Action callBack);
        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        abstract public float GetDecompressProgress();
    }
}