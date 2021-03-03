using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace MoharThoughts
{
    // race body part hediff thought worker
    public class RaceBPHediff : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            //Log.Warning(p.LabelShort + " Entering RaceBPHediff " + p.def);

            MTWDef myTWD =
                DefDatabase<MTWDef>.AllDefs
                .Where(
                    m =>
                     m.race == p.def &&
                    def == m.thought &&
                    m.HasTarget &&
                    (m.HasLifeStages ? m.lifeStages.Contains(p.ageTracker.CurLifeStage) : true)
                ).SingleOrDefault();

            if (myTWD == null)
                return false;

            //Log.Warning(p.LabelShort + " found TWD");

            IEnumerable<Hediff> HA = p.health.hediffSet.hediffs
                .Where(h =>
                h.Part != null &&
                myTWD.ValidateBP(h.Part) &&
                //h.def == myTWD.hediff 
                (h.def == myTWD.hediff || (myTWD.HasApplyList && myTWD.applyThoughtHediffList.Contains(h.def)))
            );

            if (HA.EnumerableNullOrEmpty())
                return false;

            if (myTWD.ignoreThoughtIfAddedPart)
            {
                HA = HA
                .Where(h =>
                    !p.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(h.Part)
                );
            }
            else if (myTWD.HasAccessList)
            {
                HA = HA
                .Where(h =>
                    p.IsAccessListCompatible(h.Part, myTWD, myTWD.debug)
                );
            }

            if (HA.EnumerableNullOrEmpty())
                return false;

            if (myTWD.HasRequiredBpNum)
                return myTWD.bpNum.Includes(HA.EnumerableCount());

            return false;
        }
    }
}
