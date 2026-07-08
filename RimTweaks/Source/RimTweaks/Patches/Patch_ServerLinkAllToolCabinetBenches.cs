using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.Patches
{
    [HarmonyPatch(typeof(CompAffectedByFacilities), "CanPotentiallyLinkTo_Static")]
    public static class Patch_ServerLinkAllToolCabinetBenches
    {
        static void Postfix(ThingDef facilityDef, ThingDef potentialBuildingDef, ref bool __result)
        {
            if (__result) return;
            if (facilityDef?.defName != "NOAD_ProductionServer") return;

            var affectedProps = potentialBuildingDef?.comps?
                .OfType<CompProperties_AffectedByFacilities>()
                .FirstOrDefault();

            if (affectedProps == null) return;

            __result = affectedProps.linkableFacilities.Any(f => f?.defName == "ToolCabinet");
        }
    }
}
