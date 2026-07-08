using RimWorld;
using System.Linq;
using Verse;

namespace RimTweaks
{
    /// <summary>
    /// Finds every ThingDef that accepts ToolCabinet as a facility and adds
    /// NOAD_ProductionServer to that same list. Runs at startup after all Defs are loaded,
    /// covering both vanilla and mod workbenches automatically.
    /// </summary>
    public static class ServerFacilityLinker
    {
        public static void LinkToAllToolCabinetBenches()
        {
            var serverDef = DefDatabase<ThingDef>.GetNamedSilentFail("NOAD_ProductionServer");
            if (serverDef == null)
            {
                Log.Error("[RimTweaks] NOAD_ProductionServer def not found — skipping facility linking.");
                return;
            }

            int count = 0;
            foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                var affectedComp = def.comps?
                    .OfType<CompProperties_AffectedByFacilities>()
                    .FirstOrDefault();

                if (affectedComp == null) continue;
                if (!affectedComp.linkableFacilities.Contains(serverDef) &&
                    affectedComp.linkableFacilities.Any(f => f?.defName == "ToolCabinet"))
                {
                    affectedComp.linkableFacilities.Add(serverDef);
                    count++;
                }
            }

            Log.Message($"[RimTweaks] ProductionServer linked to {count} workbench(es).");
        }
    }
}
