using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace Ubet
{
    public static class Conditions
    {
        //
        public static bool ConditionCheck(this Thing t, Condition c, bool debug = false)
        {
            if (debug)
            {
                string str = "ConditionCheck - " + c.type.DescriptionAttr();
                if (!c.parameter.NullOrEmpty())
                {
                    str += ". Parameters:";
                    foreach (string s in c.parameter)
                        str += s + "; ";
                }
                    
                Log.Warning(str);
            }
            //Log.Warning(((Ubet.Condition)c).GetEnumDescription());
            //Ubet.Condition.GetEnumDescription(c))

            switch (c.type) {
                case ConditionType.isPawn:
                    return t.ThingIsPawn();

                case ConditionType.isHuman:
                    return ((Pawn)t).PawnIsHuman();

                case ConditionType.isMale:
                    return ((Pawn)t).PawnIsMale();
                case ConditionType.isFemale:
                    return ((Pawn)t).PawnIsFemale();

                case ConditionType.isDrafted:
                    return ((Pawn)t).PawnIsDrafted();
                case ConditionType.isUndrafted:
                    return ((Pawn)t).PawnIsUndrafted();

                case ConditionType.belongsToLifeStage:
                    return ((Pawn)t).PawnBelongsToLifeStage(c.parameter);
                case ConditionType.isPawnKind:
                    return ((Pawn)t).PawnIsPawnKind(c.parameter);
                case ConditionType.hasTrait:
                    return ((Pawn)t).PawnHasTrait(c.parameter);
                case ConditionType.isPerformingJob:
                    return ((Pawn)t).PawnIsPerformingJob(c.parameter);

                case ConditionType.isOnMapWithWeather:
                    return ((Pawn)t).PawnMapWeather(c.parameter);
                case ConditionType.isOnMapWithSeason:
                    return ((Pawn)t).PawnMapSeason(c.parameter);

                default:
                    return false;
            }
        }

        //
        public static bool ThingIsPawn(this Thing t)
        {
            return t is Pawn;
        }

        public static bool PawnIsHuman(this Pawn p)
        {
            return p.def == ThingDefOf.Human;
        }

        public static bool PawnIsMale(this Pawn p)
        {
            return p.gender == Gender.Male;
        }

        public static bool PawnIsFemale(this Pawn p)
        {
            return p.gender == Gender.Female;
        }

        public static bool PawnIsDrafted(this Pawn p)
        {
            return p.Drafted;
        }

        public static bool PawnIsUndrafted(this Pawn p)
        {
            return !p.Drafted;
        }
    }
}
