using Verse;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MoharAiJob
{
    public static class WorkerCheck
    {
        // By default return null
        // Browses all corpse recipes until find an ok one
        public static IEnumerable<CorpseRecipeSettings> WorkerFulfillsRequirements(this Pawn p, CorpseJobDef CJD, bool debug = false)
        {
            string DebugStr = debug ? "WorkerFulfillsRequirements - " : string.Empty;

            //if (p.NegligiblePawn() || CJD.IsEmpty)
            //if (p.NegligiblePawnDebug(debug) || CJD.IsEmpty)
            if (CJD.IsEmpty)
            {
                //if (debug) Log.Warning("negligible pawn or empy CJD");
                if (debug) Log.Warning("empy CJD");
                yield break;
            }
            if (debug) DebugStr = p.ThingID + DebugStr;

            foreach (CorpseRecipeSettings CRS in CJD.corpseRecipeList)
            {
                if (!CRS.HasWorkerSpec)
                {
                    if (debug) Log.Warning(DebugStr + " no workrequirement, yield");
                    yield return CRS;
                }
                else
                {
                    WorkerRequirement WR = CRS.worker;

                    if (WR.HasRelevantMinHp && !p.FulfilsHPRrequirement(WR))
                    {
                        if (debug) Log.Warning(DebugStr + " HP requirement ko, continue");
                        continue;
                    }

                    if (WR.HasHediffRequirement && !p.FulfilsHediffRequirement(WR))
                    {
                        if (debug) Log.Warning(DebugStr + " Hediff requirement ko, continue");
                        continue;
                    }

                    if (WR.HasFactionRequirement && !p.FulfilsFactionRequirement(WR))
                    {
                        if (debug) Log.Warning(DebugStr + " Faction requirement ko, continue");
                        continue;
                    }

                    if (WR.HasLifeStageRequirement && !p.FulfilsLifeStageRequirement(WR))
                    {
                        if (debug) Log.Warning(DebugStr + " lifestage requirement ko, continue");
                        continue;
                    }

                    if (WR.HasRelevantChancesToWorkDivider && !p.FulfilsChancesToWorkDivider(WR))
                    {
                        if (debug) Log.Warning(DebugStr + " had not luck, continue");
                        continue;
                    }
                    yield return CRS;

                }
            }

            yield break;
        }

        public static GraveDig_JobParameters WorkerFulfillsRequirements(this Pawn p, GraveDiggerDef GDD)
        {
            //if (p.NegligiblePawn() || GDD.IsEmpty)
            if (GDD.IsEmpty)
                return null;

            if (!GDD.jobParameters.Any(jp => jp.HasWorkerRequirement))
                return GDD.jobParameters.FirstOrFallback(null);

            foreach (GraveDig_JobParameters GDJP in GDD.jobParameters)
            {
                if (GDJP.HasWorkerRequirement)
                {
                    WorkerRequirement WR = GDJP.workerRequirement;

                    if (WR.HasRelevantMinHp && !p.FulfilsHPRrequirement(WR))
                        continue;

                    if (WR.HasHediffRequirement && !p.FulfilsHediffRequirement(WR))
                        continue;

                    if (WR.HasFactionRequirement && !p.FulfilsFactionRequirement(WR))
                        continue;

                    if (WR.HasLifeStageRequirement && !p.FulfilsLifeStageRequirement(WR))
                        continue;

                    return GDJP;
                }
            }

            return null;
        }

        public static bool FulfilsLifeStageRequirement(this Pawn p, WorkerRequirement WR) =>
            WR.lifeStageRequirement.Contains(p.ageTracker.CurLifeStage);

        public static bool FulfilsHPRrequirement(this Pawn p, WorkerRequirement WR) => 
            p.health.summaryHealth.SummaryHealthPercent > WR.minHealthPerc;

        public static bool FulfilsHediffRequirement(this Pawn p, WorkerRequirement WR) =>
            p.health.hediffSet.hediffs.Any(h => WR.hediffRequirement.Any(h1 => h1.hediff == h.def && h.Severity > h1.severity));

        public static bool FulfilsFactionRequirement(this Pawn p, WorkerRequirement WR)
        {
            if (p.Faction == null)
                return WR.factionRequirement.Any(f => f.noFaction);

            if (WR.factionRequirement.Any(f => f.belongsToFaction == p.Faction.def))
                return true;

            return false;
        }

        public static bool FulfilsChancesToWorkDivider(this Pawn p, WorkerRequirement WR) =>
            (Find.TickManager.TicksGame + p.thingIDNumber ) % WR.chancesToWorkDivider == 0;
    }
}
