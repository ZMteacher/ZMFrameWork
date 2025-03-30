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

public class SuperScrollViewStringInfo
{
     public static StringBuilder GetListViewStringInfo()
     {
          return new StringBuilder();
          // StringBuilder builder = new StringBuilder();
          // builder.Append($"\t\tLoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)");
          
          // LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
          // {
          //      if (index < 0 || index >= mDataSourceMgr.TotalItemCount)
          //      {
          //           return null;
          //      }
          //      //get the data to showing
          //      ItemData itemData = mDataSourceMgr.GetItemDataByIndex(index);
          //      if(itemData == null)
          //      {
          //           return null;
          //      }
          //      /*get a new item. Every item can use a different prefab,
          //       the parameter of the NewListViewItem is the prefab’name.
          //      And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting*/
          //      LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
          //      //get your own component
          //      BaseVerticalItem itemScript = item.GetComponent<BaseVerticalItem>();
          //      //IsInitHandlerCalled is false means this item is new created but not fetched from pool.
          //      if (item.IsInitHandlerCalled == false)
          //      {
          //           item.IsInitHandlerCalled = true;
          //           itemScript.Init();// here to init the item, such as add button click event listener.
          //      }
          //      //update the item’s content for showing, such as image,text.
          //      itemScript.SetItemData(itemData,index);
          //      return item;
          // }
     }

}
