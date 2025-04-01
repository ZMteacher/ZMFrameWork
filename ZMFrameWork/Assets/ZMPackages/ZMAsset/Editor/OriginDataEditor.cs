using UnityEngine;
using UnityEditor;
using ZM.ZMAsset;

public class OriginDataEditor
{
    [MenuItem("Assets/生成原始数据")]
    public static void AssetCreateOriginData()
    {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++)
        {
            EditorUtility.DisplayProgressBar("添加原始数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreateOriginData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    public static void CreateOriginData(GameObject obj)
    {
        OriginData offlineData = obj.GetComponent<OriginData>();
        if (offlineData == null)
        {
            offlineData = obj.AddComponent<OriginData>();
        }
        offlineData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了" + obj.name + " prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/生成UI原始数据")]
    public static void AssetCreateUIOriginData()
    {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++)
        {
            EditorUtility.DisplayProgressBar("添加UI原始数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreateUIData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("离线数据/生成所有UI prefab离线数据")]
    public static void AllCreateUIData()
    {
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameData/Prefabs/UGUI" });
        for (int i = 0; i < allStr.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("添加UI原始数据", "正在扫描路径：" + prefabPath + "......", 1.0f / allStr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (obj == null)
                continue;

            CreateUIData(obj);
        }
        Debug.Log("UI原始数据全部生成完毕！");
        EditorUtility.ClearProgressBar();
    }

    public static void CreateUIData(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("UI");

        UIOriginData uiData = obj.GetComponent<UIOriginData>();
        if (uiData == null)
        {
            uiData = obj.AddComponent<UIOriginData>();
        }
        uiData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了" + obj.name + " UI prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/生成特效原始数据")]
    public static void AssetCreateEffectOriginData()
    {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++)
        {
            EditorUtility.DisplayProgressBar("添加特效原始数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreateEffectOriginData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("离线数据/生成所有特效 prefab离线数据")]
    public static void AllCreateEffectData()
    {
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameData/Prefabs/Effect" });
        for (int i = 0; i < allStr.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("添加特效原始数据", "正在扫描路径：" + prefabPath + "......", 1.0f / allStr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (obj == null)
                continue;

            CreateEffectOriginData(obj);
        }
        Debug.Log("特效原始数据全部生成完毕！");
        EditorUtility.ClearProgressBar();
    }

    public static void CreateEffectOriginData(GameObject obj)
    {
        EffectOriginData effectData = obj.GetComponent<EffectOriginData>();
        if (effectData == null)
        {
            effectData = obj.AddComponent<EffectOriginData>();
        }

        effectData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了" + obj.name + " 特效 prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
}
