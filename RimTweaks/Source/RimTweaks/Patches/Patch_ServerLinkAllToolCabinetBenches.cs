using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimTweaks.Patches
{
    // CanPotentiallyLinkTo_Static has two overloads — specify parameter types to avoid AmbiguousMatchException
    [HarmonyPatch(typeof(CompAffectedByFacilities), "CanPotentiallyLinkTo_Static",
        new[] { typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(Map) })]
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

            Log.Message($"[RimTweaks] ProductionServer eligible workbenches: {_eligibleDefs.Count}");
        }

        // Parameter names must match the actual method: facilityDef, myDef
        static void Postfix(ThingDef facilityDef, ThingDef myDef, ref bool __result)
        {
            if (__result) return;
            if (facilityDef != _serverDef) return;
            __result = myDef != null && _eligibleDefs.Contains(myDef);
        }
    }
}
