using Verse;
using System.Collections.Generic;

namespace MoHarRegeneration
{
    public class HediffCompProperties_Regeneration : HediffCompProperties
    {
        public int CheckingTicksPeriod = 600;

        public RegenParams PhysicalHediff = null;
        public RegenParams ChemicalHediff = null;
        public RegenParams DiseaseHediff = null;

        // special case, hediffDefs are not needed
        public RegenParamsSpecial PermanentInjury = null;
        public RegenParamsSpecial BodyPartRegeneration = null;
        //
        public BleedTendingParams BleedingHediff = null;
        public ChronicTendingParams ChronicHediff = null;

        public bool debug = false;

        public HediffCompProperties_Regeneration()
        {
            this.compClass = typeof(HediffComp_Regeneration);
        }
    }
}