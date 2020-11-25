using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class ConditionValidation
    {
        public static bool ValidateCompatibilityOfHediffWithPawn(this PawnCondition pCon, Pawn pawn, bool debug=false)
        {
            string debugStr = debug?pawn.LabelShort + " ValidateCompatibilityOfHediffWithPawn - ":"";

            if (pCon.HasRace)
                if (!pCon.race.Contains(pawn.def))
                {
                    Tools.Warn(debugStr + " does not belong to the good race", debug);
                    return false;
                }

            if (pCon.HasGender)
                if (!pCon.gender.Contains(pawn.gender))
                {
                    Tools.Warn(debugStr + " does not belong to the good gender", debug);
                    return false;
                }
                    

            if (!pCon.ageRange.Includes(pawn.ageTracker.AgeBiologicalYears))
            {
                Tools.Warn(debugStr + " does not have the good age : " + pawn.ageTracker.AgeBiologicalYears + " => " + pCon.ageRange, debug);
                return false;
            }

            Tools.Warn(debugStr + " valid ok", debug);
            return true;
        }

        public static bool InitialHediffConditionCheck (this HediffComp_AnotherRandom comp, bool debug = false)
        {
            string debugStr = debug ? comp.Pawn.LabelShort + " InitialHediffConditionCheck - " : "";

            Tools.Warn(debugStr + " Entering", debug);

            bool anyAppliableItem = !comp.GetCompatibleItems().NullOrEmpty();

            Tools.Warn(debugStr + "found anyAppliableItem:" + anyAppliableItem, debug);

            return anyAppliableItem;
        }

        public static bool GetBPRFromHediffCondition(this BodyPartCondition bpCon, Pawn pawn, out BodyPartRecord BPR, bool debug=false)
        {
            string debugStr = debug ? pawn.LabelShort + " GetBPRFromHediffCondition - " : "";
            BPR = null;

            Tools.Warn(debugStr + " Entering", debug);

            if(bpCon == null)
            {
                Tools.Warn(debugStr + " Found no condition, returning null aka body", debug);
                return true;
            }

            // for whole Body
            if (!bpCon.HasBPCondition)
            {
                Tools.Warn(debugStr + " Found no BP condition, returning null aka body", debug);
                return true;
            }

            Tools.Warn(
                debugStr + " Found BP conditions, selecting :" + 
                " Label:"+ bpCon.HasLabel +
                "; Def:" + bpCon.HasDef +
                "; Tag:" + bpCon.HasTag
                , debug
            );

            IEnumerable<BodyPartRecord> bprList = pawn.health.hediffSet.GetNotMissingParts().Where(
                bpr =>
                (bpCon.HasLabel ? bpCon.bodyPartLabel.Any(s => s == bpr.customLabel) : true) &&
                (bpCon.HasDef ? bpCon.bodyPartDef.Any(d => d == bpr.def) : true) &&
                (bpCon.HasTag ? !bpr.def.tags.NullOrEmpty() && !bpCon.bodyPartTag.Intersect(bpr.def.tags).EnumerableNullOrEmpty() : true)
            );

            if (!bprList.EnumerableNullOrEmpty())
            {
                BPR = bprList.RandomElement();
                return true;
            }
            else
            {
                Tools.Warn(pawn.LabelShort + " does not have any compatible bodypart", debug);
            }

            return false;
        }
    }
}
