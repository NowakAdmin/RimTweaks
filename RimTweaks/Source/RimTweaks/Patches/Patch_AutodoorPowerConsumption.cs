using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.Patches
{
    [HarmonyPatch(typeof(CompPowerTrader), nameof(CompPowerTrader.PowerOutput), MethodType.Getter)]
    public static class Patch_AutodoorPowerConsumption
    {
        private const float ReductionFactor = 0.5f;

        private static ThingDef _autodoorDef = null!;
        private static ResearchProjectDef _research = null!;

        internal static void InitCache()
        {
            _autodoorDef = ThingDef.Named("Autodoor");
            _research = DefDatabase<ResearchProjectDef>.GetNamedSilentFail("NOAD_AdvancedAutodoor");
        }

        static void Postfix(CompPowerTrader __instance, ref float __result)
        {
            if (__instance.parent?.def != _autodoorDef) return;
            if (__result >= 0f) return;
            if (_research == null || !_research.IsFinished) return;
            __result *= ReductionFactor;
        }
    }
}
