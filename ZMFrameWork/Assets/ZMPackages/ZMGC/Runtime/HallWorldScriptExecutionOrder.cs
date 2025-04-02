using System;
public class HallWorldScriptExecutionOrder  :IBehaviourExecution
{
    private static readonly string[] LogicBehaviorExecutions = new string[] {
       "TaskLogicCtrl",
     };

    private static readonly string[] DataBehaviorExecutions = new string[] {
       "RankDataMgr",
       "UserDataMgr",
     };

    private static readonly string[] MsgBehaviorExecutions = new string[] {
       "TaskMsgMgr",
     };

    public string[] GetDataBehaviourExecution()
    {
        return DataBehaviorExecutions;
    }

    public string[] GetLogicBehaviourExecution()
    {
        return LogicBehaviorExecutions;
    }

    public string[] GetMsgBehaviourExecution()
    {
        return MsgBehaviorExecutions;
    }
}
