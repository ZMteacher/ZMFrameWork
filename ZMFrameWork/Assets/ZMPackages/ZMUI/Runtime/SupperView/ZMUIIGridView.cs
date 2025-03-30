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
using SuperScrollView;
using UnityEngine;

namespace ZM.UI
{
    public class ZMUIIGridView : MonoBehaviour
    {
        public LoopGridView loopListView;

        private int m_ViewDataCount = 99; //你的数据列表长度 建议在当前接口触发时向数据层索取

        private GetItemDataDelegate m_GetItemDataCallBack = null;

        private void Awake()
        {
            if (loopListView == null)
                loopListView = GetComponent<LoopGridView>();
        }

        /// <summary>
        /// 刷新列表显示
        /// </summary>
        /// <param name="reSetPos">是否重置到顶部或底部</param>
        /// <param name="viewDataCount">数据长度</param>
        /// <param name="getItemDataCallBack">获取数据回调</param>
        public void RefreshListView(bool reSetPos, int viewDataCount, GetItemDataDelegate getItemDataCallBack)
        {
            m_ViewDataCount = viewDataCount;
            m_GetItemDataCallBack = getItemDataCallBack;

            if (!loopListView.ListViewInited)
            {
                //初始化滚动列表 切记不可在Awake或Start中初始化Count=0的列表。SupperView会有BUG
                loopListView.InitGridView(m_ViewDataCount, OnShowItemByIndex);
            }
            else
            {
                //数据发生变化，重新设置最新的数据，数据增删必须要调用此接口，否则会出现item索引与数据不一致和一切其他的显示BUG
                loopListView.SetListItemCount(m_ViewDataCount, false);
                if (reSetPos)
                {
                    loopListView.MovePanelToItemByIndex(0, 0);
                }
                else
                {
                    loopListView.RefreshAllShownItem();
                }
            }
        }

        /// <summary>
        /// Item元素显示回调
        /// </summary>
        /// <param name="listView">滚动列表</param>
        /// <param name="index">item索引</param>
        /// <returns></returns>
        LoopGridViewItem OnShowItemByIndex(LoopGridView gridView, int index,int row,int column)
        {
            if (index < 0 || index >= m_ViewDataCount) return null;

            //获取item显示数据
            object itemData = m_GetItemDataCallBack(index);
            if (itemData == null) return null;

            if (loopListView.ItemPrefabDataList.Count == 0)
            {
                Debug.LogError("ItemPrefabDataList is null!");
                return null;
            }

            //创建对应item预制体
            LoopGridViewItem item = gridView.NewListViewItem(loopListView.ItemPrefabDataList[0].mItemPrefab.name);
            //获取Item上的脚本组件
            IZMUIViewListItem itemScript = item.GetComponent<IZMUIViewListItem>();

            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                //item脚本初始化接口
                itemScript.InitListItem();
            }

            //设置Item脚本数据
            itemScript.SetListItemShowData(index, itemData);
            return item;
        }

        public void OnRelease()
        {
            // foreach (var item in loopListView.ite)
            // {
            //     item.GetComponent<IZMUIViewListItem>().OnRelease();
            // }
        }
    }
}