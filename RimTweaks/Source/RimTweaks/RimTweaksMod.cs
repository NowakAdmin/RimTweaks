using HarmonyLib;
using RimTweaks.DoorLock;
using RimTweaks.Patches;
using Verse;

namespace RimTweaks
{
    [StaticConstructorOnStartup]
    public static class RimTweaksStartup
    {
        static RimTweaksStartup()
        {
            var harmony = new Harmony("NoAd.RimTweaks");
            harmony.PatchAll();

            Patch_DoorLockPolicy.InitCache();
            Patch_AutodoorPowerConsumption.InitCache();
            Patch_ServerLinkAllToolCabinetBenches.InitCache();

            Log.Message("[RimTweaks] Initialized.");
        }
    }
}
