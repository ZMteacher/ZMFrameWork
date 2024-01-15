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
    public class ZMFrameBase : MonoBehaviour
    {
        protected static ZMAssetsFrame _Instance = null;

        public static ZMAssetsFrame Instance
        {
            get
            {
                if (_Instance==null)
                {
                   _Instance= Object.FindObjectOfType<ZMAssetsFrame>();
                    if (_Instance==null)
                    {
                        GameObject obj = new GameObject(typeof(ZMAssetsFrame).Name);
                        //禁止销毁这个物体
                        DontDestroyOnLoad(obj);
                        _Instance=obj.AddComponent<ZMAssetsFrame>();
                        _Instance.OnInitlizate();
                    }
                }
                return _Instance;
            }
        }


        protected virtual void OnInitlizate()
        {

        }
    }
}