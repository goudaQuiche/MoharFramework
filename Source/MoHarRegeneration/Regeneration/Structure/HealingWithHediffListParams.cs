using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class HealingWithHediffListParams : HealingParams
    {
        public List<HediffDef> TargetedHediffDefs;
    }
}
