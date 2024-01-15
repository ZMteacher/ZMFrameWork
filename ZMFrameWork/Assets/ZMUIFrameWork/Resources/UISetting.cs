using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UISetting", menuName = "UISetting", order = 0)]
public class UISetting : ScriptableObject
{
    private static UISetting _instance;
    public static UISetting Instance { get { if (_instance == null) { _instance = Resources.Load<UISetting>("UISetting"); } return _instance; } }


    public bool SINGMASK_SYSTEM;//是否启用单遮模式
}
