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

    public Text preLoadText;


    // Start is called before the first frame update
    async void Start()
    {
        await new WaitForSeconds(1);

    

        //同步加载Sprite
        loadSpriteImage.sprite = ZMAsset.LoadSprite(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/Logo200x200");

        //异步加载Sprite
        ZMAsset.LoadSpriteAsync(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/majiang", loadSpriteAsyncImage);

        //异步可等待的加载Sprite
        loadSpriteAsyncImage.sprite= await ZMAsset.LoadSpriteAsync(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/majiang");

        //加载图集中的Sprite
        loadAltasSpriteImage.sprite= ZMAsset.LoadAtlasSprite(AssetsPathConfig.HALL_TEXTURE_PATH+ "Login/Login", "LoginButton");

        //同步加载Texture
        LoadTexture.texture= ZMAsset.LoadTexture(AssetsPathConfig.HALL_TEXTURE_PATH+ "Hall/BG");

        //异步加载Texture
        ZMAsset.LoadTextureAsync(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/BG",(texture,param1)=> {
            LoadTextureAsync.texture = texture;
            //卸载texture资源，并立即释放内存占用
            ZMAsset.Release(texture);
        });
        //异步可等待的加载图片
        LoadTextureAsync.texture=await ZMAsset.LoadTextureAsync(AssetsPathConfig.HALL_TEXTURE_PATH + "Hall/BG");

        //加载文本资源
        LoadTextAssetText.text= ZMAsset.LoadTextAsset(AssetsPathConfig.HALL_DATA_PATH+ "PrefabConfig.txt").text;
        //异步可等待加载文本资源
        LoadTextAssetText.text = (await ZMAsset.LoadTextAssetAsync(AssetsPathConfig.HALL_DATA_PATH + "PrefabConfig.txt")).text;

        //同步克隆对象
        GameObject nObj= ZMAsset.Instantiate(AssetsPathConfig.HALL_DYNAMICITEM_PATH + "TestObj", InstantiateRoot);
        //将当前对象回收至对象池
        ZMAsset.Release(nObj);

        //异步克隆对象
        ZMAsset.InstantiateAsync(AssetsPathConfig.HALL_DYNAMICITEM_PATH + "TestObj",(obj,param1,param2)=> {
            Debug.Log("param1:"+ param1 + "  param2:"+ param2);
            obj.transform.SetParent(InstantiateAsyncRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;
        },123,456);

        //异步可等待的克隆对象
        AssetsRequest assets = await ZMAsset.InstantiateAsync(AssetsPathConfig.HALL_DYNAMICITEM_PATH + "TestObj", 123, 456,789);
        Debug.Log("param1:" + assets.param1 + "  param2:" + assets.param1);
        GameObject obj = assets.obj;
        obj.transform.SetParent(InstantiateAsyncRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = Quaternion.identity;
        assets.Release();

        //预加载对象
        ZMAsset.PreLoadObj(AssetsPathConfig.HALL_DYNAMICITEM_PATH + "TestObj", 10);
        //异步预加载对象 (这次预加载会触发框架对象池优化机制，同一个对象加载时优先从对象池中获取，故2次预加载对象池中只有10个)
        await ZMAsset.PreLoadObjAsync<GameObject>(AssetsPathConfig.HALL_DYNAMICITEM_PATH + "TestObj", 10);
        preLoadText.text = "预加载对象成功 个数:" + 10 + " 以回收至对象池，对象池节点：RecyclObjRoot";
    }

    private void OnApplicationQuit()
    {
        //释放由资源框架加载出的所有资源，
        //1.清理并销毁对象池中的对象
        //2.释放通过资源框架加载出的非对象资源(sprite,texture...)
        ZMAsset.ClearResourcesAssets(true);
    }

}
