/*----------------------------------------------------------------------------
* Title: #Title#
*
* Author: 铸梦
*
* Date: #CreateTime#
*
* Description:
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com
----------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnalysisComponentDataTool  
{
    /// <summary>
    /// 解析窗口节点数据
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="WinName"></param>
    public static void  AnalysisWindowNodeData(ref List<EditorObjectData> objDataList, Transform trans, string WinName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            string name = obj.name;
            
            if (name.Contains("#"))    continue;
            
            if (name.Contains("[") && name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1;
                string fieldName = name.Substring(index, name.Length - index);//获取字段昵称
                string fieldType = name.Substring(1, index - 2);//获取字段类型
                var objectData = new EditorObjectData { fieldName = fieldName, fieldType = fieldType, insID = obj.GetInstanceID() };
                objDataList.Add(objectData);
                //处理列表元素绑定
                if (fieldType.Contains(","))
                {
                    objectData.dataList = new List<EditorObjectData>();
                    objectData.fieldType = objectData.fieldType.Replace(",", "");
                    for (int j = 0; j < obj.transform.childCount; j++) 
                    {
                        GameObject listObjItme = obj.transform.GetChild(j).gameObject;
                        objectData.dataList.Add(new EditorObjectData { fieldName = listObjItme.name.Replace("#",""),  insID = listObjItme.GetInstanceID()});
                    }
                }
            }
            AnalysisWindowNodeData(ref objDataList,trans.GetChild(i), WinName);
        }
    }
    /// <summary>
    /// 解析窗口Tag数据
    /// </summary>
    /// <param name="objDataList"></param>
    /// <param name="trans"></param>
    /// <param name="WinName"></param>
    public static void AnalysisWindowDataByTag(ref List<EditorObjectData> objDataList,Transform trans, string WinName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            
            if (obj.name.Contains("#"))    continue;
            
            string tagName = obj.tag;
            
            if (GeneratorConfig.TAGArr.Contains(tagName))
            {
                string fieldName = obj.name;//获取字段昵称
                string fieldType = tagName;//获取字段类型
                objDataList.Add(new EditorObjectData { fieldName = fieldName, fieldType = fieldType, insID = obj.GetInstanceID() });
            }
            AnalysisWindowDataByTag(ref objDataList, trans.GetChild(i), WinName);
        }
    }
}
