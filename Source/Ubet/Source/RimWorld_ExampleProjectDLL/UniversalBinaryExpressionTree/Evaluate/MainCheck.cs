using Verse;
using System;
using System.Collections.Generic;

namespace Ubet
{
    public static class ConditionCheck
    {
        public static bool MainCheck(this Thing t, Condition c, bool debug = false)
        {
            if (debug)
            {
                string str = "MainCheck - " + c.Description;
                if (!c.stringArg.NullOrEmpty())
                {
                    str += ". Parameters:";
                    foreach (string s in c.stringArg)
                        str += s + "; ";
                }

                Log.Warning(str);
            }

            if (c.HasNoArg)
            {
                Func<Pawn, bool> myCall = ConditionDictionnary.noArgconditions.TryGetValue(c.type);
                if (myCall == null)
                {
                    if (debug)
                        Log.Warning("could not find no arg function for " + c.type.ToString() + "(" + c.Description + ")");
                    return false;
                }
                return myCall((Pawn)t);
            }
            else if (c.HasStringArg)
            {
                Func<Pawn, List<string>, bool> myCall = ConditionDictionnary.StringArgconditions.TryGetValue(c.type);
                if (myCall == null)
                {
                    if (debug)
                        Log.Warning("could not find string arg function for " + c.type.ToString() + "(" + c.Description + ")");
                    return false;
                }
                return myCall((Pawn)t, c.stringArg);
            }
            else if (c.HasIntArg)
            {
                Func<Pawn, List<IntRange>, bool> myCall = ConditionDictionnary.IntRangeArgconditions.TryGetValue(c.type);
                if (myCall == null)
                {
                    if (debug)
                        Log.Warning("could not find intrange arg function for " + c.type.ToString() + "(" + c.type.DescriptionAttr() + ")");
                    return false;
                }
                return myCall((Pawn)t, c.intArg);
            }

            if (debug)
                Log.Warning("MainCheck - This should never be reached");

            return false;
        }

        //
        public static bool OldMainCheck(this Thing t, Condition c, bool debug = false)
        {
            if (debug)
            {
                string str = "MainCheck - " + c.type.DescriptionAttr();
                if (!c.stringArg.NullOrEmpty())
                {
                    str += ". Parameters:";
                    foreach (string s in c.stringArg)
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

                //Gender
                case ConditionType.isMale:
                    return ((Pawn)t).PawnIsMale();
                case ConditionType.isFemale:
                    return ((Pawn)t).PawnIsFemale();

                //Activity
                case ConditionType.isDrafted:
                    return ((Pawn)t).PawnIsDrafted();
                case ConditionType.isUndrafted:
                    return ((Pawn)t).PawnIsUndrafted();
                case ConditionType.isPerformingJob:
                    return ((Pawn)t).PawnIsPerformingJob(c.stringArg);

                // Pawn nature
                case ConditionType.belongsToLifeStage:
                    return ((Pawn)t).PawnBelongsToLifeStage(c.stringArg);
                case ConditionType.isPawnKind:
                    return ((Pawn)t).PawnIsPawnKind(c.stringArg);
                case ConditionType.hasTrait:
                    return ((Pawn)t).PawnHasTrait(c.stringArg);
                case ConditionType.hasBackstory:
                    return ((Pawn)t).PawnHasBackstory(c.stringArg);


                //Environment
                case ConditionType.isOnMapWithWeather:
                    return ((Pawn)t).PawnMapWeather(c.stringArg);
                case ConditionType.isOnMapWithSeason:
                    return ((Pawn)t).PawnMapSeason(c.stringArg);

                //Relations
                case ConditionType.hasDeadRelation:
                    return ((Pawn)t).PawnHasDeadRelation(c.stringArg);
                case ConditionType.hasAliveRelation:
                    return ((Pawn)t).PawnHasAliveRelation(c.stringArg);

                //Condition
                case ConditionType.hasBodyPart:
                    return ((Pawn)t).PawnHasBodyPart(c.stringArg);

                //Equipment
                case ConditionType.wearsApparelMadeOf:
                    return ((Pawn)t).PawnWearsApparelMadeOf(c.stringArg);
                case ConditionType.usesWeaponMadeOf:
                    return ((Pawn)t).PawnUsesWeaponMadeOf(c.stringArg);

                default:
                    return false;
            }
        }

    }
}
