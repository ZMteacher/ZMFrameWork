using UnityEngine;
/*********************************************
*
* Title: ZM 资源管理框架 请关注资源接口 AssetsManager 
*
*Description: 集多版本、多模块热更、回退、加密、解密、压缩、内嵌、解压、下载、Editor加载、Bundle加载、对象池系统、冗余剔除、多版本颗粒化、引用计数、内存自动管理优化、等一体式框架
*
* Author: ZM
*
* Date: 2019.7.1
*
* Modify: 
 *
 * ********************************************/ 
namespace ZM.ZMAsset
 {
	
	public class OriginData : MonoBehaviour
	{
	    public Rigidbody m_Rigidbody;
	    public Collider m_Collider;
	    public Transform[] m_AllPoint;
	    public int[] m_AllPointChildCount;
	    public bool[] m_AllPointActive;
	    public Vector3[] m_Pos;
	    public Vector3[] m_Scale;
	    public Quaternion[] m_Rot;
	
	    /// <summary>
	    /// 还原属性
	    /// </summary>
	    public virtual void ResetOriginData()
	    {
	        int allPointCount = m_AllPoint.Length;
	        for (int i = 0; i < allPointCount; i++)
	        {
	            Transform tempTrs = m_AllPoint[i];
	            if (tempTrs != null)
	            {
	                tempTrs.localPosition = m_Pos[i];
	                tempTrs.localRotation = m_Rot[i];
	                tempTrs.localScale = m_Scale[i];
					
	
	                if (m_AllPointActive[i])
	                {
	                    if (!tempTrs.gameObject.activeSelf)
	                    {
	                        tempTrs.gameObject.SetActive(true);
	                    }
	                }
	                else
	                {
	                    if (tempTrs.gameObject.activeSelf)
	                    {
	                        tempTrs.gameObject.SetActive(false);
	                    }
	                }
	            }
	        }
	    }
	
	    /// <summary>
	    /// 编辑器下保存初始数据
	    /// </summary>
	    public virtual void BindData()
	    {
	        m_Collider = gameObject.GetComponentInChildren<Collider>(true);
	        m_Rigidbody = gameObject.GetComponentInChildren<Rigidbody>(true);
	        m_AllPoint = gameObject.GetComponentsInChildren<Transform>(true);
	        int allPointCount = m_AllPoint.Length;
	        m_AllPointChildCount = new int[allPointCount];
	        m_AllPointActive = new bool[allPointCount];
	        m_Pos = new Vector3[allPointCount];
	        m_Rot = new Quaternion[allPointCount];
	        m_Scale = new Vector3[allPointCount];
	        for (int i = 0; i < allPointCount; i++)
	        {
	            Transform temp = m_AllPoint[i] as Transform;
	            m_AllPointChildCount[i] = temp.childCount;
	            m_AllPointActive[i] = temp.gameObject.activeSelf;
	            m_Pos[i] = temp.localPosition;
	            m_Rot[i] = temp.localRotation;
	            m_Scale[i] = temp.localScale;
	        }
	    }
	}
}
