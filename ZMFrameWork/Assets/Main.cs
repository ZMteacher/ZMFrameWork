
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;
using ZMGC.Hall;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        //初始化游戏热更框架
        ZMAsset.Instance.InitFrameWork();
        
        Debug.Log(Application.persistentDataPath);
    }

    void Start()
    {
        //热更大厅资源
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, StartGame);
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
