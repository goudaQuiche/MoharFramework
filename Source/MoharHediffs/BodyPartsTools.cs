using RimWorld;
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
            missingHediff = pawn.health.hediffSet.hediffs.Where(h => h.def == HediffDefOf.MissingBodyPart && h.Part == BPR).FirstOrFallback();
            //missingHediff = pawn.health.hediffSet.hediffs.Where(h => h.Part == BPR && pawn.health.hediffSet.PartIsMissing(BPR)).FirstOrFallback();

            return missingHediff != null;
        }

        public static bool HasMissingChildren(this Pawn pawn, BodyPartRecord BPR)
        {
            List<Hediff_MissingPart> mpca = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            return mpca.Any(HMP => HMP.Part == BPR);
        }

        public static BodyPartRecord GetBPRecordWithoutHediff(this Pawn pawn, string bodyPartLabel, HediffDef hd, bool AllowMissing = false, bool PrioritizeMissing = false, bool ForbidAddedPart=false, bool myDebug = false)
        {
            List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(h => h.def == hd))
            {
                if (!bprToExclude.Contains(hediff.Part))
                    bprToExclude.Add(hediff.Part);
            }

            BodyPartRecord bodyPart = null;
            IEnumerable<BodyPartRecord> BPRList = null;
            if (AllowMissing)
            {
                if (PrioritizeMissing)
                {
                    BPRList = pawn.RaceProps.body.AllParts.Where(
                        bpr => bpr.customLabel == bodyPartLabel && 
                        (pawn.health.hediffSet.PartIsMissing(bpr) || pawn.HasMissingChildren(bpr)) &&
                        (ForbidAddedPart? !pawn.health.hediffSet.AncestorHasDirectlyAddedParts(bpr) : true)
                    );
                }
                if (BPRList.EnumerableNullOrEmpty())
                    BPRList = pawn.RaceProps.body.AllParts.Where(
                        bpr => bpr.customLabel == bodyPartLabel &&
                        (ForbidAddedPart ? !pawn.health.hediffSet.AncestorHasDirectlyAddedParts(bpr) : true)
                    );
           }
            else
            {
                BPRList = pawn.health.hediffSet.GetNotMissingParts().Where(bpr => 
                    bpr.customLabel == bodyPartLabel &&
                    (ForbidAddedPart ? !pawn.health.hediffSet.AncestorHasDirectlyAddedParts(bpr) : true)
                );
            }

            if (bprToExclude.NullOrEmpty())
            {
                BPRList.TryRandomElement(out bodyPart);
            }
            else
            {
                BPRList.Where(bp => !bprToExclude.Contains(bp)).TryRandomElement(out bodyPart);
            }

            Tools.Warn(
                pawn.Label + "GetBPRecord - did " + (bodyPart == null ? "not" : "") +
                " find with label " + bodyPartLabel + " without hediff def " + hd.defName, myDebug);

            return bodyPart;
        }


        public static BodyPartRecord GetBPRecordWithoutHediff(this Pawn pawn, BodyPartDef bodyPartDef, HediffDef hd, bool AllowMissing = false, bool PrioritizeMissing = false, bool ForbidAddedPart = false, bool myDebug = false)
        {
            string debugStr = pawn.Label + " GetBPRecordWithoutHediff - ";
            Tools.Warn(debugStr + 
                $"bodyPartDef:{bodyPartDef?.defName} Hediff:{hd?.defName} AllowMissing:{AllowMissing} PrioritizeMissing:{PrioritizeMissing} ForbidAddedPart:{ForbidAddedPart}"
                , myDebug
            );

            if (!DefDatabase<BodyPartDef>.AllDefs.Any((BodyPartDef b) => b == bodyPartDef))
            {
                Tools.Warn(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef?.defName, myDebug);
                return null;
            }

            List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(
                h => 
                h.def == hd)
            )
            {
                if (!bprToExclude.Contains(hediff.Part))
                    bprToExclude.Add(hediff.Part);
            }

            BodyPartRecord bodyPart = null;
            IEnumerable<BodyPartRecord> BPRList = null;
            if (AllowMissing)
            {
                if (PrioritizeMissing && BPRList.Any(bpr =>pawn.health.hediffSet.PartIsMissing(bpr) || pawn.HasMissingChildren(bpr)))
                {
                    BPRList = pawn.RaceProps.body.GetPartsWithDef(bodyPartDef).Where(
                        bpr =>
                        pawn.health.hediffSet.PartIsMissing(bpr) || pawn.HasMissingChildren(bpr)
                    );
                }
                if (BPRList.EnumerableNullOrEmpty())
                    BPRList = pawn.RaceProps.body.GetPartsWithDef(bodyPartDef);
            }
            else {
                BPRList = pawn.health.hediffSet.GetNotMissingParts().Where(
                    bpr => 
                    bpr.def == bodyPartDef
                );
            }

            if (ForbidAddedPart && BPRList.Any(bpr => pawn.health.hediffSet.AncestorHasDirectlyAddedParts(bpr)));
                BPRList = BPRList.Where( bpr => (ForbidAddedPart ? !pawn.health.hediffSet.AncestorHasDirectlyAddedParts(bpr) : true));

            if (bprToExclude.NullOrEmpty())
            {
                BPRList.TryRandomElement(out bodyPart);
            }
            else
            {
                BPRList.Where(bp => !bprToExclude.Contains(bp)).TryRandomElement(out bodyPart);
            }

            Tools.Warn(
                pawn.Label + "GetBPRecord - did " + (bodyPart == null ? "not" : "") + 
                " find with def " + bodyPartDef?.defName + " without hediff def " + hd.defName, myDebug);

            return bodyPart;
        }
    }
}
