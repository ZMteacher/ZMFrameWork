using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
    public class DescList
    {
        public static string[] mStrList = 
        {
            "Item count infinite.",           
            "Item in different height.",
            "Item in different width.",
            "Item with different prefab.",
            "Item with unknown size at init time.",
            "Item various padding.",
            "Item scroll with an offset.",
            "Item count changed at runtime.",
            "Item height width changed at runtime.",
            "Item width changed at runtime.",
            "Item snapped to any position.",
            "Item scroll looping.",
            "Item refreshed and reloaded.",
            "Item cached using pool.",           
            "Item count with high performance.",
            "Item recycled efficiently.",
        };

        public static string[] mLongStrList =
        {
            "For a ScrollRect with ten thousand items, list view does not really create ten thousand items, but only create a few items based on the size of the viewport.",
            "When the ScrollRect moves up, the list view component would check the topmost item's position, and once the topmost item is out of the viewport, then the list view component would recycle the topmost item.",
            "At the same time, it checks the bottommost item's position, and once the bottommost item is near the bottom of the viewport, the list view component would call the onGetItemByIndex handler to create a new item and then positon the new created item under the bottommost item, so the new created item becomes the new bottommost item.",
            "Every item can use a different prefab and can have different height or width and padding.",
            "Every item can use a different prefab and every prefab can have different default padding which is the amount of spacing between each item in the ScrollRect.",
            "Every prefab has a pool for getting and recycling action, and the InitCreateCount is the count created in pool at start.",
            "In the InitListView Method, itemTotalCount parameter indicates the total item count in the scroll view. If this parameter is set to negative one, then it means there are infinite items, and scrollbar would not be supported, and the ItemIndex can be set from min value to max value. If the value of this parameter is greater than or equal to zero, then the ItemIndex can only be set from zero to itemTotalCount substracting one.",
            "When an item is getting in the ScrollRect viewport, this onGetItemByIndex action will be called with the item' index as a parameter, to let you create the item and update its content.",
            "Every created item has a list view component auto attached.",
            "The mItemId property indicates the item's id. This property is set when the item is created or fetched from pool, and will no longer change until the item is recycled back to pool.",
            "SetListItemCount method can be used to set the item total count of the scroll view at runtime. If this parameter is set to negative one, then means there are infinite items, and scrollbar would not be supported, and the ItemIndex can be set from min value to max value. If the value of this parameter is greater than or equal to zero, then the ItemIndex can only be set from zero to itemTotalCount substracting one. If resetPos is set false, then the ScrollRect's content position will not change after this method finished. Item snapped to any position.",
            "For a vertical ScrollRect, when a visible item's height changed at runtime, then OnItemSizeChanged method should be called to let the list view component reposition all visible items' position. For a horizontal ScrollRect, when a visible item's width changed at runtime, then this method should be called to let the list view component reposition all visible items' position.",
            "RefreshItemByItemIndex method can be used to update an item by itemIndex. If the itemIndex item is not visible, then this method will do nothing. Otherwise this method will first call onGetItemByIndex method to get an updated item and then reposition all visible items' position.",
        };
    }    
}