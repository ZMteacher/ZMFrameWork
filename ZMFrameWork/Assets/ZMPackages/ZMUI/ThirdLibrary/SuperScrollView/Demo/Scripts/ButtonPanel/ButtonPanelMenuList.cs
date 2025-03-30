using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{    
    public class ButtonPanelMenuList : MonoBehaviour 
    {
        static string[] sceneArray0 =
        {
            "ListViewTopToBottomDemo",
            "ListViewLeftToRightDemo",
            "GalleryVerticalDemo",
            "GalleryHorizontalDemo",   
            "ListViewPullDownRefreshDemo",
            "ListViewPullUpLoadMoreDemo",            
            "ListViewSelectDeleteDemo",
            "ListViewExpandDemo",            
            "ListViewFilterDemo",    
            "ListViewContentFitterDemo",  
            "ListViewClickLoadMoreDemo",      
            "ListViewMultiplePrefabTopToBottomDemo",
            "ListViewSimpleTopToBottomDemo",            
            "ListViewSimpleLoopTopToBottomDemo", 
            "ListViewSimpleLoadMoreDemo",  
        };

        static string[] sceneArray1 =
        {
            "GridViewTopToBottomDemo",
            "GridViewLeftToRightDemo",           
            "GridViewClickLoadMoreDemo",
            "GridViewSelectDeleteDemo",
            "GridViewSimpleFilterDemo",           
            "GridViewDiagonalTopLeftDemo", 
            "GridViewMultiplePrefabTopToBottomDemo", 
            "GridViewMultiplePrefabLeftToRightDemo",                              
            "GridViewSimpleTopToBottomDemo",                    
            "GridViewSimpleDiagonalTopRightDemo",
            "ResponsiveViewDemo",
            "ResponsiveViewRefreshLoadDemo",
            "SpinDatePickerDemo",
            "SpinTimePickerDemo",
            "SpinDateTimePickerDemo",
        };        

        static string[] sceneArray2 =
        {
            "StaggeredViewTopToBottomDemo",
            "StaggeredViewLeftToRightDemo",
            "StaggeredViewBottomToTopDemo",
            "StaggeredViewRightToLeftDemo",
            "StaggeredViewSimpleTopToBottomDemo",
            "StaggeredViewSimpleLeftToRightDemo",    
            "ChatViewDemo",
            "ChatViewChangeViewportHeightDemo",         
            "TreeViewDemo",
            "TreeViewWithStickyHeadDemo",
            "TreeViewWithChildIndentDemo",            
            "TreeViewSimpleDemo",
            "PageViewDemo",
            "PageViewSimpleDemo",        
        };    

        static string[] sceneArray3 =
        {
            "SpecialGridViewTopToBottomDemo",
            "SpecialGridViewLeftToRightDemo",
            "SpecialGridViewPullDownRefreshDemo",
            "SpecialGridViewPullUpLoadMoreDemo",
            "SpecialGridViewSelectDeleteDemo",
            "SpecialGridViewFeatureTopToBottomDemo",            
            "SpecialGridViewSimpleTopToBottomDemo",
            "SpecialGridViewSimpleLeftToRightDemo",
            "NestedListViewTopToBottomDemo",
            "NestedGridViewTopToBottomDemo",
            "NestedListViewLeftToRightDemo",
            "NestedGridViewLeftToRightDemo",
            "NestedSimpleListViewDemo",
            "NestedSimpleGridViewDemo",   
            "NestedSimpleSpecialGridViewDemo",
        };       

        static string[] sceneArray4 =
        {           
            "ListViewAddClipAnimationDemo",
            "ListViewAddFadeAnimationDemo",
            "ListViewAddClipFadeAnimationDemo",
            "ListViewAddSlideLeftAnimationDemo",
            "ListViewAddSlideRightAnimationDemo",
            "ListViewDeleteClipAnimationDemo",
            "ListViewDeleteFadeAnimationDemo",
            "ListViewDeleteClipFadeAnimationDemo",
            "ListViewDeleteSlideLeftAnimationDemo",
            "ListViewDeleteSlideRightAnimationDemo",
            "ListViewExpandClipAnimationDemo",
            "ListViewExpandFadeAnimationDemo",
            "ListViewExpandClipFadeAnimationDemo",
            "DraggableViewFadeTopToBottomDemo",
            "DraggableViewFadeLeftToRightDemo",
            "DraggableViewTopToBottomDemo",
            "DraggableViewLeftToRightDemo"
        };       

        static string[][] allSceneArray = new string[][] { sceneArray0, sceneArray1, sceneArray2, sceneArray3, sceneArray4};
        static string[] mainMenuSceneArray = new string[] { "MenuListViewGallery", "MenuGridViewResponsiveSpin", "MenuStaggeredChatTreePage", "MenuSpecialViewNested","MenuListAnimationDraggable"};
        
        Button button;
        public int sceneArrayIndex;
        
        void Start() 
        {
            Scene curScene = SceneManager.GetActiveScene();
            string curSceneName = curScene.name;
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        public void OnButtonClick()
        {
            int index = GetSelfIndexInParent();
            ButtonPanelMenu.lastSelectSceneArrayIndex = sceneArrayIndex;
            string sceneName = GetSceneName(sceneArrayIndex, index);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public static void BackToMainMenu()
        {
            string mainMenuSceneName = "Menu";
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }


        static int GetSceneArrayIndexByName(string sceneName)
        {
            for (int i = 0; i < allSceneArray.Length; ++i)
            {
                string[] sceneArray = allSceneArray[i];
                foreach (string name in sceneArray)
                {
                    if (name == sceneName)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        string GetSceneName(int sceneArrayIndex,int index)
        {
            string[] sceneArray = allSceneArray[sceneArrayIndex];
            return sceneArray[index];
        }

        int GetSelfIndexInParent()
        {
            Transform parentTrans = gameObject.transform.parent;
            for(int i =0;i< parentTrans.childCount;++i)
            {
                if(parentTrans.GetChild(i) == gameObject.transform)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}