using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class IntRangeArgConditionMethods
    {

        //
        public static bool DayOfYearWithin(this Pawn p, List<IntRange> parameters)
        {
            if (p.Map == null)
                return false;

            int val = GenLocalDate.DayOfYear(p);

            return parameters.Any(ir => ir.min >= val && ir.max <= val);
        }

        public static bool HourOfDayWithin(this Pawn p, List<IntRange> parameters)
        {
            if (p.Map == null)
                return false;

            int val = GenLocalDate.HourOfDay(p);

            return parameters.Any(ir => ir.min >= val && ir.max <= val);
        }

        public static bool DayOfTwelfthWithin(this Pawn p, List<IntRange> parameters)
        {
            if (p.Map == null)
                return false;

            int val = GenLocalDate.DayOfTwelfth(p);

            return parameters.Any(ir => ir.min >= val && ir.max <= val);
        }

        public static bool DayOfSeasonWithin(this Pawn p, List<IntRange> parameters)
        {
            if (p.Map == null)
                return false;

            int val = GenLocalDate.DayOfSeason(p);

            return parameters.Any(ir => ir.min >= val && ir.max <= val);
        }
        public static bool DayOfQuadrumWithin(this Pawn p, List<IntRange> parameters)
        {
            if (p.Map == null)
                return false;

            int val = GenLocalDate.DayOfQuadrum(p);

            return parameters.Any(ir => ir.min >= val && ir.max <= val);
        }

        public static bool TwelfthWithin(this Pawn p, List<IntRange> parameters)
        {
            if (p.Map == null)
                return false;

            int val = (int)GenLocalDate.Twelfth(p);

            return parameters.Any(ir => ir.min >= val && ir.max <= val);
        }
    }
}
