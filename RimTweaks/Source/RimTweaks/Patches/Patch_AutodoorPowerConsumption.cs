using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.Patches
{
    /// <summary>
    /// After research "NOAD_AdvancedAutodoor" is completed, all Autodoors use 50% less power.
    /// Patches CompPowerTrader.PowerOutput (getter) to halve consumption when research is done.
    /// </summary>
    [HarmonyPatch(typeof(CompPowerTrader), nameof(CompPowerTrader.PowerOutput), MethodType.Getter)]
    public static class Patch_AutodoorPowerConsumption
    {
        private const string ResearchDefName = "NOAD_AdvancedAutodoor";
        private const float ReductionFactor = 0.5f;

        static void Postfix(CompPowerTrader __instance, ref float __result)
        {
            // Only apply to Autodoor
            if (__instance.parent?.def?.defName != "Autodoor")
                return;

            // Only when consuming (negative value = drawing power)
            if (__result >= 0f)
                return;

            // Only when research is complete
            var research = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(ResearchDefName);
            if (research == null || !research.IsFinished)
                return;

            __result *= ReductionFactor;
        }
    }
}
