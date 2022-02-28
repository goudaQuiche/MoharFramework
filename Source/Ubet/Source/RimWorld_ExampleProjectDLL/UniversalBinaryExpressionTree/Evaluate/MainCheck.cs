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
                ParametersDump(c);

            if (!(((Pawn)t) is Pawn p))
            {
                if (debug) Log.Warning("MainCheck - thing was not pawn, returning false");
                return false;
            }
                

            if (c.HasNoArg)
            {
                Func<Pawn, bool> myCall = ConditionDictionnary.noArgconditions.TryGetValue(c.type);
                if (MethodIsNullAndDebug(c, "no arg method", myCall == null, debug))
                    return false;
                
                return myCall(p);
            }
            else if (c.Has1StringArg)
            {
                Func<Pawn, List<string>, bool> myCall = ConditionDictionnary.StringArgconditions.TryGetValue(c.type);
                if (MethodIsNullAndDebug(c, "1 string arg method", myCall == null, debug))
                    return false;
                
                return myCall(p, c.stringArg[0]);
            }
            else if (c.Has2StringArg)
            {
                Func<Pawn, List<string>, List<string>, bool> myCall = ConditionDictionnary.TwoStringArgconditions.TryGetValue(c.type);
                if (MethodIsNullAndDebug(c, "2 string arg method", myCall == null, debug))
                    return false;
                
                return myCall(p, c.stringArg[0], c.stringArg[1]);
            }
            else if (c.HasStringFloatArg)
            {
                Func<Pawn, List<string>, List<FloatRange>, bool> myCall = ConditionDictionnary.StringFloatArgconditions.TryGetValue(c.type);
                if (MethodIsNullAndDebug(c, "strind and float range arg method", myCall == null, debug))
                    return false;

                return myCall(p, c.stringArg[0], c.floatArg);

            }
            else if (c.HasIntArg)
            {
                Func<Pawn, List<IntRange>, bool> myCall = ConditionDictionnary.IntRangeListArgconditions.TryGetValue(c.type);
                if (MethodIsNullAndDebug(c, "int range arg method", myCall == null, debug))
                    return false;
 
                return myCall(p, c.intArg);
            }
            else if (c.HasFloatArg)
            {
                Func<FloatRange, bool> myCall = ConditionDictionnary.FloatRangeArgconditions.TryGetValue(c.type);
                if (MethodIsNullAndDebug(c, "float range arg method", myCall == null, debug))
                    return false;
                
                return myCall(c.floatArg[0]);
            }else if (c.HasCurve)
            {
                Func<Pawn, SimpleCurve, bool> myCall = ConditionDictionnary.CurveArgconditions.TryGetValue(c.type);

                if (MethodIsNullAndDebug(c, "curve arg method", myCall == null, debug))
                    return false;

                return myCall(p, c.curve);
            }

            if (debug)
                Log.Warning("MainCheck - This should never be reached");

            return false;
        }

        public static void ParametersDump(Condition c)
        {
            string str = "MainCheck - " + c.Description;
            if (c.HasNoArg)
            {

            }
            else
            {
                if (c.Has2StringArg || c.Has1StringArg)
                {
                    str += ". string Parameters:";
                    foreach (List<string> sl in c.stringArg)
                    {

                        foreach (string s in sl)
                            str += s + "; ";
                    }
                }
                if (c.HasIntArg)
                {
                    str += ". int range Parameters:";
                    foreach (IntRange i in c.intArg)
                    {
                        str += i.ToString() + "; ";
                    }
                }
                if (c.HasFloatArg)
                {
                    str += ". float range Parameters:";
                    foreach (FloatRange f in c.floatArg)
                    {
                        str += f.ToString() + "; ";
                    }
                }
                if (c.HasCurve)
                {
                    str += ".curve param:";
                    foreach(var cp in c.curve.Points)
                    {
                        str += cp.ToString() + "; ";
                    }
                }
            }
            

            Log.Warning(str);
        }

        public static void FoundMethodDebug(Condition c, string MethodType)
        {
            Log.Warning("could not find " + MethodType + " function for " + c.type.ToString() + "(" + c.Description + ")");
        }
        public static bool MethodIsNullAndDebug(Condition c, string debugStr, bool MethodIsNull, bool debug)
        {
            if (MethodIsNull)
            {
                if (debug)
                    FoundMethodDebug(c, debugStr);
                return true;
            }
            return false;
        }

    }
}
