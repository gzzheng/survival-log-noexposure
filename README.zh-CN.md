# NoExposure · 生存日志 · 无暴露值

[English](README.md) · **简体中文**

控制《生存日志》探索时的**暴露值**增长——可以完全冻结，也可以只让它涨得慢一些。

![game](https://img.shields.io/badge/game-Survival%20Log-2b2b2b)
![framework](https://img.shields.io/badge/BepInEx-6%20IL2CPP-6a5acd)
![version](https://img.shields.io/badge/version-1.0.0-blue)
![license](https://img.shields.io/badge/license-PolyForm%20Noncommercial%201.0.0-orange)

> **非官方 Mod。** 与开发商 Midnight Workshop 及发行商 Lilith Games 无任何关联，也未获其认可或支持。
> 《生存日志》目前没有官方 mod 工具，也不支持创意工坊，官方曾明确提醒第三方来源的文件可能携带木马。
> 因此本项目以**可审计的源码**形式发布：你可以直接读代码、自己编译，或者使用本仓库 Releases 里的成品。请不要从其他任何地方下载。

## 功能说明

探索过程中，游戏在 `ExploreManager` 上维护一个暴露值（`ExposureValue`）。本 Mod 拦截对这个值的**所有写入**，按配置的倍率缩放。默认倍率 `0`，即暴露值完全不再增长。

匹配依据是类名与方法名，不是内存偏移，因此一些无关的游戏更新不会导致失效。

## 特性

- 覆盖**所有**增长来源：挂在属性 setter 上，而不是逐个拦截调用点
- 倍率可配置：`0`（冻结）到 `1`（原版），中间值对应变慢
- 无界面、不改存档文件、不起后台线程
- 失效即安全：游戏更新导致目标类型消失时，只写一条错误日志，不干扰游戏本身

## 前置要求

| | |
|---|---|
| 游戏 | 《生存日志》抢先体验版，Windows x64，IL2CPP 构建 |
| 加载器 | [BepInEx 6 **IL2CPP**（x64）](https://builds.bepinex.dev/projects/bepinex_be) —— BepInEx 5 **不可用** |
| 运行时 | .NET 6（BepInEx 6 IL2CPP 版本已自带） |

## 安装

1. 把 **BepInEx 6 IL2CPP x64** 装到游戏根目录，也就是 `SurvivalLog.exe` 所在的那一层。解压后应能在 exe 同级看到 `BepInEx\`、`winhttp.dll` 和 `doorstop_config.ini`。
2. 启动游戏一次再退出。BepInEx 会生成 `BepInEx\interop\`，IL2CPP 插件依赖这个目录。
3. 把 `NoExposure.dll` 放进 `BepInEx\plugins\`。
4. 启动游戏，查看 `BepInEx\LogOutput.log`，应出现：

   ```
   [Info   :   BepInEx] Loading [NoExposure 1.0.0]
   [Info   :NoExposure] NoExposure 1.0.0 loaded. ExposureGrowthRate = 0
   ```

5. 可选：**关闭游戏后**修改 `BepInEx\config\gzzheng.survival-log.noexposure.cfg`，再重新启动。

## 配置

文件位置：`BepInEx\config\gzzheng.survival-log.noexposure.cfg`

| 分组 | 键 | 类型 | 默认值 | 范围 | 说明 |
|---|---|---|---|---|---|
| `General` | `ExposureGrowthRate` | 浮点 | `0` | `0` – `1` | 每次增长时乘的倍率。`0` = 不增长，`1` = 原版 |

```ini
[General]

## Multiplier applied to every increase of the exposure value.
## 0 = never increases, 0.5 = grows slower, 1 = vanilla behaviour.
# Setting type: Single
# Default value: 0
# Acceptable value range: From 0 to 1
ExposureGrowthRate = 0
```

配置只在加载时读取一次，改完需要重启游戏。

## 卸载

删除 `BepInEx\plugins\NoExposure.dll`，如果需要彻底清理，再删掉 `BepInEx\config\gzzheng.survival-log.noexposure.cfg`。除此之外不产生任何文件。

## 关于存档

本 Mod 除了自己的配置文件，不读写磁盘上任何东西。它修改的数值存在于内存中，但如果游戏会把暴露值写进存档，那么存档里记录的就会是修改后的数值。

**首次使用前请备份存档。** Unity 存档一般在 `%USERPROFILE%\AppData\LocalLow\<开发商或游戏名>\` 下，具体目录随版本可能不同。

## 从源码构建

需要 [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)。

```powershell
dotnet build -c Release -p:BepInExDir="你的\BepInEx\路径"
```

`BepInExDir` 要指向包含 `core\BepInEx.Core.dll` 的那层目录。不传的话会依次回退到环境变量 `BEPINEX_DIR`、再回退到作者的默认路径 `C:\games\BepInEx`。

产物：`bin\Release\NoExposure.dll`。

## 仓库结构

| 路径 | 内容 |
|---|---|
| `Plugin.cs` | BepInEx 入口、配置绑定、Harmony 初始化 |
| `ExposePatches.cs` | 两个 Harmony 补丁 |
| `NoExposure.csproj` | 工程文件；BepInEx 程序集只引用、不复制 |

## 已知限制

- **中间值是近似效果。** setter 每次写入都会被缩放，如果游戏是以 `当前值 + 增量` 的方式写回，反复缩放会让结果趋近于「一个增量」，而不是把总量精确减半。`0` 和 `1` 是精确的，`0.5` 只能理解为「明显变慢」。
- **按名称挂钩。** 如果游戏更新改掉了 `ExploreManager`、`ExposureValue` 或 `AddExposure` 的名字，补丁会失效，此时日志会写 `Type not found`，Mod 不做任何事。
- **补丁冲突。** 其他同样修改这个 setter 的 mod 会与本 Mod 叠加，效果不可预期。排查问题时请只启用一个。

## 反馈问题

请到 [Issues](https://github.com/gzzheng/survival-log-noexposure/issues) 提交，并附上：

- 游戏版本号（Steam → 右键游戏 → 属性 → 已安装文件）
- Mod 版本
- `BepInEx\LogOutput.log` 里的相关行
- 配置文件内容与复现步骤

## 授权

**PolyForm Noncommercial License 1.0.0**，完整且具有法律效力的条款见 [LICENSE](LICENSE)。

用大白话说明：

- ✅ 任何**非商业**用途都可以免费使用、修改、分发——自己玩、研究、直播、录视频、收录在同样免费整合包里
- ❌ **禁止售卖**——不论是本 Mod 整体，还是任何修改版、衍生版，都不允许以任何形式售卖：付费下载、付费整合包、订阅或打赏解锁、以及任何以获取金钱为目的的分发
- 📄 再分发（无论是否修改）必须保留本许可证与版权声明，并注明来源仓库
- ⚠️ 不提供任何形式的担保

以上摘要仅为方便理解，若与 `LICENSE` 原文冲突，以 `LICENSE` 为准。

---

*《生存日志》（Survival Log）及其全部相关素材归 Midnight Workshop 与 Lilith Games 所有。本仓库不包含任何游戏代码、美术资源或数据文件。*
