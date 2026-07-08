using Verse;

namespace RimTweaks
{
    public class RimTweaksSettings : ModSettings
    {
        public bool enableDoorLockPolicy   = true;
        public bool enableAdvancedAutodoor = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableDoorLockPolicy,   "enableDoorLockPolicy",   true);
            Scribe_Values.Look(ref enableAdvancedAutodoor, "enableAdvancedAutodoor", true);
            base.ExposeData();
        }
    }
}
