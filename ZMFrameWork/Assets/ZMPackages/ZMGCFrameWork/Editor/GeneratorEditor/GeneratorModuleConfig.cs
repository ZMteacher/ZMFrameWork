// GeneratorModuleConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorModuleConfig", menuName = "ScriptableObjects/GeneratorModuleConfig", order = 1)]
public class GeneratorModuleConfig : ScriptableObject
{

    public string savePath ="Scripts";

    [System.Serializable]
    public class ModuleInfo
    {
        public string moduleName;
        public string moduleNamespace;
    }

    public ModuleInfo[] modules;
}
