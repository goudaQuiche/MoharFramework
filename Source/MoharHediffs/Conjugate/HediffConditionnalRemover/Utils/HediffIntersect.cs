using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class HediffIntersect
    {
        public static bool RemoveHediffAndReturnTrue(Pawn p, Hediff h, bool debug = false)
        {
            string debugStr = debug ? $"{p.LabelShort} - {p.def.defName} - RemoveHediff {h.def.defName}" : "";
            Tools.Warn(debugStr, debug);

            p.health.RemoveHediff(h);
            
            return true;
        }

        public static bool TreatLightCondition(this Pawn p, LightCondition LC, Hediff h, float lightLevel, bool outside, bool debug=false)
        {
            if (
                (LC.RequiresLightLevel && !LC.level.Value.Includes(lightLevel)) ||
                (LC.requiresOutside && !outside) ||
                (LC.requiresInside && outside)
            )
            {
                return RemoveHediffAndReturnTrue(p, h, debug);
            }
            return false;
        }

        public static bool TreatNeedCondition(this Pawn p, List<NeedCondition> needs, Hediff h, bool debug = false)
        {
            string debugStr = debug ? $"{p.LabelShort} TreatNeedCondition - " : "";

            foreach (NeedCondition NC in needs)
            {
                Tools.Warn(debugStr + $"checking {NC.needDef.defName} => {NC.level.min} > x > {NC.level.max}", debug);
                Need need = p.needs.AllNeeds.Where(
                    n => 
                    n.def == NC.needDef && 
                    !NC.level.Includes(n.CurLevelPercentage)
                ).FirstOrFallback();
                
                if (need == null) continue;

                Tools.Warn(debugStr + $"Found {need.def.defName} out of range: {need.CurLevelPercentage}", debug);

                return RemoveHediffAndReturnTrue(p, h, debug);
            }
            return false;
        }

        public static bool TreatHediffSeverityCondition(this Pawn p, List<HediffSeverityCondition> destroyingHediffs, Hediff h, bool debug = false)
        {
            foreach (HediffSeverityCondition HSC in destroyingHediffs)
            {
                Hediff hediff = p.health.hediffSet.hediffs.Where(dh => dh.def == HSC.hediffDef && !HSC.acceptableSeverity.Includes(dh.Severity)).FirstOrFallback();
                if (hediff == null) continue;

                return RemoveHediffAndReturnTrue(p, hediff, debug);
            }
            return false;
        }

        public static bool TreatRelevantHediffsAndReportIfStillHediffsToCheck(this HediffComp_OnTheCarpet comp)
        {
            bool Mydebug = comp.MyDebug;

            bool AtLeastOneIntersectHediffLeft = false;
            bool AtLeastOneNonRemovedHediffLeft = false;

            Pawn pawn = comp.Pawn;
            string debugStr = Mydebug ? $"{pawn.LabelShort} TreatRelevant - " : "";

            Tools.Warn(debugStr + " Entering", Mydebug);

            float temperature = pawn.AmbientTemperature;
            float lightLevel = pawn.Map.glowGrid.GameGlowAt(pawn.Position);
            Room room = pawn.GetRoom();
            bool outside = (room == null) ? true : room.PsychologicallyOutdoors;

            List<Hediff> pHediffs = comp.Pawn.health.hediffSet.hediffs;
            //foreach (Hediff H in comp.Pawn.health.hediffSet.hediffs)
            for (int Hi = pHediffs.Count - 1; Hi >= 0 && !pHediffs.NullOrEmpty(); Hi--)
            {
                Hediff H = pHediffs[Hi];
                foreach (HediffItemToRemove HITR in comp.Props.hediffPool.Where(h => h.hediffDef == H.def))
                {
                    Tools.Warn(debugStr + " found intersect hediff: " + H.def.defName, Mydebug);

                    AtLeastOneNonRemovedHediffLeft = true;

                    HediffKeepingCondition HKC = HediffRemovalConditionBuilder.GetDefaultPlusSpecificHediffCondition(comp.Props.defaultCondition, HITR.specificCondition, Mydebug);
                    bool RemovedSingleHediff = false;
                    // Light : glow outside inside
                    if (HKC.HasLightCondition)
                    {
                        Tools.Warn(debugStr + H.def.defName + "checking light", Mydebug);
                        RemovedSingleHediff = pawn.TreatLightCondition(HKC.light, H, lightLevel, outside, Mydebug);
                    }
                    // Temperature
                    else if (HKC.HasTemperatureCondition && !HKC.temperature.Value.Includes(temperature))
                    {
                        Tools.Warn(debugStr + H.def.defName + "checking temperature", Mydebug);
                        RemovedSingleHediff = RemoveHediffAndReturnTrue(pawn, H, Mydebug);
                    }
                    // Needs
                    else if (HKC.HasNeedCondition)
                    {
                        Tools.Warn(debugStr + H.def.defName + "checking " + HKC.needs.Count + "need", Mydebug);
                        RemovedSingleHediff = pawn.TreatNeedCondition(HKC.needs, H, Mydebug);
                    }
                    // Hediffs
                    else if (HKC.HasDestroyingHediffs)
                    {
                        Tools.Warn(debugStr + H.def.defName + "checking other hediffs", Mydebug);
                        RemovedSingleHediff = pawn.TreatHediffSeverityCondition(HKC.destroyingHediffs, H, Mydebug);
                    }

                    AtLeastOneIntersectHediffLeft |= AtLeastOneNonRemovedHediffLeft = !RemovedSingleHediff;
                    if (RemovedSingleHediff)
                        return true;
                }
            }


            Tools.Warn(debugStr + "exiting", Mydebug);
            return AtLeastOneIntersectHediffLeft;
        }
    }
}
