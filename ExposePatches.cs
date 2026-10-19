using BepInEx.Logging;
using HarmonyLib;

namespace NoExposure
{
    /// <summary>
    /// Hooks the two places where the game writes the exploration exposure value.
    ///
    /// 1. Primary: the property setter <c>set_ExposureValue(int)</c>. Every write
    ///    (action growth, passive growth, anything else) ends up there, so scaling
    ///    the incoming value here covers all sources at once.
    /// 2. Fallback: <c>AddExposure(int)</c>, the entry point used by exploration actions.
    /// </summary>
    public static class ExposePatches
    {
        // Confirmed against the game's IL2CPP dump:
        //   namespace : GameCore.HotUpdate.Battle.Logic
        //   class     : public class ExploreManager : BaseEntity, IEventDispatcher
        //   property  : public int ExposureValue { get; set; }
        //   growth    : public void AddExposure(int value)
        private const string OwnerClass   = "GameCore.HotUpdate.Battle.Logic.ExploreManager";
        private const string SetterMethod = "set_ExposureValue";
        private const string AddMethod    = "AddExposure";

        /// <summary>Multiplier applied to every exposure increase. 0 = frozen, 1 = vanilla.</summary>
        public static float Rate = 0f;

        public static void Apply(Harmony harmony, ManualLogSource log)
        {
            var type = AccessTools.TypeByName(OwnerClass);
            if (type == null)
            {
                log.LogError($"Type not found: {OwnerClass}. " +
                             "The game has probably been updated and this mod needs a matching update.");
                return;
            }

            // Primary hook: the setter intercepts every write to the value.
            var setter = AccessTools.Method(type, SetterMethod, new[] { typeof(int) });
            if (setter != null)
            {
                harmony.Patch(setter,
                    prefix: new HarmonyMethod(typeof(ExposePatches), nameof(ExposureValueSetPrefix)));
                log.LogInfo($"Patched: {OwnerClass}.{SetterMethod}(int)");
            }
            else
            {
                log.LogWarning($"Method not found: {SetterMethod}. Falling back to {AddMethod} only.");
            }

            // Fallback hook: the action-driven growth entry point.
            var add = AccessTools.Method(type, AddMethod, new[] { typeof(int) });
            if (add != null)
            {
                harmony.Patch(add,
                    prefix: new HarmonyMethod(typeof(ExposePatches), nameof(AddExposurePrefix)));
                log.LogInfo($"Patched: {OwnerClass}.{AddMethod}(int)");
            }
            else
            {
                log.LogWarning($"Method not found: {AddMethod}");
            }
        }

        // Scales every incoming value, whichever source wrote it. Rate 0 always yields 0.
        private static void ExposureValueSetPrefix(ref int __0)
        {
            __0 = (int)(__0 * Rate);
        }

        // Scales the delta passed to the action-driven growth method.
        private static void AddExposurePrefix(ref int __0)
        {
            __0 = (int)(__0 * Rate);
        }
    }
}
