using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimTweaks.DoorLock
{
    public class CompDoorLockPolicy : ThingComp
    {
        public DoorLockPolicy Policy = DoorLockPolicy.Default;

        private static readonly (DoorLockPolicy policy, string label)[] Policies =
        {
            (DoorLockPolicy.Default,                          "Default"),
            (DoorLockPolicy.Everyone,                         "Everyone"),
            (DoorLockPolicy.ColonistsAndAnimals,              "Only colonists and animals"),
            (DoorLockPolicy.ColonistsAnimalsAndFriendlies,    "Only colonists, animals and friendlies"),
            (DoorLockPolicy.DraftedOnly,                      "Only drafted"),
        };

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref Policy, "NOAD_doorLockPolicy", DoorLockPolicy.Default);
        }

        public bool CanPass(Pawn pawn)
        {
            if (pawn == null) return true;
            return Policy switch
            {
                DoorLockPolicy.Default                       => true,
                DoorLockPolicy.Everyone                      => true,
                DoorLockPolicy.ColonistsAndAnimals           => IsColonistOrAnimal(pawn),
                DoorLockPolicy.ColonistsAnimalsAndFriendlies => IsColonistOrAnimal(pawn) || IsFriendly(pawn),
                DoorLockPolicy.DraftedOnly                   => pawn.Drafted && pawn.Faction == Faction.OfPlayer,
                _                                            => true,
            };
        }

        private static bool IsColonistOrAnimal(Pawn pawn) =>
            pawn.Faction == Faction.OfPlayer || pawn.RaceProps.Animal;

        private static bool IsFriendly(Pawn pawn) =>
            pawn.Faction != null &&
            pawn.Faction != Faction.OfPlayer &&
            !pawn.Faction.HostileTo(Faction.OfPlayer);

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = $"Door policy: {CurrentLabel()}",
                defaultDesc  = "Click to change who can pass through this door.",
                icon         = ContentFinder<Texture2D>.Get("UI/Commands/ForbidOff", false)
                               ?? BaseContent.BadTex,
                action       = CyclePolicy,
            };
        }

        private void CyclePolicy()
        {
            int next = ((int)Policy + 1) % Policies.Length;
            Policy = Policies[next].policy;
        }

        private string CurrentLabel()
        {
            foreach (var (policy, label) in Policies)
                if (policy == Policy) return label;
            return Policy.ToString();
        }
    }
}
