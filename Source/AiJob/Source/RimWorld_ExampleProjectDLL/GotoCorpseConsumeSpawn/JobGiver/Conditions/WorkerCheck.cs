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
        public static IEnumerable<CorpseRecipeSettings> WorkerFulfillsRequirements(this Pawn p, CorpseJobDef CJD)
        {
            if (p.NegligiblePawn() || CJD.IsEmpty)
                yield break;

            foreach(CorpseRecipeSettings CRS in CJD.corpseRecipeList)
            {
                if (!CRS.HasWorkerRequirement)
                {
                    yield return CRS;
                }
                else
                {
                    WorkerRequirement WR = CRS.workerRequirement;

                    if (WR.HasMinHpRequirement && !p.FulfillsHPRrequirement(WR))
                        continue;

                    if (WR.HasHediffRequirement && !p.FulfillsHediffRequirement(WR))
                        continue;

                    if (WR.HasFactionRequirement && !p.FulfillsFactionRequirement(WR))
                        continue;

                    if (WR.HasLifeStageRequirement && !p.FulfillsLifeStageRequirement(WR))
                        continue;

                    yield return CRS;
                }
            }

            yield break;
        }

        public static GraveDig_JobParameters WorkerFulfillsRequirements(this Pawn p, GraveDiggerDef GDD)
        {
            if (p.NegligiblePawn() || GDD.IsEmpty)
                return null;

            if (!GDD.jobParameters.Any(jp => jp.HasWorkerRequirement))
                return GDD.jobParameters.FirstOrFallback(null);

            foreach (GraveDig_JobParameters GDJP in GDD.jobParameters)
            {
                if (GDJP.HasWorkerRequirement)
                {
                    WorkerRequirement WR = GDJP.workerRequirement;

                    if (WR.HasMinHpRequirement && !p.FulfillsHPRrequirement(WR))
                        continue;

                    if (WR.HasHediffRequirement && !p.FulfillsHediffRequirement(WR))
                        continue;

                    if (WR.HasFactionRequirement && !p.FulfillsFactionRequirement(WR))
                        continue;

                    if (WR.HasLifeStageRequirement && !p.FulfillsLifeStageRequirement(WR))
                        continue;

                    return GDJP;
                }
            }

            return null;
        }

        public static bool FulfillsLifeStageRequirement(this Pawn p, WorkerRequirement WR) =>
            WR.lifeStageRequirement.Contains(p.ageTracker.CurLifeStage);

        public static bool FulfillsHPRrequirement(this Pawn p, WorkerRequirement WR) => 
            p.health.summaryHealth.SummaryHealthPercent > WR.minHealthPerc;

        public static bool FulfillsHediffRequirement(this Pawn p, WorkerRequirement WR) =>
            p.health.hediffSet.hediffs.Any(h => WR.hediffRequirement.Any(h1 => h1.hediff == h.def && h.Severity > h1.severity));

        public static bool FulfillsFactionRequirement(this Pawn p, WorkerRequirement WR)
        {
            if (p.Faction == null)
                return WR.factionRequirement.Any(f => f.noFaction);

            if (WR.factionRequirement.Any(f => f.belongsToFaction == p.Faction.def))
                return true;

            return false;
        }
    }
}
