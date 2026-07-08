using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.Patches
{
    [HarmonyPatch(typeof(CompAffectedByFacilities), "CanPotentiallyLinkTo_Static")]
    public static class Patch_ServerLinkAllToolCabinetBenches
    {
        private static ThingDef _serverDef = null!;
        private static HashSet<ThingDef> _eligibleDefs = null!;

        internal static void InitCache()
        {
            _serverDef = DefDatabase<ThingDef>.GetNamedSilentFail("NOAD_ProductionServer");
            _eligibleDefs = new HashSet<ThingDef>();

            var toolCabinetDef = DefDatabase<ThingDef>.GetNamedSilentFail("ToolCabinet");
            if (toolCabinetDef == null) return;

            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                var affectedProps = def.comps?
                    .OfType<CompProperties_AffectedByFacilities>()
                    .FirstOrDefault();

                if (affectedProps?.linkableFacilities != null &&
                    affectedProps.linkableFacilities.Contains(toolCabinetDef))
                {
                    _eligibleDefs.Add(def);
                }
            }

            Log.Message($"[RimTweaks] ProductionServer eligible workbenches cached: {_eligibleDefs.Count}");
        }

        static void Postfix(ThingDef facilityDef, ThingDef potentialBuildingDef, ref bool __result)
        {
            if (__result) return;
            if (facilityDef != _serverDef) return;
            __result = potentialBuildingDef != null && _eligibleDefs.Contains(potentialBuildingDef);
        }
    }
}
