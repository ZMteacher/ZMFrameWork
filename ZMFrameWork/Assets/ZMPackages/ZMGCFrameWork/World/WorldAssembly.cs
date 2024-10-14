using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class World
{
    public  void AddLogicCtrl(ILogicBehaviour behaviour)
    {
        mLogicBehaviourDic.Add(behaviour.GetType().Name, behaviour);
        behaviour.OnCreate();
    }

    public  void AddDataMgr(IDataBehaviour behaviour)
    {
        mDataBehaviourDic.Add(behaviour.GetType().Name, behaviour);
        behaviour.OnCreate();
    }
    public  void AddMsgMgr(IMsgBehaviour behaviour)
    {
        mMsgBehaviourDic.Add(behaviour.GetType().Name, behaviour);
        behaviour.OnCreate();
    }
}
