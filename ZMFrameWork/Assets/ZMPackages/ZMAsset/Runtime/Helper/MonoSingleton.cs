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
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T mInstance = null;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = Object.FindFirstObjectByType<T>();
                    if (mInstance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
                        mInstance = obj.AddComponent<T>();
                        mInstance.OnAwake();
                        DontDestroyOnLoad(obj);
                    }
                }

                return mInstance;
            }
        }

        protected virtual void OnAwake()
        {
        }

        public virtual void Dispose()
        {
            Destroy(mInstance.gameObject);
        }
    }
}