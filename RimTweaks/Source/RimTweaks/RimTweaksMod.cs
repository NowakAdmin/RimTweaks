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

            ServerFacilityLinker.LinkToAllToolCabinetBenches();
        }
    }
}
