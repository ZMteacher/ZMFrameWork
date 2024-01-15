using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Hall
{
    public class LoginMsgMgr : IMsgBehaviour
    {
        public void OnCreate()
        {
            
        }

        public void SendLoginReqeust(string account,string pass)
        {
            //创建出通讯结构体，发送给服务端， TODO

            //测试代码 模拟响应
            OnLoginResponse();
        }

        public void OnLoginResponse()
        {
            UserDataServerModelTest userData = new UserDataServerModelTest();
            userData.id = Random.Range(0,99999);
            userData.name = "铸梦xy"+Random.Range(0, 99999);
            userData.gold = Random.Range(0, 1000000);
            HallWorld.GetExitsLogicCtrl<LoginLogicCtrl>().OnLoginResult(userData);
        }
        public void OnDestroy()
        {
             
        }
    }
}