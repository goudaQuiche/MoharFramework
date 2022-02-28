using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class StringFloatArgConditionMethods
    {

        //
        public static bool PawnHasNeedInRange(this Pawn p, List<string> strParam, List<FloatRange> floatParam)
        {
            if (p.needs == null)
                return false;

            if(p.needs.AllNeeds.Where( n => strParam.Contains( n.def.defName )).FirstOrFallback() is Need foundNeed)
            {
                return floatParam.Any(f => f.Includes(foundNeed.CurLevel));
            }
            return false;
        }

    }
}
