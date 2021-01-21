using Verse;
using System;
using System.Linq;
using UnityEngine;

namespace MoharAiJob
{
    public static class WorkerCheck
    {
        // By default return null
        // Browses all corpse recipes until find an ok one
        public static CorpseRecipeSettings WorkerFulfillsRequirements(this Pawn p, CorpseJobDef CJD)
        {
            if (p.NegligiblePawn() || CJD.IsEmpty)
                return null;

            if (!CJD.corpseRecipeList.Any(CRL => CRL.HasWorkerRequirement))
                return CJD.corpseRecipeList.FirstOrFallback(null);

            foreach(CorpseRecipeSettings CRS in CJD.corpseRecipeList)
            {
                if (CRS.HasWorkerRequirement)
                {
                    if (CRS.HasMinHpRequirement && !p.FulfillsHPRrequirement(CRS.workerRequirement))
                        continue;

                    if (CRS.HasHediffRequirement && !p.FulfillsHediffRequirement(CRS.workerRequirement))
                        continue;

                    if (CRS.HasFactionRequirement && !p.FulfillsFactionRequirement(CRS.workerRequirement))
                        continue;

                    return CRS;
                }
            }

            return null;
        }

        public static bool FulfillsHPRrequirement(this Pawn p, WorkerRequirement WR) => p.health.summaryHealth.SummaryHealthPercent > WR.minHealth;
        public static bool FulfillsHediffRequirement(this Pawn p, WorkerRequirement WR) =>
            p.health.hediffSet.hediffs.Any(h => WR.needsHediff.Any(hd => hd.hediff == h.def && h.Severity > hd.Severity));

        public static bool FulfillsFactionRequirement(this Pawn p, WorkerRequirement WR)
        {
            if (p.Faction == null)
                return WR.needsFaction.Any(f => f.noFaction);

            if (WR.needsFaction.Any(f => f.belongsToFaction == p.Faction.def))
                return true;

            return false;
        }
    }
}
