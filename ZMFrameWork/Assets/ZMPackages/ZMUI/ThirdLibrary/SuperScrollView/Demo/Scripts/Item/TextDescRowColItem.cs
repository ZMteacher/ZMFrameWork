using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class TextDescRowColItem : MonoBehaviour
    {
        public Text mNameText;
        public Text mRowText;
        public Text mColumnText;
        public Text mDescText;
        
        int mItemDataIndex = -1;
        ItemData mItemData;

        public void Init()
        {
        }
     
        public void SetItemData(ItemData itemData, int itemIndex,int row,int column)
        {
            mItemData = itemData;
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mRowText.text = "Row: "+row;
            mColumnText.text = "Column: " + column;
            mDescText.text = itemData.mDesc;
        }
    }
}
