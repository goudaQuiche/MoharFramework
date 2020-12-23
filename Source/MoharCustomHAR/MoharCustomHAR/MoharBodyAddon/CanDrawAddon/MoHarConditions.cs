using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharCustomHAR
{
    public static class MoharConditions
    {
        public static bool BodyPartChildrenCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            if (bodyAddon.drawIfDirectMissingChild)
                return true;

            return !pawn.RaceProps.body.AllParts.Any(
                bpr =>
                    bpr.IsSearchedBodyPart(bodyAddon.bodyPart) &&
                    pawn.HasDirectMissingChildrenAndIsNotBionic(bpr)
                );
        }

        public static bool DeadCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            return bodyAddon.drawIfDead ? true : !pawn.Dead;
        }

        public static bool DraftedCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            if (bodyAddon.drawIfDrafted && !pawn.Drafted)
                return false;

            if (bodyAddon.drawIfUndrafted && pawn.Drafted)
                return false;

            return true;
        }

        public static bool JobCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            if (!bodyAddon.HasJobParams)
                return true;

            JobParameters JP = bodyAddon.jobParams;

            if (pawn.CurJob == null)
                return JP.drawIfNullJob;

            if (JP.HasHideJobs)
            {
                if (JP.hideIfJob.Any(
                    j =>
                    j.job == pawn.CurJob.def
                    && (j.HasPostureOrMoving ? j.postureOrMoving.IsOkWithPostureAndMoving(pawn) : true)))
                    return false;
            }

            if (JP.HasDrawJobs)
            {
                if (JP.drawIfJob.Any(
                    j =>
                    j.job == pawn.CurJob.def
                    && (j.HasPostureOrMoving ? j.postureOrMoving.IsOkWithPostureAndMoving(pawn) : true)))
                    return true;
            }

            return true;
        }

        public static bool HasDirectMissingChildrenAndIsNotBionic(this Pawn pawn, BodyPartRecord ParentBPR)
        {
            if (pawn.health.hediffSet.HasDirectlyAddedPartFor(ParentBPR))
                return false;

            return pawn.RaceProps.body.AllParts.Any(
                bpr =>
                pawn.health.hediffSet.PartIsMissing(bpr) &&
                ParentBPR.parts != null && ParentBPR.parts.Contains(bpr)
            );
        }
        public static bool DebugHasDirectMissingChildrenAndIsNotBionic(this Pawn pawn, BodyPartRecord ParentBPR)
        {
            string debugStr = pawn.LabelShort + " HDMCAINB - " + ParentBPR.customLabel + " - ";

            if (pawn.health.hediffSet.HasDirectlyAddedPartFor(ParentBPR))
            {
                Log.Warning(debugStr + "is bionic, exiting with false");
                return false;
            }
            else
            {
                Log.Warning(debugStr + "is not bionic, going on");
                Log.Warning(debugStr +
                    "POAAHDAP:" + pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(ParentBPR) +
                    "HDAPF:" + pawn.health.hediffSet.HasDirectlyAddedPartFor(ParentBPR)
                );
                if (ParentBPR.parent != null)
                    Log.Warning(debugStr +
                        "parent POAAHDAP:" + pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(ParentBPR.parent) +
                        "parent HDAPF:" + pawn.health.hediffSet.HasDirectlyAddedPartFor(ParentBPR.parent)
                    );
                foreach (Hediff h in pawn.health.hediffSet.hediffs.Where(hp => hp.Part == ParentBPR))
                {
                    Log.Warning(debugStr + $" hediff: {h.def.defName}");
                }
            }

            //List <Hediff_MissingPart> mpca = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            bool answer = pawn.RaceProps.body.AllParts.Any(
                bpr =>
                pawn.health.hediffSet.PartIsMissing(bpr) &&
                ParentBPR.parts != null && ParentBPR.parts.Contains(bpr)
            );

            Log.Warning(debugStr + "found missing part root:" + answer);

            IEnumerable<BodyPartRecord> bprList = ParentBPR.GetDirectChildParts().Where(
                bpr =>
                pawn.health.hediffSet.PartIsMissing(bpr) &&
                ParentBPR.parts != null && ParentBPR.parts.Contains(bpr)
            );
            /*
                = pawn.RaceProps.body.AllParts.Where(
                bpr =>
                pawn.health.hediffSet.PartIsMissing(bpr) &&
                bpr.parent != null && !pawn.health.hediffSet.PartIsMissing(bpr.parent)
                && bpr.
            );
            */

            if (!bprList.EnumerableNullOrEmpty())
                foreach (BodyPartRecord bpr in bprList)
                {
                    Log.Warning("bpr:" + bpr.customLabel + " is missing child of " + ParentBPR.customLabel);
                }


            Log.Warning(debugStr + " => final answer: " + answer);

            return answer;
        }

    }
}
