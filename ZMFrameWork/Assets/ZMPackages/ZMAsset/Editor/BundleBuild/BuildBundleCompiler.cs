/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAsset
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZM.ZMAsset
{
    public enum BuildType
    {
        AssetBundle,
        HotPatch, //热更补丁
    }

    public class BuildBundleCompiler
    {
        /// <summary>
        /// 更新公告
        /// </summary>
        private static string mUpdateNotice;
        /// <summary>
        /// 热更补丁版本
        /// </summary>
        private static int mHotPatchVersion;
        /// <summary>
        /// 打包类型
        /// </summary>
        private static BuildType mBuildType;
        /// <summary>
        /// 打包模块数据
        /// </summary>
        private static BundleModuleData mBuildModuleData;
        /// <summary>
        /// 打包模块类型
        /// </summary>
        private static BundleModuleEnum mBundleModuleEnum;
        /// <summary>
        /// 所有AssetBundle文件路径列表
        /// </summary>
        private static List<string> mAllBundlePathList = new List<string>();

        /// <summary>
        /// 所有文件夹的Bundle列表
        /// </summary>
        private static Dictionary<string, List<string>> mAllFolderBundleDic = new Dictionary<string, List<string>>();

        /// <summary>
        /// 所有预制体的Budle字典
        /// </summary>
        private static Dictionary<string, List<string>> mAllPrefabsBundleDic = new Dictionary<string, List<string>>();
        /// <summary>
        /// 要打包的Bundle资产数组
        /// </summary>
        private static List<AssetBundleBuild> mBundleBuildList = new List<AssetBundleBuild>();
        /// <summary>
        /// AssetBundle文件输出路径
        /// </summary>
        private static string mBundleOutPutPath { get { return Application.dataPath + "/../AssetBundle/" + mBundleModuleEnum + "/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/"; } }
        /// <summary>
        /// 热更资源文件输出路径
        /// </summary>
        private static string mHotAssetsOutPutPath { get { return Application.dataPath + "/../HotAssets/" + mBundleModuleEnum + "/" +mHotPatchVersion+"/"+ EditorUserBuildSettings.activeBuildTarget.ToString() + "/"; } }
        /// <summary>
        /// 框架Resources路径
        /// </summary>
        private static string mResourcesPath { get { return Application.dataPath +"/"+BundleSettings.Instance.ZMAssetRootPath+ "/Resources/"; } }
        /// <summary>
        /// 打包AssetBundle
        /// </summary>
        /// <param name="moduleData">资源模块配置数据</param>
        /// <param name="buildType">打包类型</param>
        /// <param name="hotPatchVersion">热更补丁版本</param>
        /// <param name="updateNotice">更新公告</param>
        public static void BuildAssetBundle(BundleModuleData moduleData, BuildType buildType = BuildType.AssetBundle, int hotPatchVersion = 0, string updateNotice = "")
        {

            //初始化打包数据
            bool initStatus= Initlization(moduleData, buildType, hotPatchVersion, updateNotice);
            if (!initStatus) { return; }
            //打包所有的文件夹
            BuildAllFolder();
            //打包父节点下的所有子文件夹
            BuildRootSubFolder();
            //打包所有预制体
            BuildAllPrefabs();
            //开始调用UnityAPI进行打包AssetBundle
            BuildAllAssetBundle();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="moduleData"></param>
        /// <param name="buildType"></param>
        /// <param name="hotPatchVersion"></param>
        /// <param name="updateNotice"></param>
        public static bool Initlization(BundleModuleData moduleData, BuildType buildType = BuildType.AssetBundle, int hotPatchVersion = 0, string updateNotice = "")
        {
            //清理数据以防下次打包时有数据残留
            mAllBundlePathList.Clear();
            mAllFolderBundleDic.Clear();
            mAllPrefabsBundleDic.Clear();
            mBundleBuildList.Clear();
            
            mBuildType = buildType;
            mUpdateNotice = updateNotice;
            mBuildModuleData = moduleData;
            mHotPatchVersion = hotPatchVersion;
            try
            {
                mBundleModuleEnum = (BundleModuleEnum)Enum.Parse(typeof(BundleModuleEnum), moduleData.moduleName);
            }
            catch (Exception)
            {
                Debug.LogError($"{moduleData.moduleName} Enum Not find! Plase Gennerate Enum : Menu ZMFrame-GeneratorModuleEnum");
                return false;
            }
            
            FileHelper.DeleteFolder(mBundleOutPutPath);
            Directory.CreateDirectory(mBundleOutPutPath);
            return true;    

        }
        /// <summary>
        /// 打包所有文件夹AssetBundle
        /// </summary>
        public static void BuildAllFolder()
        {
            if (mBuildModuleData.signFolderPathArr == null || mBuildModuleData.signFolderPathArr.Length == 0)
            {
                return;
            }

            for (int i = 0; i < mBuildModuleData.signFolderPathArr.Length; i++)
            {
                //获取文件夹路径
                string path = mBuildModuleData.signFolderPathArr[i].bundlePath.Replace(@"\", "/");
                EditorUtility.DisplayProgressBar("查找AB文件", "name:" + mBuildModuleData.signFolderPathArr[i].abName, i * 1.0f / mBuildModuleData.signFolderPathArr.Length);
               
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] pathArr = info.GetFiles("*", SearchOption.AllDirectories); ;
                foreach (var fileInfo in pathArr)
                {
                    int removeStartIndex = fileInfo.FullName.LastIndexOf("Assets");
                    string filePath = fileInfo.FullName.Substring(removeStartIndex, fileInfo.FullName.Length - removeStartIndex).Replace("\\", "/");
                    
                    if (filePath.EndsWith(".cs") || filePath.EndsWith(".meta")) continue;
                    
                    mAllBundlePathList.Add(filePath);
                    //获取以模块名+_+AbName的格式的AssetBundle包名
                    string bundleName = GenerateBundleName(mBuildModuleData.signFolderPathArr[i].abName);
                    if (!mAllFolderBundleDic.ContainsKey(bundleName))
                    {
                        mAllFolderBundleDic.Add(bundleName, new List<string> { filePath });
                    }
                    else
                    {
                        mAllFolderBundleDic[bundleName].Add(filePath);
                    }
                }
            }
        }

        /// <summary>
        /// 打包父文件夹下的所有子文件夹
        /// </summary>
        public static void BuildRootSubFolder()
        {
            //检测父文件夹是否有配置，如果没配置就直接跳过
            if (mBuildModuleData.rootFolderPathArr == null || mBuildModuleData.rootFolderPathArr.Length == 0)
            {
                return;
            }

            for (int i = 0; i < mBuildModuleData.rootFolderPathArr.Length; i++)
            {
                string path = mBuildModuleData.rootFolderPathArr[i] + "/";
                //获取父文夹的所有的子文件夹
                string[] folderArr = Directory.GetDirectories(path);
                foreach (var item in folderArr)
                {
                    path = item.Replace(@"\", "/");
                    int nameIndex = path.LastIndexOf("/") + 1;
                    //获取文件夹同名的AssetBundle名称
                    string bundleName = GenerateBundleName(path.Substring(nameIndex, path.Length - nameIndex));
                    //处理子文件夹资源的代码
                    string[] filePathArr = Directory.GetFiles(path, "*");
                    foreach (var filePath in filePathArr)
                    {
                        //过滤.meta文件
                        if (!filePath.EndsWith(".meta"))
                        {
                            string abFilePath = filePath.Replace(@"\", "/");
                            if (!IsRepeatBundleFile(abFilePath))
                            {
                                mAllBundlePathList.Add(abFilePath);
                                if (!mAllFolderBundleDic.ContainsKey(bundleName))
                                {
                                    mAllFolderBundleDic.Add(bundleName, new List<string> { abFilePath });
                                }
                                else
                                {
                                    mAllFolderBundleDic[bundleName].Add(abFilePath);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 打包指定文件夹下的所有预制体
        /// </summary>
        public static void BuildAllPrefabs()
        {
            if (mBuildModuleData.prefabPathArr == null || mBuildModuleData.prefabPathArr.Length == 0)
            {
                return;
            }
            //获取所有预制体的GUID
            string[] guidArr = AssetDatabase.FindAssets("t:Prefab", mBuildModuleData.prefabPathArr);

            for (int i = 0; i < guidArr.Length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guidArr[i]);
                //计算AssetBundle名称
                string bundleName = GenerateBundleName(Path.GetFileNameWithoutExtension(filePath));
                //如果该AssetBUndle不存在，就计算打包数据
                if (!mAllBundlePathList.Contains(filePath))
                {
                    //获取预制体所有的依赖项
                    string[] dependsArr = AssetDatabase.GetDependencies(filePath);
                    List<string> dependsList = new List<string>();
                    for (int k = 0; k < dependsArr.Length; k++)
                    {
                        string path = dependsArr[k];
                        //如果不是冗余文件，就归纳进打包
                        if (!IsRepeatBundleFile(path))
                        {
                            mAllBundlePathList.Add(path);
                            dependsList.Add(path);
                        }
                    }
                    if (!mAllPrefabsBundleDic.ContainsKey(bundleName))
                    {
                        mAllPrefabsBundleDic.Add(bundleName,dependsList);
                    }
                    else
                    {
                        Debug.LogError("重复预制体名字，当前模块下有预制体文件重复 Name:"+bundleName);
                    }
                }
            }
        }
        /// <summary>
        /// 打包AssetBundle
        /// </summary>
        public static void BuildAllAssetBundle()
        {
            //生成所有要打包的Bundle
            GenerateBundleBuilder();
            //生成一份AssetBundle配置
            WriteAssetBundleConfig();

            AssetDatabase.Refresh();

            //调用UnityAPI打包AssetBundle
            AssetBundleManifest manifest= BuildPipeline.BuildAssetBundles(mBundleOutPutPath,mBundleBuildList.ToArray(), (UnityEditor.BuildAssetBundleOptions)Enum.Parse(typeof(UnityEditor.BuildAssetBundleOptions),BundleSettings.Instance.buildbundleOptions.ToString())
                , (UnityEditor.BuildTarget)Enum.Parse(typeof(UnityEditor.BuildTarget), BundleSettings.Instance.buildTarget.ToString()));
            if (manifest==null)
            {
                EditorUtility.DisplayProgressBar("BuildAssetBundle!", "BuildAssetBundle failed!",1);
                Debug.LogError("AssetBundle Build failed!");
            }
            else
            {
                Debug.Log("AssetBundle Build Successs!:"+ manifest);
                DeleteAllBundleManifestFile();
                EncryptAllBundle();
                if (mBuildType== BuildType.HotPatch)
                {
                    GeneratorHotAssets();
                }
            }
            
            EditorUtility.ClearProgressBar();
    
        }
        /// <summary>
        /// 生成AssetBundle配置文件
        /// </summary>
        public static void WriteAssetBundleConfig()
        {
            BundleConfig config = new BundleConfig();
            config.bundleInfoList = new List<BundleInfo>();
            
            //所有AssetBundle文件字典 key =路径 value =AssetBundleName
            Dictionary<string, string> allBundleFilePathDic = new Dictionary<string, string>();
            //遍历所有的Bundle列表，收集成字典，方便写入配置
            foreach (var item in mBundleBuildList)
            {
                foreach (var itemValue in item.assetNames)
                {
                    if (!allBundleFilePathDic.ContainsKey(itemValue))
                    {
                        allBundleFilePathDic.Add(itemValue, item.assetBundleName);
                    }
                }
            }
            
            int index = 0;
            //计算AssetBundle数据，生成AsestBundle配置文件。
            foreach (var item in allBundleFilePathDic)
            {
                //获取文件路径
                string filePath = item.Key;
                if (!filePath.EndsWith(".cs"))
                {
                    BundleInfo info = new BundleInfo();
                    info.path = filePath;
                    info.bundleName = item.Value;
                    info.assetName = Path.GetFileName(filePath);
                    info.crc = Crc32.GetCrc32(filePath);
                    info.bundleModule = mBundleModuleEnum.ToString();
                    info.isAddressableAsset = mBuildModuleData.isAddressableAsset;
                    info.bundleDependce = new List<string>();
                    EditorUtility.DisplayProgressBar("写入AssetBundle配置", "ABName:" + allBundleFilePathDic[filePath], index * 1.0f / allBundleFilePathDic.Count);
                    
                    string[] dependence= AssetDatabase.GetDependencies(filePath);
                    foreach (var dePath in dependence)
                    {
                        //如果依赖项不是当前的这个文件，以及依赖项不是cs脚本 就进行处理
                        if (!dePath.Equals(filePath) && dePath.EndsWith(".cs")==false)
                        {
                            if (allBundleFilePathDic.TryGetValue(dePath,out var assetBundleName))
                            {
                                //如果依赖项已经包含这个AssetBundle就不进行处理，否则添加进依赖项
                                if (!info.bundleDependce.Contains(assetBundleName))
                                {
                                    info.bundleDependce.Add(assetBundleName);
                                }
                            }
                        }
                    }

                    config.bundleInfoList.Add(info);
                }
            }
            //生成AsestBundle配置文件
            string json = JsonConvert.SerializeObject(config,Formatting.Indented);
            string bundleConfigPath = Application.dataPath + "/" + BundleSettings.Instance.ZMAssetRootPath + "/Config/" + mBundleModuleEnum.ToString().ToLower() + "assetbundleconfig.json";
            StreamWriter writer= File.CreateText(bundleConfigPath);
            writer.Write(json);
            writer.Dispose();
            writer.Close();

            AssetDatabase.Refresh();
            
            //生成热更模块配置到Resources文件夹
            GeneratorHotModuleCfgToResource();
        }
        /// <summary>
        /// 生成Bundle打包列表
        /// </summary>
        /// <param name="clear"></param>
        public static void GenerateBundleBuilder(bool clear=false)
        {
            int i = 0;
            //收集所有要打包的文件夹Bundle
            foreach (var item in mAllFolderBundleDic)
            {
                i++;
                EditorUtility.DisplayProgressBar("Modify AssetBundle Name","Name:"+item.Key, i*1.0f/mAllFolderBundleDic.Count);
                mBundleBuildList.Add(new AssetBundleBuild(){ assetBundleName =$"{item.Key.ToLower()}{BundleSettings.Instance.ABSUFFIX}" , assetNames = item.Value.ToArray() });
            }
            //收集所有要打包的预制体Bundle
            i = 0;
            foreach (var item in mAllPrefabsBundleDic)
            {
                i++;
                EditorUtility.DisplayProgressBar("Modify AssetBundle Name", "Name:" + item.Key, i * 1.0f / mAllPrefabsBundleDic.Count);
                mBundleBuildList.Add(new AssetBundleBuild(){ assetBundleName = $"{item.Key.ToLower()}{BundleSettings.Instance.ABSUFFIX}", assetNames = item.Value.ToArray() });
            }
            
            //收集至Bundle打包配置文件
            string bundleConfigPath = Application.dataPath + "/" + BundleSettings.Instance.ZMAssetRootPath + "/Config/" + mBundleModuleEnum.ToString().ToLower() + "assetbundleconfig.json";
            mBundleBuildList.Add(new AssetBundleBuild(){ assetBundleName = mBundleModuleEnum.ToString().ToLower() + "bundleconfig"+BundleSettings.Instance.ABSUFFIX,assetNames = new []
            {
                $"{bundleConfigPath.Replace(Application.dataPath, "Assets/")}"
            }});

        }
        /// <summary>
        /// 是否是重复的Bundle文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsRepeatBundleFile(string path)
        {
            foreach (var item in mAllBundlePathList)
            {
                if (string.Equals(item, path) || item.Contains(path) || path.EndsWith(".cs"))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GenerateBundleName(string abName)
        {
            return mBundleModuleEnum.ToString() + "_" + abName;
        }

        /// <summary>
        /// 删除所有AssetBundle自动生成的清单文件
        /// </summary>
        public static void DeleteAllBundleManifestFile()
        {
           string[] filePathArr= Directory.GetFiles(mBundleOutPutPath);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".manifest"))
                {
                    File.Delete(path);
                }
            }
        }
        /// <summary>
        /// 加密所有的AssetBundle
        /// </summary>
        public static void EncryptAllBundle()
        {
            if (BundleSettings.Instance.bundleEncrypt.isEncrypt)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(mBundleOutPutPath);
                FileInfo[] fileInfoArr = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < fileInfoArr.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("加密文件", "Name:" + fileInfoArr[i].Name, i * 1.0f / fileInfoArr.Length);
                    AES.AESFileEncrypt(fileInfoArr[i].FullName, BundleSettings.Instance.bundleEncrypt.encryptKey);
                }
                EditorUtility.ClearProgressBar();
                Debug.Log("AssetBundle Encrypt Finish!");
            }
        }
        /// <summary>
        /// 拷贝AssetBundle至StramingAssets文件夹
        /// </summary>
        /// <param name="moduleData"></param>
        /// <param name="showTips"></param>
        public static void CopyBundleToStramingAssets(BundleModuleData moduleData,bool showTips=true)
        {
            mBundleModuleEnum = (BundleModuleEnum)Enum.Parse(typeof(BundleModuleEnum),moduleData.moduleName);
            //获取目标文件夹下的所有AssetBundle文件
            DirectoryInfo directoryInfo = new DirectoryInfo(mBundleOutPutPath);
            FileInfo[] fileInfoArr = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            //Bundle内嵌的目标文件夹
            string streamingAssetsPath = Application.streamingAssetsPath + "/AssetBundle/" + mBundleModuleEnum + "/";

            FileHelper.DeleteFolder(streamingAssetsPath);
            Directory.CreateDirectory(streamingAssetsPath);

            List<BuiltinBundleInfo> bundleInfoList = new List<BuiltinBundleInfo>();
            for (int i = 0; i < fileInfoArr.Length; i++)
            {
                EditorUtility.DisplayProgressBar("内嵌资源中", "Name:" + fileInfoArr[i].Name, i * 1.0f / fileInfoArr.Length);
                //拷贝文件
                File.Copy(fileInfoArr[i].FullName, streamingAssetsPath+ fileInfoArr[i].Name);
                //生成内嵌资源文件信息
                BuiltinBundleInfo info = new BuiltinBundleInfo();
                info.fileName = fileInfoArr[i].Name;
                info.md5 = MD5.GetMd5FromFile(fileInfoArr[i].FullName);
                info.size = fileInfoArr[i].Length / 1024;
                bundleInfoList.Add(info);
            }

            string json = JsonConvert.SerializeObject(bundleInfoList, Formatting.Indented);

            if (!Directory.Exists(mResourcesPath))
            {
                Directory.CreateDirectory(mResourcesPath);
            }

            //写入包内资源配置文件到Resources文件夹
            FileHelper.WriteFile(mResourcesPath+mBundleModuleEnum+"info.json",System.Text.Encoding.UTF8.GetBytes(json));

            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
            if (showTips)
            {
                EditorUtility.DisplayDialog("内嵌操作","内嵌资源完成 Path："+ streamingAssetsPath,"确认");
            }
            Debug.Log(" Assets Copy toStreamingAssets Finish!");
        }


        /// <summary>
        /// 生成热更模块配置到Resources文件夹
        /// </summary>
        public static void GeneratorHotModuleCfgToResource()
        {
            string bundleModuleCfgJson = string.Empty;
            TextAsset textAsset= Resources.Load<TextAsset>("bundlemoduleCfg");
            if (textAsset==null)
            {
                List<BundleModuleData> bundleModuleData = new List<BundleModuleData> { mBuildModuleData };
                bundleModuleCfgJson = JsonConvert.SerializeObject(bundleModuleData);
                File.WriteAllText(mResourcesPath + "bundlemoduleCfg.json", bundleModuleCfgJson);
            }
            else
            {
                List<BundleModuleData> bundleModuleData = JsonConvert.DeserializeObject<List<BundleModuleData>>(textAsset.text);
                for (int i = 0; i < bundleModuleData.Count; i++)
                {
                    if (string.Equals(bundleModuleData[i].moduleName,mBuildModuleData.moduleName))
                    {
                        bundleModuleData.Remove(bundleModuleData[i]);
                        break;
                    }
                }
                bundleModuleData.Add(mBuildModuleData);
                bundleModuleCfgJson= JsonConvert.SerializeObject(bundleModuleData);
                File.WriteAllText(mResourcesPath+ "bundlemoduleCfg.json", bundleModuleCfgJson);
            }

        }
        /// <summary>
        /// 生成热更资源
        /// </summary>
        public static void GeneratorHotAssets()
        {
            FileHelper.DeleteFolder(mHotAssetsOutPutPath);
            Directory.CreateDirectory(mHotAssetsOutPutPath);

            string[] bundlePatchArr= Directory.GetFiles(mBundleOutPutPath,"*"+ BundleSettings.Instance.ABSUFFIX);
            for (int i = 0; i < bundlePatchArr.Length; i++)
            {
              
                string path = bundlePatchArr[i];
                EditorUtility.DisplayProgressBar("生成热更文件", "Name:" + Path.GetFileName(path), i * 1.0f / bundlePatchArr.Length);
                string disPath = mHotAssetsOutPutPath + Path.GetFileName(path);

                File.Copy(path,disPath);
            }
            Debug.Log("热更文件生成成功");
            GeneratorHotAssetsManifest();
        }
        /// <summary>
        /// 生成热更资源配置清单
        /// </summary>
        public static void GeneratorHotAssetsManifest()
        {
            //设置清单数据
            HotAssetsManifest assetsManifest = new HotAssetsManifest();
            assetsManifest.updateNotice = mUpdateNotice;
            assetsManifest.downLoadURL = BundleSettings.Instance.AssetBundleDownLoadUrl+"/HotAssets/"+mBundleModuleEnum+"/"+
                mHotPatchVersion+"/"+BundleSettings.Instance.buildTarget;

            //设置补丁数据
            HotAssetsPatch hotAssetsPatch = new HotAssetsPatch();
            hotAssetsPatch.patchVersion = mHotPatchVersion;
            //计算热更补丁文件信息
            DirectoryInfo directory = new DirectoryInfo(mHotAssetsOutPutPath);
            FileInfo[] bundleInfoArr= directory.GetFiles("*"+ BundleSettings.Instance.ABSUFFIX);
            foreach (var bundleInfo in bundleInfoArr)
            {
                HotFileInfo info = new HotFileInfo();
                info.abName = bundleInfo.Name;
                info.md5 = MD5.GetMd5FromFile(bundleInfo.FullName);
                info.size = bundleInfo.Length / 1024.0f;
                hotAssetsPatch.hotAssetsList.Add(info);
            }
            assetsManifest.hotAssetsPatchList.Add(hotAssetsPatch);

            //把对象转换为Json字符串
            string json= JsonConvert.SerializeObject(assetsManifest, Formatting.Indented);
            FileHelper.WriteFile(Application.dataPath+"/../HotAssets/"+mBundleModuleEnum+"AssetsHotManifest.json",
                System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}