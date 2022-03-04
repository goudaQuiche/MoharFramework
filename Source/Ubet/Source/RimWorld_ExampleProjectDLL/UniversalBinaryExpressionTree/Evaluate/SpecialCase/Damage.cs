using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class DamageTaken
    {

        //
        public static bool PawnGotWounded(DamageInfo dinfo, float totalDamageDealt, List<string> damageType, List<FloatRange> floatRanges)
        {
            if (damageType.NullOrEmpty() && floatRanges.NullOrEmpty())
                return true;

            if (damageType.NullOrEmpty() && !floatRanges.NullOrEmpty())
            {
                return floatRanges.Any(f => f.Includes(totalDamageDealt));
            }

            if (!damageType.NullOrEmpty() && floatRanges.NullOrEmpty())
            {
                return damageType.Contains(dinfo.Def.defName);
            }

            if (damageType.Count != floatRanges.Count)
                return false;

            for (int i=0; i<damageType.Count; i++)
            {
                if (damageType[i] == dinfo.Def.defName && floatRanges[i].Includes(totalDamageDealt))
                    return true;
            }

            return false;
        }

    }
}
