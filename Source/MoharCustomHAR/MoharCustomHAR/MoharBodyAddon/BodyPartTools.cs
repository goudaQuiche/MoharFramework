using Verse;
using System.Collections.Generic;
using System.Linq;


namespace MoharCustomHAR
{
    public static class BodyPartTools
    {
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

            if ( pawn.health.hediffSet.HasDirectlyAddedPartFor(ParentBPR))
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
                if(ParentBPR.parent!=null)
                Log.Warning(debugStr +
                    "parent POAAHDAP:" + pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(ParentBPR.parent) +
                    "parent HDAPF:" + pawn.health.hediffSet.HasDirectlyAddedPartFor(ParentBPR.parent)
                );
                foreach (Hediff h in pawn.health.hediffSet.hediffs.Where( hp => hp.Part == ParentBPR))
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

            if(!bprList.EnumerableNullOrEmpty())
                foreach(BodyPartRecord bpr in bprList)
                {
                    Log.Warning("bpr:" + bpr.customLabel + " is missing child of " + ParentBPR.customLabel);
                }
                

            Log.Warning(debugStr+" => final answer: " + answer);

            return answer;
        }

        public static bool IsSearchedBodyPart (this BodyPartRecord bpr, string HarBodyPart)
        {

            return bpr.untranslatedCustomLabel == HarBodyPart || bpr.def.defName == HarBodyPart;
        }
    }
}
