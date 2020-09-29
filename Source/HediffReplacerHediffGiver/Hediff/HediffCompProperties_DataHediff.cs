using Verse;
using System.Collections.Generic;
using RimWorld;

namespace HEREHEGI
{
    public class HediffCompProperties_DataHediff : HediffCompProperties
    {

        public List<HediffDef> InputHediffPool;
        public List<HediffDef> OutputHediffPool;
        public List<float> HediffReplacementChance;

        public bool debug = false;

        public HediffCompProperties_DataHediff()
        {
            this.compClass = typeof(HediffComp_DataHediff);
        }
    }
}