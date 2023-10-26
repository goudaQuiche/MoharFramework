using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class BodyPartsTools
    {

        public static BodyPartRecord GetBPRWithoutHediff(this Pawn pawn, BodyPartDef bpd, HediffDef hediffDef)
        {

            // All pawn non missing body parts defined by bpd. If empty nothing is valid
            if (!(pawn.health.hediffSet.GetNotMissingParts().Where(b => b.def == bpd) is IEnumerable<BodyPartRecord> bprL))
                return null;

            // Creating a list of bpr that have the hediff
            List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(h => h.def == hediffDef))
            {
                if (!bprToExclude.Contains(hediff.Part))
                    bprToExclude.Add(hediff.Part);
            }

            // nothing to exclude, any bpr is valid
            if (bprToExclude.NullOrEmpty())
                return bprL.RandomElementWithFallback();

            // All bpr that are in the primary list and not in exclusion list. If empty, nothing is valid
            if (!(bprL.Where(b => !bprToExclude.Contains(b)) is IEnumerable<BodyPartRecord> nonHediffBprL))
                return null;

            return nonHediffBprL.RandomElementWithFallback();
        }

        public static bool CheckIfExistingNaturalBP(this Pawn pawn, BodyPartDef bodyPartDef, bool myDebug = false)
        {
            BodyPartRecord BPR = pawn.GetBPRecord(bodyPartDef) ?? null;

            if (BPR == null || pawn.health.hediffSet.PartIsMissing(BPR) || pawn.health.hediffSet.AncestorHasDirectlyAddedParts(BPR))
                return false;

            return true;
        }

        public static BodyPartRecord GetBPRecord(this Pawn pawn, BodyPartDef bodyPartDef, bool myDebug = false)
        {
            IEnumerable<BodyPartDef> BPDefIE = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == bodyPartDef);
            if (BPDefIE.EnumerableNullOrEmpty())
            {
                if (myDebug) Log.Warning(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef?.defName);
                return null;
            }

            BodyPartDef BPDef = BPDefIE.RandomElement();
            pawn.RaceProps.body.GetPartsWithDef(BPDef).TryRandomElement(out BodyPartRecord bodyPart);

            if (myDebug) Log.Warning(pawn.Label + "GetBPRecord - DID find " + bodyPartDef?.defName);
            return bodyPart;
        }

        public static bool IsMissingBPR(this Pawn pawn, BodyPartRecord BPR, out Hediff missingHediff)
        {
            if (BPR == null)
            {
                missingHediff = null;
                return false;
            }
                
            missingHediff = pawn.health.hediffSet.hediffs.Where(h => h.def == HediffDefOf.MissingBodyPart && h.Part == BPR).FirstOrFallback();

            return missingHediff != null;
        }

        public static bool HasMissingChildren(this Pawn pawn, BodyPartRecord bpr)
        {
            List<Hediff_MissingPart> mpca = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            return mpca.Any(HMP => HMP.Part == bpr);
        }

        public static bool IsMissingOrHasMissingChildren(this Pawn pawn, BodyPartRecord bpr)
        {
            return pawn.health.hediffSet.PartIsMissing(bpr) || pawn.HasMissingChildren(bpr);
        }

        public static IEnumerable<BodyPartRecord> GetAllBPR(this Pawn pawn, string bodyPartLabel, BodyPartDef bodyPartDef)
        {
            bool HasLabel = !bodyPartLabel.NullOrEmpty();
            bool HasDef = bodyPartDef != null;

            return pawn.RaceProps.body.AllParts.Where(
                bpr =>
                (HasLabel ? bpr.customLabel == bodyPartLabel : true) &&
                (HasDef ? bpr.def == bodyPartDef : true)
            );
        }

        public static IEnumerable<BodyPartRecord> GetAllNotMissingBPR(this Pawn pawn, string bodyPartLabel, BodyPartDef bodyPartDef)
        {
            bool HasLabel = !bodyPartLabel.NullOrEmpty();
            bool HasDef = bodyPartDef != null;

            return pawn.health.hediffSet.GetNotMissingParts().Where(
                    bpr =>
                    (HasLabel ? bpr.customLabel == bodyPartLabel : true) &&
                    (HasDef ? bpr.def == bodyPartDef : true)
                );
        }

        public static BodyPartRecord GetBPRecordWithoutHediff(this Pawn pawn, string bodyPartLabel, BodyPartDef bodyPartDef, HediffDef hd, bool AllowMissing = false, bool PrioritizeMissing = false, bool AllowAddedPart = true, bool myDebug = false)
        {
            bool HasHediffDef = hd != null;
            bool HasLabel = !bodyPartLabel.NullOrEmpty();
            bool HasDef = bodyPartDef != null;

            string debugStr = pawn.Label + " GetBPRecordWithoutHediff - ";
            if (myDebug) Log.Warning(debugStr +
                $"HasDef?{HasDef} bodyPartDef:{bodyPartDef?.defName} " +
                $"HasLabel?{HasLabel} bodyPartLabel:{bodyPartLabel} " +
                $"HasHediffDef?{HasHediffDef} Hediff:{hd?.defName} " +
                $"AllowMissing:{AllowMissing} PrioritizeMissing:{PrioritizeMissing} AllowAddedPart:{AllowAddedPart}"
            );

            List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
            if (HasHediffDef)
            {
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(
                    h =>
                    h.def == hd
                    /*&&
                    HasLabel ? h.Part.customLabel == bodyPartLabel : true &&
                    HasDef ? h.Part.def == bodyPartDef : true*/
                    )
                )
                {
                    if (!bprToExclude.Contains(hediff.Part))
                        bprToExclude.Add(hediff.Part);
                }
                if (myDebug) Log.Warning(debugStr + "found " + bprToExclude?.Count + " bpr to exclude bc they had " + hd.defName);
            }

            BodyPartRecord bodyPart = null;
            IEnumerable<BodyPartRecord> BPRList = null;
            if (AllowMissing)
            {
                BPRList = pawn.GetAllBPR(bodyPartLabel, bodyPartDef);
                if (myDebug) Log.Warning(debugStr + "Allow missing - found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr");

                if (PrioritizeMissing && !BPRList.EnumerableNullOrEmpty() && BPRList.Any(bpr => pawn.IsMissingOrHasMissingChildren(bpr)))
                {
                    BPRList = BPRList.Where(bpr => pawn.IsMissingOrHasMissingChildren(bpr));
                    if (myDebug) Log.Warning(debugStr + "Prioritize Missing - found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr");
                }
            }
            else {
                BPRList = pawn.GetAllNotMissingBPR(bodyPartLabel, bodyPartDef);
                if (myDebug) Log.Warning(debugStr + "Not missing - found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr");
            }

            if (BPRList.EnumerableNullOrEmpty())
                return null;

            // if there are bpr that are not added part, we remove them
            if (!AllowAddedPart) {
                Tools.Warn(debugStr + "Trying to exlude addedpart", myDebug);
                if (BPRList.Any(bpr => pawn.health.hediffSet.HasDirectlyAddedPartFor(bpr)))
                {
                    BPRList = BPRList.Where(bpr => !pawn.health.hediffSet.HasDirectlyAddedPartFor(bpr));
                    if (myDebug) Log.Warning(debugStr + "Added parts(bionics) forbidden- found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr");
                } else
                    if (myDebug) Log.Warning(debugStr + "found no addedpart to exclude");
            }

            if (bprToExclude.NullOrEmpty())
            {
                BPRList.TryRandomElement(out bodyPart);
            }
            else
            {
                if (BPRList.Any(bp => !bprToExclude.Contains(bp)))
                    BPRList.Where(bp => !bprToExclude.Contains(bp)).TryRandomElement(out bodyPart);
                else bodyPart = null;
            }

            if (myDebug) Log.Warning(
                pawn.Label + "GetBPRecord - did " + (bodyPart == null ? "not" : "") +
                " find with def " + bodyPartDef?.defName + " without hediff def " + hd.defName);

            return bodyPart;
        }

        public static BodyPartRecord GetBrain(this Pawn pawn)
        {
            pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).TryRandomElement(out BodyPartRecord bodyPart);
            return bodyPart;
        }
    }
}
