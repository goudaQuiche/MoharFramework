using Verse;
using System.Collections.Generic;
using RimWorld;

namespace OHPLS
{
    public class HediffCompProperties_LifeStageHediffAssociation : HediffCompProperties
    {
        public string race;
        public BodyPartDef bodyPartDef;
        public string bodyPartLabel;

        public List<Association> associations;
        public bool debug = false;

        public HediffCompProperties_LifeStageHediffAssociation()
        {
            this.compClass = typeof(HediffComp_LifeStageHediffAssociation);
        }
    }
}