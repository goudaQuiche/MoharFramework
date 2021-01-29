using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class KeepingConditionCompatibility
    {
        public static bool IsPawnNeedConditionCompatible(this Pawn p, HediffKeepingCondition HKC, bool debug=false)
        {
            string debugStr = debug ? $"{p.Label} IsPawnNeedConditionCompatible - " : "";

            if (HKC.HasNeedCondition)
            {
                foreach (NeedCondition nc in HKC.needs)
                {
                    bool doesContain = false;
                    foreach (Need n in p.needs.AllNeeds)
                    {
                        Tools.Warn(debugStr + $"{nc.needDef.defName} found in pawn needs, ok", debug);
                        doesContain |= n.def == nc.needDef;
                    }
                        

                    if (!doesContain)
                    {
                        Tools.Warn(debugStr + $"{nc.needDef.defName} not found in pawn needs, exiting", debug);
                        return false;
                    }
                }
            }

            Tools.Warn(debugStr + "is need compatible, ok", debug);

            return true;
        }

        /*
        public static bool HasPawnDestroyingHediff(this HediffCompProperties_OnTheCarpet Props, Pawn p)
        {
            bool MyDebug = Props.debug;

            string debugStr = MyDebug ? $"{p.Label} HasPawnDestroyingHediff - " : "";
            bool AtLeastOneHediff = false;
            if (Props.has)
            {

            }
            return AtLeastOneHediff;
        }
        */
        public static bool IsPawnNeedCompatible(this HediffCompProperties_OnTheCarpet Props, Pawn p)
        {
            bool MyDebug = Props.debug;

            string debugStr = MyDebug ? $"{p.Label} IsPawnNeedCompatible - " : "";

            if (Props.HasDefaultCondition)
            {
                Tools.Warn(debugStr + "checking default condition", MyDebug);
                if (!p.IsPawnNeedConditionCompatible(Props.defaultCondition, MyDebug))
                {
                    Tools.Warn(debugStr + "defaultCondition not compatible with pawn, exiting", MyDebug);
                    return false;
                }
                else Tools.Warn(debugStr + " Compatible with defaultCondition", MyDebug);

            }
            foreach(HediffItemToRemove HITR in Props.hediffPool)
            {
                if (HITR.HasSpecificCondition)
                {
                    Tools.Warn(debugStr + $"checking {HITR.hediffDef.defName} specific condition", MyDebug);
                    if (!p.IsPawnNeedConditionCompatible(HITR.specificCondition, MyDebug))
                    {
                        Tools.Warn(debugStr + "specificCondition not compatible with pawn, exiting", MyDebug);
                        return false;
                    }else Tools.Warn(debugStr + " Compatible with specificCondition", MyDebug);
                }
            }
            return true;
        }
    }
}
