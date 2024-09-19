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
namespace ZM.ZMAsset
{

	public class Singleton<T> where T : new()
	{
		private static T m_Instance;
		public static T Instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new T();
				}

				return m_Instance;
			}
		}

	}
}