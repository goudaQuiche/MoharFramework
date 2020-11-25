using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public class HediffItem
    {
        //public string label;

        public HediffDef hediffDef;
        public FloatRange applyChance = new FloatRange(1, 1);
        public FloatRange severity = new FloatRange(.1f, .2f);
        public float weight = 1;

        public HediffCondition specificCondition;

        public bool HasSpecific => specificCondition != null;
    }
}
