# RimTweaks by NoAd — RimWorld

Personal compilation mod. Adds cherry-picked features from various mods to avoid installing bloated mods for single mechanics.

## Structure

```
RimTweaks/
├── About/          — mod metadata (About.xml)
├── Defs/
│   ├── ThingDefs_Items/      — items, resources, weapons, apparel
│   ├── ThingDefs_Buildings/  — buildings and furniture
│   ├── RecipeDefs/           — crafting recipes
│   └── ResearchProjectDefs/  — research tree additions
├── Patches/        — XPath patches to modify vanilla/other mod Defs
├── Textures/       — PNG textures referenced in Defs
└── Languages/
    └── English/Keyed/        — translation keys
```

## Naming convention

All defNames use the prefix `NOAD_` to avoid conflicts.

## Dependencies

- **Vanilla Expanded Framework** (VEF) — required for door lock policy

## Features

### Door Lock Policy on vanilla Autodoor
`Patches/VanillaDoors_LockPolicy.xml`

XPath patch that changes vanilla `Autodoor` thingClass to `VEF.Buildings.Building_AutoDoorLockable`.
Right-click the door in-game to cycle through lock policies:
- **Default** — vanilla AI behavior
- **Everyone** — all pawns pass freely
- **Only colonists and animals** — blocks visitors/hostiles
- **Only colonists, animals and friendlies**
- **Only drafted** — blocks everyone except drafted colonists
