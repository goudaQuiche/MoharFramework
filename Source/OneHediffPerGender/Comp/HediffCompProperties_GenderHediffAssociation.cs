using Verse;
using System.Collections.Generic;

namespace OHPG
{
    public class HediffCompProperties_GenderHediffAssociation : HediffCompProperties
    {
        public string race;
        public BodyPartDef bodyPartDef;
        public string bodyPartLabel;

        public List<Association> associations;
        public bool debug = false;
        
        public HediffCompProperties_GenderHediffAssociation()
        {
            this.compClass = typeof(HediffComp_GenderHediffAssociation);
        }
    }
}