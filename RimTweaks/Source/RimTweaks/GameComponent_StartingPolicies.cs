using RimWorld;
using Verse;

namespace RimTweaks
{
    /// <summary>
    /// GameComponent that adds custom apparel policies once when a new game starts.
    /// Must be registered in a GameComponentDef XML.
    /// </summary>
    public class GameComponent_StartingPolicies : GameComponent
    {
        public GameComponent_StartingPolicies(Game game) { }

        public override void StartedNewGame()
        {
            var db = Current.Game.outfitDatabase;
            AddPolicy_NoTaintedBelow51(db);
        }

        private static void AddPolicy_NoTaintedBelow51(OutfitDatabase db)
        {
            var policy = db.MakeNewOutfit();
            policy.label = "No tainted / worn-out";

            // Block deadman's (tainted) apparel
            var allowDeadmans = DefDatabase<SpecialThingFilterDef>.GetNamed("AllowDeadmansApparel");
            policy.filter.SetAllow(allowDeadmans, false);

            // Block items below 51% durability
            policy.filter.AllowedHitPointsPercents = new FloatRange(0.51f, 1f);
        }
    }
}
