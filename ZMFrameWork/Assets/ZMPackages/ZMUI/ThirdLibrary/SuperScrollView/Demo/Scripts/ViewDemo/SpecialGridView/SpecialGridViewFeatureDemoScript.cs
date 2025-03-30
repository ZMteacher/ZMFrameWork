using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class SpecialGridViewFeatureDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public int mTotalDataCount = 10000;
        DataSourceMgr<ItemData> mDataSourceMgr;
        const int mItemCountPerRow = 3;// how many items in one row

        Button mSetCountButton;
        InputField mSetCountInput;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mAddButton;
        InputField mAddInput;
        Button mBackButton;     

        int[] mFeatureArray = {1,2};
        string[] mFeaturePrefabs = {"ItemPrefab1","ItemPrefab2"};

        // Use this for initialization
        void Start()
        {
            mDataSourceMgr = new DataSourceMgr<ItemData>(mTotalDataCount);
            int featureCount = GetFeatureItemCount();        
            int row = (mDataSourceMgr.TotalItemCount-featureCount) / mItemCountPerRow;
            if((mDataSourceMgr.TotalItemCount-featureCount) % mItemCountPerRow > 0)
            {
                row++;
            }
            //count is the total row count
            mLoopListView.InitListView(row+mFeatureArray.Length, OnGetItemByIndex);    
            InitButtonPanel();
        }

        void InitButtonPanel()
        {
            mSetCountButton = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/ButtonGroupSetCount/SetCountInputField").GetComponent<InputField>();
            mSetCountButton.onClick.AddListener(OnSetCountButtonClicked);
            
            mScrollToButton = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/ButtonGroupScrollTo/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnScrollToButtonClicked);

            mAddButton = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddButton").GetComponent<Button>();
            mAddButton.onClick.AddListener(OnAddButtonClicked);
            mAddInput = GameObject.Find("ButtonPanel/ButtonGroupAdd/AddInputField").GetComponent<InputField>();

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackButtonClicked);  
        }               

        /*when a row is getting show in the scrollrect viewport, 
        this method will be called with the row’ rowIndex as a parameter, 
        to let you create the row  and update its content.

        SuperScrollView uses single items with subitems that make up the columns in the row.
        so in fact, the GridView is ListView.
        if one row is make up with 3 subitems, then the GridView looks like:

            row0:  subitem0 subitem1 subitem2
            row1:  subitem3 subitem4 subitem5
            row2:  subitem6 subitem7 subitem8
            row3:  subitem9 subitem10 subitem11
            ...
        */
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0 )
            {
                return null;
            }    

            LoopListViewItem2 item = null;  
            if(rowIndex < mFeatureArray.Length)
            {
                item = NewFeatureItems(listView,rowIndex);                
            }   
            else
            {
                item = NewMainItems(listView,rowIndex); 
            } 
            return item;
        }  

        LoopListViewItem2 NewFeatureItems(LoopListView2 listView, int rowIndex)
        {
            if(rowIndex >= mFeatureArray.Length)
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem(mFeaturePrefabs[rowIndex]);  
            BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            int initItemIndex = GetInitItemIndex(rowIndex);
            for (int i = 0;i< mFeatureArray[rowIndex]; ++i)
            {
                int itemIndex = i + initItemIndex;
                if(itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                //update the subitem content.
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }                 
            return item;         
        }    

        LoopListViewItem2 NewMainItems(LoopListView2 listView, int rowIndex)
        {            
            if(rowIndex < mFeatureArray.Length)
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab3");
            BaseHorizontalItemList itemScript = item.GetComponent<BaseHorizontalItemList>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }  
            int initItemIndex = GetInitItemIndex(rowIndex);
            int initRowIndex =  GetInitRowIndex(rowIndex);
            for (int i = 0;i< mItemCountPerRow; ++i)
            {
                int itemIndex = initRowIndex * mItemCountPerRow + i + initItemIndex;
                if(itemIndex >= mDataSourceMgr.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = mDataSourceMgr.GetItemDataByIndex(itemIndex);
                //update the subitem content.
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }  
            return item;  
        }  

        int GetInitItemIndex(int rowIndex)
        {
            if(rowIndex == 0)
            {
                return 0;
            }
            int initItemIndex = 0;
            int maxLength = mFeatureArray.Length;
            if(rowIndex < mFeatureArray.Length)
            {
                maxLength = rowIndex;
            }
            for(int i = 0; i < maxLength; i++ )
            {
                initItemIndex += mFeatureArray[i];
            }
            return initItemIndex;
        }

        int GetInitRowIndex(int rowIndex)
        {
            int initRowIndex = 0;
            if(rowIndex < mFeatureArray.Length)
            {
                initRowIndex = rowIndex;
            }
            else
            {
                initRowIndex = rowIndex - mFeatureArray.Length;
            }
            return initRowIndex;
        }

        int GetFeatureItemCount()
        {
            int tmpCount = 0;
            for(int i = 0; i < mFeatureArray.Length; i++)
            {
                tmpCount += mFeatureArray[i];
            }
            return tmpCount;
        }

        int GetFeatureLastRowIndex()
        {
            return mFeatureArray.Length-1;
        }

        int GetRowIndex(int itemIndex)
        {
            int featureItemCount = GetFeatureItemCount();
            int firstRowIndex = GetFeatureLastRowIndex() + 1;
            int tmpRowIndex = 0;
            if(itemIndex > (featureItemCount-1))
            {
                tmpRowIndex = ((itemIndex-featureItemCount)/mItemCountPerRow) + firstRowIndex;
            }
            else
            {
                int curItemIndex = itemIndex;
                tmpRowIndex = GetFeatureLastRowIndex();
                for(int i = 0; i < mFeatureArray.Length; ++i)
                {     
                    curItemIndex = curItemIndex - mFeatureArray[i];
                    if(curItemIndex < 0)
                    {
                        tmpRowIndex = i;
                        break;
                    }
                }    
            }
            return tmpRowIndex;            
        }

        void OnSetCountButtonClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count < 0)
            {
                return;
            }                    

            int tmpRowIndex = GetRowIndex(count-1); 
            mDataSourceMgr.SetDataTotalCount(count);     
            mLoopListView.SetListItemCount(tmpRowIndex+1, false);
            mLoopListView.RefreshAllShownItem();            
        }  

        void OnScrollToButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0) || (itemIndex >= mDataSourceMgr.TotalItemCount))
            {
                return;
            }
            int tmpRowIndex = GetRowIndex(itemIndex);        
            mLoopListView.MovePanelToItemIndex(tmpRowIndex, 0);
        }

        void OnAddButtonClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mAddInput.text, out itemIndex) == false)
            {
                return;
            }
            if((itemIndex < 0)||(itemIndex > mDataSourceMgr.TotalItemCount))
            {
                return;
            }         
            int tmpRow = GetRowIndex(mDataSourceMgr.TotalItemCount);  
            ItemData newData = mDataSourceMgr.InsertData(itemIndex);  
            newData.mDesc = newData.mDesc +" [New]"; 
            mLoopListView.SetListItemCount(tmpRow+1, false);
            mLoopListView.RefreshAllShownItem();               
        }     

        void OnBackButtonClicked()
        {
            ButtonPanelMenuList.BackToMainMenu();
        }    
    }    
}
