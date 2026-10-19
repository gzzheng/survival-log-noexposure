using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace NoExposure
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            // Config lives at BepInEx/config/<guid>.cfg
            var config = new ConfigFile(Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID + ".cfg"), true);

            ExposePatches.Rate = config.Bind(
                "General",
                "ExposureGrowthRate",
                0f,
                new ConfigDescription(
                    "Multiplier applied to every increase of the exposure value. " +
                    "0 = never increases, 0.5 = grows slower, 1 = vanilla behaviour.",
                    new AcceptableValueRange<float>(0f, 1f))).Value;

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            ExposePatches.Apply(harmony, Log);

            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded. " +
                        $"ExposureGrowthRate = {ExposePatches.Rate}");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID    = "gzzheng.survival-log.noexposure";
        public const string PLUGIN_NAME    = "NoExposure";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
