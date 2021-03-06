﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class BodyPartsTools
    {

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
                Tools.Warn(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef?.defName, myDebug);
                return null;
            }

            BodyPartDef BPDef = BPDefIE.RandomElement();
            pawn.RaceProps.body.GetPartsWithDef(BPDef).TryRandomElement(out BodyPartRecord bodyPart);

            Tools.Warn(pawn.Label + "GetBPRecord - DID find " + bodyPartDef?.defName, myDebug);
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
            //missingHediff = pawn.health.hediffSet.hediffs.Where(h => h.Part == BPR && pawn.health.hediffSet.PartIsMissing(BPR)).FirstOrFallback();

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
            Tools.Warn(debugStr +
                $"HasDef?{HasDef} bodyPartDef:{bodyPartDef?.defName} " +
                $"HasLabel?{HasLabel} bodyPartLabel:{bodyPartLabel} " +
                $"HasHediffDef?{HasHediffDef} Hediff:{hd?.defName} " +
                $"AllowMissing:{AllowMissing} PrioritizeMissing:{PrioritizeMissing} AllowAddedPart:{AllowAddedPart}"
                , myDebug
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
                Tools.Warn(debugStr + "found " + bprToExclude?.Count + " bpr to exclude bc they had " + hd.defName, myDebug);
            }

            BodyPartRecord bodyPart = null;
            IEnumerable<BodyPartRecord> BPRList = null;
            if (AllowMissing)
            {
                BPRList = pawn.GetAllBPR(bodyPartLabel, bodyPartDef);
                Tools.Warn(debugStr + "Allow missing - found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr", myDebug);

                if (PrioritizeMissing && !BPRList.EnumerableNullOrEmpty() && BPRList.Any(bpr => pawn.IsMissingOrHasMissingChildren(bpr)))
                {
                    BPRList = BPRList.Where(bpr => pawn.IsMissingOrHasMissingChildren(bpr));
                    Tools.Warn(debugStr + "Prioritize Missing - found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr", myDebug);
                }
            }
            else {
                BPRList = pawn.GetAllNotMissingBPR(bodyPartLabel, bodyPartDef);
                Tools.Warn(debugStr + "Not missing - found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr", myDebug);
            }

            if (BPRList.EnumerableNullOrEmpty())
                return null;

            // if there are bpr that are not added part, we remove them
            if (!AllowAddedPart) {
                Tools.Warn(debugStr + "Trying to exlude addedpart",myDebug);
                if (BPRList.Any(bpr => pawn.health.hediffSet.HasDirectlyAddedPartFor(bpr)))
                {
                    BPRList = BPRList.Where(bpr => !pawn.health.hediffSet.HasDirectlyAddedPartFor(bpr));
                    Tools.Warn(debugStr + "Added parts(bionics) forbidden- found " + (BPRList.EnumerableNullOrEmpty() ? "0" : BPRList.Count().ToString()) + " bpr", myDebug);
                } else
                    Tools.Warn(debugStr + "found no addedpart to exclude", myDebug);
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

            Tools.Warn(
                pawn.Label + "GetBPRecord - did " + (bodyPart == null ? "not" : "") +
                " find with def " + bodyPartDef?.defName + " without hediff def " + hd.defName, myDebug);

            return bodyPart;
        }
    }
}
