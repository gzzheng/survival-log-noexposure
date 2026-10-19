# NoExposure

**English** · [简体中文](README.zh-CN.md)

Control the growth of the exploration **exposure value** in *Survival Log* — freeze it completely, or just make it build up more slowly.

![game](https://img.shields.io/badge/game-Survival%20Log-2b2b2b)
![framework](https://img.shields.io/badge/BepInEx-6%20IL2CPP-6a5acd)
![version](https://img.shields.io/badge/version-1.0.0-blue)
![license](https://img.shields.io/badge/license-PolyForm%20Noncommercial%201.0.0-orange)

> **Unofficial mod.** Not affiliated with, endorsed by, or supported by Midnight Workshop or Lilith Games.
> *Survival Log* has no official modding tools and no Steam Workshop support, and the developer has warned that untrusted third-party files can carry malware.
> This mod is therefore published as **auditable source code**: read it, build it yourself, or use the binary from this repository's Releases. Do not download it from anywhere else.

## What it does

While you explore, the game keeps an `ExposureValue` on its `ExploreManager`. This mod patches every write to that value and scales it by a configurable rate. At the default rate of `0`, the value simply never goes up.

The value is matched by class and member name, not by memory offsets, so the patch is resilient to unrelated game updates.

## Features

- Works on **all** sources of exposure growth, because it hooks the property setter rather than individual call sites
- Configurable rate: `0` (frozen) to `1` (vanilla), with intermediate values for a slower build-up
- No UI, no save-file editing, no background threads
- Fails safe: if the game is updated and the target type disappears, the mod logs an error and leaves the game untouched

## Requirements

| | |
|---|---|
| Game | *Survival Log*, Windows x64, IL2CPP build |
| Game version | `1.0.0` — other builds are untested |
| Loader | [BepInEx 6 **IL2CPP** (x64)](https://builds.bepinex.dev/projects/bepinex_be) — BepInEx 5 will **not** work |
| Runtime | .NET 6 (bundled with the BepInEx 6 IL2CPP build) |

## Installation

1. Install **BepInEx 6 IL2CPP x64** into the game folder — the folder that contains `SurvivalLog.exe`. Extract the archive there so you get `BepInEx\`, `winhttp.dll` and `doorstop_config.ini` next to the executable.
2. Launch the game once, then quit. BepInEx generates `BepInEx\interop\`, which IL2CPP plugins require.
3. Copy `NoExposure.dll` into `BepInEx\plugins\`.
4. Start the game and check `BepInEx\LogOutput.log`. You should see:

   ```
   [Info   :   BepInEx] Loading [NoExposure 1.0.0]
   [Info   :NoExposure] NoExposure 1.0.0 loaded. ExposureGrowthRate = 0
   ```

5. Optional: edit `BepInEx\config\gzzheng.survival-log.noexposure.cfg` **while the game is closed**, then restart.

## Configuration

File: `BepInEx\config\gzzheng.survival-log.noexposure.cfg`

| Section | Key | Type | Default | Range | Meaning |
|---|---|---|---|---|---|
| `General` | `ExposureGrowthRate` | float | `0` | `0` – `1` | Multiplier applied to every increase. `0` = frozen, `1` = vanilla |

```ini
[General]

## Multiplier applied to every increase of the exposure value.
## 0 = never increases, 0.5 = grows slower, 1 = vanilla behaviour.
# Setting type: Single
# Default value: 0
# Acceptable value range: From 0 to 1
ExposureGrowthRate = 0
```

The value is read once at load time — changes take effect on the next launch.

## Uninstall

Delete `BepInEx\plugins\NoExposure.dll` and, if you want a clean slate, `BepInEx\config\gzzheng.survival-log.noexposure.cfg`. No other files are created.

## Saving

The mod never edits files on disk other than its own config. Values it changes exist in memory, but if the game serialises the exposure value into a save, that save will contain the modified number.

**Back up your saves before the first run.** Unity saves normally live under `%USERPROFILE%\AppData\LocalLow\<developer or game name>\`; the exact folder varies between builds.

## Building from source

Requires the [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).

```powershell
dotnet build -c Release -p:BepInExDir="C:\path\to\your\BepInEx"
```

`BepInExDir` must point at the folder that contains `core\BepInEx.Core.dll`. If you skip it, the project falls back to `C:\games\BepInEx`, or to the `BEPINEX_DIR` environment variable when that is set.

Output: `bin\Release\NoExposure.dll`.

## Repository layout

| Path | Contents |
|---|---|
| `Plugin.cs` | BepInEx entry point, config binding, Harmony bootstrap |
| `ExposePatches.cs` | The two Harmony patches |
| `NoExposure.csproj` | Project file; BepInEx assemblies are referenced, never copied |

## Known limitations

- **Intermediate values are approximate.** The setter is scaled on every write, so if the game writes `current + delta`, repeated scaling converges towards roughly one `delta` rather than halving the running total. `0` and `1` are exact; `0.5` is "noticeably slower", not a precise 50 %.
- **Name-based hooking.** A game update that renames `ExploreManager`, `ExposureValue` or `AddExposure` makes the hook fail. The mod then logs `Type not found` and does nothing.
- **Patch conflicts.** Any other mod that patches the same setter will stack with this one; results are unpredictable. Run one at a time when debugging.

## Reporting issues

Open an [issue](https://github.com/gzzheng/survival-log-noexposure/issues) and include:

- game build number (Steam → right-click the game → Properties → Installed Files)
- mod version
- the relevant lines from `BepInEx\LogOutput.log`
- your config file contents, and steps to reproduce

## Support

This mod is free, and it stays free. If it saved you some annoyance and you feel like saying thanks, you can buy me a coffee on 爱发电 (Alipay / WeChat Pay):

**[→ 爱发电](https://afdian.com/a/YOUR_ID)**

Donations are voluntary and buy nothing: no extra features, no early builds, no priority support. Every feature is the same for everyone.

## License

**PolyForm Noncommercial License 1.0.0** — see [LICENSE](LICENSE) for the full, binding text.

In plain language:

- ✅ Free to use, modify and redistribute for any **noncommercial** purpose — personal use, study, streaming, videos, modpacks that are themselves free
- ❌ **Selling is not allowed** — neither the mod as a whole nor any modified or derived version, in any form: paid downloads, paid bundles, subscription- or donation-gated distribution, or anything else where money changes hands for the software
- 📄 Redistribution, with or without changes, must keep this licence and the copyright notice, and must point back to this repository
- ⚠️ No warranty of any kind

The summary above is a convenience only. If the two ever disagree, the `LICENSE` file wins.

---

*Survival Log and all related assets are the property of Midnight Workshop and Lilith Games. This repository contains no game code, assets or data files.*
