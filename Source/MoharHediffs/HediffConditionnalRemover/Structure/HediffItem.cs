using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public class HediffItemToRemove
    {
        public HediffDef hediffDef;
        public HediffKeepingCondition specificCondition;

        public bool HasSpecificCondition => specificCondition != null;
    }
}
