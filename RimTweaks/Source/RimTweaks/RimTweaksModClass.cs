using UnityEngine;
using Verse;

namespace RimTweaks
{
    public class RimTweaksModClass : Mod
    {
        public static RimTweaksSettings Settings { get; private set; } = null!;

        public RimTweaksModClass(ModContentPack content) : base(content)
        {
            Settings = GetSettings<RimTweaksSettings>();
        }

        public override string SettingsCategory() => "RimTweaks by NoAd";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("Harmony patches (take effect immediately):");
            listing.Gap(4f);
            listing.CheckboxLabeled(
                "Door Lock Policy on Autodoor",
                ref Settings.enableDoorLockPolicy,
                "Adds a gizmo to Autodoors to restrict who can pass through.");
            listing.CheckboxLabeled(
                "Advanced Autodoor — 50% power reduction after research",
                ref Settings.enableAdvancedAutodoor,
                "Halves Autodoor power consumption once the Advanced Autodoor research is complete.");

            listing.Gap(12f);
            listing.Label("XML features (always active — use in-game to ignore them):");
            listing.Gap(4f);
            listing.Label("  • Production Server — workbench work speed +10%");
            listing.Label("  • RimCorn — high-fertility crop");
            listing.Label("  • Caravan Wings — implant for caravan speed & carry weight");

            listing.End();
        }
    }
}
