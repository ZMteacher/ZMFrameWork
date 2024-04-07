
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;
using ZMGC.Hall;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    //Assets/Hall/Prefab/LoginWindow.prefab
    private void Awake()
    {
        //初始化游戏热更框架
        ZMAsset.Instance.InitFrameWork();
        
        Debug.Log(Application.persistentDataPath);
    }

    void Start()
    {
        //热更大厅资源
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, this);
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        UIModule.Instance.Initialize();
        WorldManager.CreateWorld<HallWorld>();
    }

}
