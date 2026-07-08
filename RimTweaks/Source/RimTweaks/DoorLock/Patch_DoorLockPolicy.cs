using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.DoorLock
{
    [HarmonyPatch(typeof(Building_Door), "CanPhysicallyPass")]
    public static class Patch_DoorLockPolicy
    {
        private static ThingDef _autodoorDef = null!;

        internal static void InitCache()
        {
            _autodoorDef = ThingDef.Named("Autodoor");
        }

        static void Postfix(Building_Door __instance, Pawn p, ref bool __result)
        {
            if (!__result) return;
            if (!RimTweaksModClass.Settings.enableDoorLockPolicy) return;
            if (__instance.def != _autodoorDef) return;

            var comp = __instance.TryGetComp<CompDoorLockPolicy>();
            if (comp == null) return;

            __result = comp.CanPass(p);
        }
    }
}
