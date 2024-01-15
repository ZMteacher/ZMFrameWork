/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAssetFrameWork
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;
public class ExampleWindow : MonoBehaviour
{
    public Image loadSpriteImage;

    public Image loadSpriteAsyncImage;

    public Image loadAltasSpriteImage;

    public RawImage LoadTexture;

    public RawImage LoadTextureAsync;

    public Text LoadTextAssetText;

    public Transform InstantiateRoot;

    public Transform InstantiateAsyncRoot;


    // Start is called before the first frame update
    void Start()
    {
        //同步加载Sprite
        loadSpriteImage.sprite = ZMAssetsFrame.LoadSprite(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/Logo200x200");
        //异步加载Sprite
        ZMAssetsFrame.LoadSpriteAsync(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/majiang", loadSpriteAsyncImage);
        //加载图集中的Sprite
        loadAltasSpriteImage.sprite= ZMAssetsFrame.LoadAtlasSprite(AssetsPathConfig.HALL_TEXTURE_PATH+ "Login/Login", "LoginButton");
        //同步加载图片
        LoadTexture.texture= ZMAssetsFrame.LoadTexture(AssetsPathConfig.HALL_TEXTURE_PATH+ "Hall/BG");
        //异步加载图片
        ZMAssetsFrame.LoadTextureAsync(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/BG",(texture,param1)=> {
            LoadTextureAsync.texture = texture;
        });
        //加载文本资源
        LoadTextAssetText.text= ZMAssetsFrame.LoadTextAsset(AssetsPathConfig.HALL_DATA_PATH+ "PrefabConfig.txt").text;
        //同步克隆对象
        ZMAssetsFrame.Instantiate(AssetsPathConfig.HALL_PREFAB_PATH+ "TestObj", InstantiateRoot);
        //异步克隆对象
        ZMAssetsFrame.InstantiateAsync(AssetsPathConfig.HALL_PREFAB_PATH + "TestObj",(obj,param1,param2)=> {
            Debug.Log("param1:"+ param1 + "  param2:"+ param2);
            obj.transform.SetParent(InstantiateAsyncRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;
        },123,456);

        ZMAssetsFrame.PreLoadObj(AssetsPathConfig.HALL_PREFAB_PATH + "TestObj",100);
    }


}
