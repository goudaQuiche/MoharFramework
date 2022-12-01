using Verse;
using RimWorld;
using UnityEngine;


namespace MoharBlood
{
    //[StaticConstructorOnStartup]
    public static class OverrideMaterialIfNeeded_Utils
    {
        public static Color BloodColorIfEligible(bool eligible, Color defaultColor, Color bloodColor)
        {
            return eligible ? bloodColor : defaultColor;
        }
    }
}
