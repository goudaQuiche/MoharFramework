using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenerationUtility
    {
        public enum HealingTask
        {
            [Description("None")]
            None = 0,
            [Description("Bleeding tending")]
            BleedingTending = 1,
            [Description("Injury regeneration")]
            InjuryRegeneration = 2,
            [Description("Chemical removal")]
            ChemicalRemoval = 3,
            [Description("Disease healing")]
            DiseaseHealing = 4,
            [Description("Permanent injury regeneration")]
            PermanentInjuryRegeneration = 5,
            [Description("Body part regeneration")]
            BodyPartRegeneration = 6
        }

        public static void TryTendBleeding(this HediffComp_Regeneration RegenHComp)
        {

        }
    }
}
