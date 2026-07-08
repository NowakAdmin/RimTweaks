using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.DoorLock
{
    [HarmonyPatch(typeof(Building_Door), "CanPhysicallyPass")]
    public static class Patch_DoorLockPolicy
    {
        static void Postfix(Building_Door __instance, Pawn p, ref bool __result)
        {
            if (!__result) return; // already blocked by vanilla — don't interfere

            var comp = __instance.TryGetComp<CompDoorLockPolicy>();
            if (comp == null) return;

            __result = comp.CanPass(p);
        }
    }
}
