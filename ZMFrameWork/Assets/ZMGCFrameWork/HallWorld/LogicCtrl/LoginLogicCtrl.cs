using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZMGC.Hall
{
    public class LoginLogicCtrl : ILogicBehaviour
    {
        private LoginMsgMgr mLoginMsg;
        public void OnCreate()
        {
            mLoginMsg = HallWorld.GetExitsMsgMgr<LoginMsgMgr>();
        }

        /// <summary>
        /// 账号登录逻辑
        /// </summary>
        /// <returns></returns>
        public int AccountLogin(string account, string pass)
        {
            //验证账号是否符合规范
            if (account.Length < 6)
            {
                Debug.Log("账号长度不能小于6个字符");
                return 1;
            }
            if (pass.Length < 4)
            {
                Debug.Log("密码长度不能小于4个字符");
                return 2;
            }
            //本地验证通过，发送登录请求
            mLoginMsg.SendLoginReqeust(account, pass);
            return 0;
        }
        /// <summary>
        /// 登录结果
        /// </summary>
        /// <param name="user"></param>
        public void OnLoginResult(UserDataServerModelTest user)
        {
            //缓存用户登录数据
            UserDataMgr userData= HallWorld.GetExitsDataMgr<UserDataMgr>();
            userData.CacheUserData(user);

            //通过事件发送到UI层 让UI去更新界面
            UIEventControl.DispensEvent(UIEventEnum.LoginSuccess);
            HallWorld.EnterHallWorldFormLogin();
            Debug.Log("登录成功 userid:"+user.id+"  userName:"+user.name +" userGold:"+user.gold);
        }
      
        public void OnDestroy()
        {
          
        }
    }
}