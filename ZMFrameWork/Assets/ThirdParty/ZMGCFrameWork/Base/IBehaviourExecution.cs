using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourExecution  
{
    Type[] GetLogicBehaviourExecution();
    Type[] GetDataBehaviourExecution();
    Type[] GetMsgBehaviourExecution();
}
