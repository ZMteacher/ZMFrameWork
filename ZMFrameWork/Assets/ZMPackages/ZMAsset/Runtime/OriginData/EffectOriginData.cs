using System.Collections;
using System.Collections.Generic;
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
	
	public class EffectOriginData : OriginData
	{
	    public ParticleSystem[] m_Particle;
	    public TrailRenderer[] m_TrailRe;
	
	    public override void ResetOriginData()
	    {
	        base.ResetOriginData();
	        foreach (ParticleSystem particle in m_Particle)
	        {
	            particle.Clear(true);
	            particle.Play();
	        }
	
	        foreach (TrailRenderer trail in m_TrailRe)
	        {
	            trail.Clear();
	        }
	    }
	
	    public override void BindData()
	    {
	        base.BindData();
	        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
	        m_TrailRe = gameObject.GetComponentsInChildren<TrailRenderer>(true);
	    }
	}
}
