using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class RequirementUtils
    {
        public static bool FulfilsSeverityRequirement(this HediffComp_RandySpawnUponDeath comp)
        {
            string debugStr = comp.MyDebug ? comp.Pawn.LabelShort + " FulfilsSeverityRequirement - " : "";
            if(comp.MyDebug)Log.Warning(debugStr + "Entering");

            if (comp.Pawn == null || !comp.HasHediffRequirement)
            {
                if(comp.MyDebug)Log.Warning(debugStr + " null pawn or no requirement");
                return false;
            }

            bool Answer = true;

            foreach(HediffRequirementSettings HRS in comp.Props.requirements.hediff)
            {
                if (HRS.hediffDef == null)
                    continue;

                IEnumerable<Hediff> parentHediffs = comp.Pawn.health.hediffSet.hediffs.Where(
                    h => h.def == HRS.hediffDef &&
                    h.Severity >= HRS.severity.min &&
                    h.Severity <= HRS.severity.max
                );

                bool FoundHediff = !parentHediffs.EnumerableNullOrEmpty();

                Answer &= FoundHediff;

                if (!FoundHediff)
                {
                    if(comp.MyDebug)Log.Warning(debugStr + " did not find "+ HRS.hediffDef);
                    return false;
                }
                    
            }

            return Answer;
        }

        public static bool FulfilsThingRequirement(this HediffComp_RandySpawnUponDeath comp, Corpse corpse, out Thing closestThing)
        {
            string debugStr = comp.MyDebug ? comp.Pawn.LabelShort + " FulfilsThingRequirement - " : "";
            if(comp.MyDebug)Log.Warning(debugStr + "Entering");

            closestThing = null;

            if (corpse.Negligible() || !comp.HasThingRequirement)
            {
                if(comp.MyDebug)Log.Warning(debugStr + " negligeable corpse or no requirement");
                return false;
            }

            bool Answer = true;

            foreach (ThingRequirementSettings TRS in comp.Props.requirements.thing)
            {
                if (TRS.thingDef == null)
                    continue;

                CompRefuelable fuelComp = null;
                CompPowerTrader powerComp = null;

                IEnumerable<Thing> thingsOnMap = Find.CurrentMap.spawnedThings.Where(
                    t => t.def == TRS.thingDef &&
                    t.Position.DistanceTo(corpse.Position) <= TRS.distance.max &&
                    t.Position.DistanceTo(corpse.Position) >= TRS.distance.min &&
                    (TRS.sameFaction ? corpse.InnerPawn.Faction == t.Faction : true) &&
                    (TRS.needsFueled ?  ((fuelComp = t.TryGetComp< CompRefuelable >())!=null) && fuelComp.HasFuel : true) &&
                    (TRS.needsPowered ? ((powerComp = t.TryGetComp<CompPowerTrader>()) != null) && powerComp.PowerOn : true)
                );

                bool FoundThing = !thingsOnMap.EnumerableNullOrEmpty();

                if (FoundThing && (TRS.spawnClose || TRS.spawnInside))
                {
                    closestThing = thingsOnMap.MinBy(t => t.Position.DistanceTo(corpse.Position));
                }
                Answer &= FoundThing;

                if (!FoundThing)
                {
                    if(comp.MyDebug)Log.Warning(debugStr + " did not find " + TRS.thingDef);
                    return false;
                }
                    
            }

            return Answer;
        }

        public static bool FulfilsRequirement(this HediffComp_RandySpawnUponDeath comp, out Thing closestThing)
        {
            closestThing = null;

            if (!comp.HasRequirement)
                return true;

            if (comp.HasHediffRequirement && !comp.FulfilsSeverityRequirement())
            {
                if(comp.MyDebug)Log.Warning("hediff requirements not fulfiled");
                return false;
            }
            if (comp.HasThingRequirement && !comp.FulfilsThingRequirement(comp.Pawn.Corpse, out closestThing))
            {
                if(comp.MyDebug)Log.Warning("thing requirements not fulfiled");
                return false;
            }

            return true;
        }

    }
}