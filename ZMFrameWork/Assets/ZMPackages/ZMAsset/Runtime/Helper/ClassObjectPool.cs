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
    public class ClassObjectPool<T> where T : class, new()
    {
        /// <summary>
        /// 存放类的一个对象池，偏底层的东西 尽量别使用list 
        /// </summary>
        protected Stack<T> mPool = new Stack<T>();
        /// <summary>
        /// 最大的缓存对象个数 小于等于0表示不限个数
        /// </summary>
        protected int mMaxCount = 0;

        public int PoolCount { get { return mPool.Count; } }
        public ClassObjectPool(int maxCount)
        {
            mMaxCount = maxCount;
            for (int i = 0; i < maxCount; i++)
            {
                mPool.Push(new T());
            }
        }
        /// <summary>
        /// 取出对象
        /// </summary>
        /// <returns></returns>
        public T Spawn()
        {
            if (mPool.Count>0)
            {
                return mPool.Pop();
            }
            else
            {
                return new T();
            }
        }
        /// <summary>
        /// 回收类对象
        /// </summary>
        /// <param name="obj"></param>
        public void Recycl(T obj)
        {
            if (obj==null)
            {
                Debug.LogError("Recycl Obj failed,obj is null!");
                return;
            }
            mPool.Push(obj);
        }
    }
}