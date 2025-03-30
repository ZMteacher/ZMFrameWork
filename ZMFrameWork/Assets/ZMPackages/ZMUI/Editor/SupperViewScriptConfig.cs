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
using System.Text;
using UnityEngine;

public class SupperViewScriptConfig 
{
    public static void GenerateInitContent(string fieldName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("\t\tpublic void RefreshListView(bool reSetPos)");
        builder.Append("\t\t{");
        builder.Append("\t\t\tint dataCount = 99; //你的数据列表长度 建议在当前接口触发时向数据层索取");
        
        builder.Append($"\t\t\tif (!{fieldName}.ListViewInited)");
        builder.Append("\t\t\t{");
        builder.Append("\t\t\t//初始化滚动列表 切记不可在Awake或Start中初始化Count=0的列表。会有BUG");
        builder.Append($"\t\t\t{fieldName}.InitListView(dataCount, OnGetItemByIndex);");
        builder.Append("\t\t\t}");
        builder.Append("\t\t\telse");
        builder.Append("\t\t\t{");
        builder.Append($"\t\t\t//数据发生变化，重新设置最新的数据，数据增删必须要调用此接口，否则会出现item索引与数据不一致和一切其他的显示BUG");
        builder.Append($"\t\t\t{fieldName}.SetListItemCount(dataCount, reSetPos);");
        builder.Append($"\t\t\t{fieldName}.RefreshAllShownItem();");
        builder.Append("\t\t\t}");
        builder.Append("\t\t}");
    }
    
    public string GetCodeAsString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)");
        sb.AppendLine("{");
        sb.AppendLine("    if (index < 0 || index >= mDataSourceMgr.TotalItemCount)   return null;");
        sb.AppendLine("    //获取item显示数据");
        sb.AppendLine("    //ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);");
        sb.AppendLine("    if(itemData == null) return null;");
        sb.AppendLine(" ");
        sb.AppendLine("    LoopListViewItem2 item = listView.NewListViewItem(\"ItemPrefab\");");
        sb.AppendLine(" "); 
        sb.AppendLine("    //BaseVerticalItem itemScript = item.GetComponent<BaseVerticalItem>();");
        sb.AppendLine(" ");
        sb.AppendLine("    //if (item.IsInitHandlerCalled == false)");
        sb.AppendLine("    //{");
        sb.AppendLine("        //item.IsInitHandlerCalled = true;");
        sb.AppendLine(" ");
        sb.AppendLine("        //itemScript.Init();");
        sb.AppendLine("    }");
        sb.AppendLine(" ");
        sb.AppendLine("    //itemScript.SetItemData(itemData, index);");
        sb.AppendLine("    return item;");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
