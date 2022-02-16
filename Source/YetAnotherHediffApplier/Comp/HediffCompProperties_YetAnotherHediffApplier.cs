using Verse;
using System.Collections.Generic;
using RimWorld;

namespace YAHA
{
    public class HediffCompProperties_YetAnotherHediffApplier : HediffCompProperties
    {
        public List<HediffAssociation> associations;

        public int checkFrequency = 1800;
        public bool debug = false;

        public bool PeriodicCheck => checkFrequency > 0;

        public HediffCompProperties_YetAnotherHediffApplier()
        {
            compClass = typeof(HediffComp_YetAnotherHediffApplier);
        }
    }
}