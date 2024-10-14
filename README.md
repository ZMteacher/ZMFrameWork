# ZMFrameWork
UIFramem,AssetsFrame,DMVC
# 项目结构：

GameData：游戏内所有资源目录，可以包含多个游戏资源

ZMUIFrameWork：一款国内独一档的顶级高性能自动化的UI框架。

ZMGCFrameWork：一款MVC思想的智能化、自动化、代码场景式管理框架。游戏有场景，代码有Wold。场景跳转后场景对象全销毁，World销毁后，World内的所有脚本全从内存中释放。开发可以轻松合理规划、控制、脚本内存使用和占用。与ZMUI搭配组合成DMVC。

ZMAsset：一款成熟的游戏资源管理框架,集多版本、多模块热更、资源版本回退、资源加密、资源解密、资源压缩、资源内嵌、资源解压、资源下载、Editor加载、Bundle加载、大型对象池系统、AB冗余剔除、多版本颗粒化、引用计数、内存自动管理优化、等一体式框架

版本更新：

Date: 2024/6/12 11:45:25
优化资源热更流程，AssetBundleMangaer.LoadBundleConfig接口封装为ZMAsset.InitAssetModule.并自动化处理资产模块的初始化。
增加边玩边下，加载资源时发现没有该资源则会自动下载该资源，下载完成后自动加载
AssetBundle模块配置面板增加是否寻址资源开关

Date: 2024/6/12 11:45:25
优化stream.Length在2020以上版本报错问题，优化AssetsDownLoader下载错误回调使用为成功回调问题，

Date: 2024/6/12 11:41:46
ZMUI框架增加可视化配置窗口，菜单栏ZMFrame-ZNUISetting。ZMAsset增加场景加载接口:LoadSceneAsync。

Date: 2024/4/11 12:26:05
ZMUI框架增加智能显隐功能

Date: 2024/4/7 13:01:02
v1.0.1 ZMAssetsFrame更名为ZMAsset,增加await可等待异步加载接口，引入AsyncAwaitUtil库(节省包体大小没有使用UniTask),增加APIDemo场景，优化案例工程逻辑


