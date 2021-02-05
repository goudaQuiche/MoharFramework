using Verse;
using System.Collections.Generic;
using RimWorld;

namespace HEREHEGI
{
    public class HediffCompProperties_DataHediff : HediffCompProperties
    {
        public List<ReplaceHediffItem> replaceHediffs;
        public bool debug = false;

        public HediffCompProperties_DataHediff()
        {
            compClass = typeof(HediffComp_DataHediff);
        }
    }

    public class ReplaceHediffItem
    {
        public HediffDef inputH;
        public HediffDef outputH;
        public FloatRange chance = new FloatRange(1, 1);
        public bool destroy = false;

        public bool IsValid => inputH != null && ((outputH != null && destroy == false) || (outputH == null && destroy == true));
        public bool HasConsiderableChances => chance.min != 1 || chance.max != 1;
    }
}