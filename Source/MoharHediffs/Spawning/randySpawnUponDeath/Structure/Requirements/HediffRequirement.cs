using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class HediffRequirementSettings
    {
        public HediffDef hediffDef;
        public FloatRange severity = new FloatRange(0,1);

        public bool HasHediffDef => hediffDef != null;
    }
}