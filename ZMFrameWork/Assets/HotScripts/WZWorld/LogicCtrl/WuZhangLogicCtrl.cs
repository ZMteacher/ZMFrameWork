/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2024/9/20 0:33:16
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
 
namespace ZMGC.WZ
{
    public class WuZhangLogicCtrl : ILogicBehaviour
    {

        public void OnCreate()
        {

        }

        public void ExitGame()
        {
            WorldManager.DestroyWorld<WZWorld>();
        }
        public void OnDestroy()
        {

        }

    }
}
