using Verse;
using System.Collections.Generic;

namespace MoHarRegeneration
{
    public class HediffCompProperties_Regeneration : HediffCompProperties
    {
        public int CheckingTicksPeriod = 600;

        //Tending
        public TendingParams BloodLossTendingParams = null;
        public TendingParams ChronicHediffTendingParams = null;
        public TendingParams RegularDiseaseTendingParams = null;

        public RegenParams PhysicalInjuryRegenParams = null;
        public RegenParams ChemicalHediffRegenParams = null;
        public RegenParams DiseaseHediffRegenParams = null;

        // special case, hediffDefs are not needed
        public RegenParamsSpecial PermanentInjuryRegenParams = null;
        public RegenParamsSpecial BodyPartRegenParams = null;
        


        public bool debug = false;

        public HediffCompProperties_Regeneration()
        {
            this.compClass = typeof(HediffComp_Regeneration);
        }
    }
}