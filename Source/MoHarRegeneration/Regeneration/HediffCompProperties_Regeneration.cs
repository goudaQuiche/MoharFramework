using Verse;
using System.Collections.Generic;

namespace MoHarRegeneration
{
    public class HediffCompProperties_Regeneration : HediffCompProperties
    {
        public int CheckingTicksPeriod = 600;

        //Tending
        public HealingParams BloodLossTendingParams = null;
        public HealingParams ChronicHediffTendingParams = null;
        public HealingParams RegularDiseaseTendingParams = null;

        public HealingWithHediffListParams PhysicalInjuryRegenParams = null;
        public HealingWithHediffListParams ChemicalHediffRegenParams = null;
        public HealingWithHediffListParams DiseaseHediffRegenParams = null;

        // special case, hediffDefs are not needed
        public HealingWithMaxParams PermanentInjuryRegenParams = null;
        public HealingWithMaxParams BodyPartRegenParams = null;

        public bool debug = false;

        public HediffCompProperties_Regeneration()
        {
            this.compClass = typeof(HediffComp_Regeneration);
        }
    }
}