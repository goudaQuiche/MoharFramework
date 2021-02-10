using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MoharThoughts
{
    public static class BodyPartUtils
    {
        public static bool ValidateBP(this MTWDef mtwDef, BodyPartRecord BPR)
        {
            if (mtwDef.HasBodyPartDefTarget)
                return mtwDef.bodyPart == BPR.def;
            else if (mtwDef.HasBodyPartLabelTarget)
                return mtwDef.bodyPartLabel == BPR.customLabel;

            return false;
        }

        public static bool BodyPartHasHediff(this Pawn p, BodyPartRecord bpr, List<HediffDef> HDL)
        {
            return p.health.hediffSet.hediffs.Any(
                h => 
                h.Part == bpr &&
                HDL.Contains(h.def)
            );
        }

        public static bool PartOrAnyAncestorHasHediff(this Pawn p, BodyPartRecord bpr, List<HediffDef> HDL)
        {
            if (p.BodyPartHasHediff(bpr, HDL))
                return true;

            if (bpr.parent != null && p.PartOrAnyAncestorHasHediff(bpr.parent, HDL))
                return true;

            return false;
        }

        public static bool IsAccessListCompatible(this Pawn p, BodyPartRecord bpr, MTWDef mTWDef, bool debug=false)
        {
            if (mTWDef.HasApplyList && p.PartOrAnyAncestorHasHediff(bpr, mTWDef.applyThoughtHediffList))
            {
                if(debug)Log.Warning(p.LabelShort + " " + bpr.Label + " is in apply list");
                return true;
            }
                

            if (mTWDef.HasIgnoreList && p.PartOrAnyAncestorHasHediff(bpr, mTWDef.ignoreThoughtHediffList))
            {
                if (debug) Log.Warning(p.LabelShort + " " + bpr.Label + " is in ignore list");
                return false;
            }

            if (debug) Log.Warning(p.LabelShort + " " + bpr.Label + " is ACL compatible");
            return true;
        }

    }
}
