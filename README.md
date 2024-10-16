# ZMFrameWork
UIFramem,AssetsFrame,DMVC
# 项目结构：

GameData：游戏内所有资源目录，可以包含多个游戏资源

ZMUIFrameWork：一款国内独一档的顶级高性能自动化的UI框架。

ZMGCFrameWork：一款MVC思想的智能化、自动化、代码场景式管理框架。游戏有场景，代码有Wold。场景跳转后场景对象全销毁，World销毁后，World内的所有脚本全从内存中释放。开发可以轻松合理规划、控制、脚本内存使用和占用。与ZMUI搭配组合成DMVC。

ZMAsset：一款成熟的游戏资源管理框架,集多版本、多模块热更、资源版本回退、资源加密、资源解密、资源压缩、资源内嵌、资源解压、资源下载、Editor加载、Bundle加载、大型对象池系统、AB冗余剔除、多版本颗粒化、引用计数、内存自动管理优化、等一体式框架

# 更新记录：

## 2024/9/27：

[功能优化] 优化AssetBundle 打包依赖，依赖的材质打成公共资源，降低冗余问题。

## 2024/9/24：

[插件升级] Odin升级为3.3.1.8版本。

[功能优化] 修复AssetBundle打包完成时 InvalidOperationException: Stack empty.报错。感谢 七寸息 提供的问题。

[功能优化] 优化AssetBundle Editor 打包窗口双击逻辑，选中更加准确。


## 2024/9/20：

[目录调整] 调整ZMAsset ZMUI ZMGC 框架目录至ZMPackage目录下，为后续做框架插件统一注入插件做准备。

[单词优化] 修复onCreate命名错误问题。感谢 七寸息 提供的问题。

[Demo功能修复] 增加 WZWorld(五张游戏世界) 修复大厅界面 “五张游戏按钮”点击无法进入对应游戏世界问题。

## 2024/8/6：

[功能新增] 增加可寻址加载资源加载(加载本地某个文件-本地没有-自动下载-下载完成-自动加载资源和依赖-触发资源加载回调)
详见：ZMAddressableAsset 脚本。

[功能新增] ZMFrame-AssetBundle-BundleModule配置窗口增加是否寻址资源开关。寻址资源模块打包后为寻址资源，生成寻址相关配置数据，可通过ZMAddressableAsset脚本进行加载，非寻址资源无法通过ZMAddressableAsset脚本进行加载。

## 2024/7/10：

[BUG修复] 优化stream.Length在2020以上版本报错问题，优化AssetsDownLoader下载错误回调使用为成功回调问题，

[功能新增] 增加UI框架可视化配置，菜单栏ZMFrame-ZNUISetting。ZMAsset增加场景加载接口，LoadSceneAsync。


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