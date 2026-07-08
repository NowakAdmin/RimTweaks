using HarmonyLib;
using Verse;

namespace RimTweaks
{
    [StaticConstructorOnStartup]
    public static class RimTweaksMod
    {
        static RimTweaksMod()
        {
            var harmony = new Harmony("NoAd.RimTweaks");
            harmony.PatchAll();
            Log.Message("[RimTweaks] Harmony patches applied.");

            // DEBUG: verify Autodoor thingClass after patch
            var autodoorDef = DefDatabase<ThingDef>.GetNamedSilentFail("Autodoor");
            if (autodoorDef != null)
                Log.Message($"[RimTweaks] Autodoor.thingClass = {autodoorDef.thingClass?.FullName ?? "NULL"}");
            else
                Log.Warning("[RimTweaks] Autodoor def not found!");
        }
    }
}
