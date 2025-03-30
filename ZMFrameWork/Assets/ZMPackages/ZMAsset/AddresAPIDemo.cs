using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZM.ZMAsset;

public class AddresAPIDemo : MonoBehaviour
{
    public RawImage rawImageAsync;
    
    private async void Awake()
    {
        ZMAsset.InitFrameWork();
        AssetsRequest asset = await ZMAddressableAsset.InstantiateAsyncFormPool(AssetsPathConfig.GAME_ITEM_PATH + "6013/6013",BundleModuleEnum.AdressAsset);
 
        asset.obj.transform.SetParent(transform.GetChild(0).GetChild(0));

        await UniTask.Delay(1000);
        rawImageAsync.texture=await ZMAddressableAsset.LoadResourceAsync<Texture>(AssetsPathConfig.GAME_ITEM_PATH + "6001/huafei.png", BundleModuleEnum.AdressAsset);
    }

    public async void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            rawImageAsync.texture = await ZMAddressableAsset.LoadResourceAsync<Texture>(AssetsPathConfig.GAME_ITEM_PATH + "6001/huafei.png", BundleModuleEnum.AdressAsset);

        }
    }


}
