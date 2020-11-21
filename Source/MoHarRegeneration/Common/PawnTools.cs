using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoHarRegeneration
{
    public static class PawnTools
    {

        public static float GetPawnAgeOverlifeExpectancyRatio(this Pawn pawn, bool debug = false)
        {
            float ratio = 1f;

            if (pawn == null)
            {
                Tools.Warn("GetPawnAgeOverlifeExpectancyRatio pawn NOT OK", debug);
                return ratio;
            }

            ratio = (pawn.ageTracker.AgeBiologicalYearsFloat / pawn.RaceProps.lifeExpectancy);

            Tools.Warn(pawn.Label + " Age: " + pawn.ageTracker.AgeBiologicalYearsFloat + "; lifeExpectancy: " + pawn.RaceProps.lifeExpectancy + "; ratio:" + ratio, debug);
            
            return ratio;
        }
        public static float GetPawnAdultRatio(this Pawn pawn, bool debug = false)
        {
            float ratio = 1f;

            if (!pawn.NegligeablePawn())
            {
                Tools.Warn("GetPawnAgeOverlifeExpectancyRatio pawn NOT OK", debug);
                return ratio;
            }

            //Expecting adult to be the last lifestage
            ratio = ( 
                (pawn.ageTracker.AgeBiologicalYearsFloat - pawn.RaceProps.lifeStageAges.Last().minAge) /
                (pawn.RaceProps.lifeExpectancy - pawn.RaceProps.lifeStageAges.Last().minAge)
            );

            return ratio;
        }
        public static bool IsInjured(this Pawn pawn, bool debug = false)
        {
            if (pawn == null)
            {
                Tools.Warn("pawn is null - wounded ", debug);
                return false;
            }

            float num = 0f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
                {
                    num += hediffs[i].Severity;
                }
            }

            Tools.Warn(pawn.Label + " is wounded ", debug && (num>0));
            return (num > 0);
        }
        public static bool IsHungry(Pawn pawn, bool debug = false)
        {
            if (pawn == null)
            {
                Tools.Warn("pawn is null - IsHungry ", debug);
                return false;
            }

            bool answer = pawn.needs.food != null && pawn.needs.food.CurCategory == HungerCategory.Starving;
            Tools.Warn(pawn.Label + " is hungry ", debug && answer);

            return answer;
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
                Tools.Warn(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef.defName, myDebug);
                return null;
            }

            BodyPartDef BPDef = BPDefIE.RandomElement();
            pawn.RaceProps.body.GetPartsWithDef(BPDef).TryRandomElement(out BodyPartRecord bodyPart);

            Tools.Warn(pawn.Label + "GetBPRecord - DID find " + bodyPartDef.defName, myDebug);
            return bodyPart;
        }

        public static BodyPartRecord GetBPRecordWithoutHediff(this Pawn pawn, BodyPartDef bodyPartDef, HediffDef hd, bool myDebug = false)
        {
            IEnumerable<BodyPartDef> BPDefIE = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == bodyPartDef);
            if (BPDefIE.EnumerableNullOrEmpty())
            {
                Tools.Warn(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef.defName, myDebug);
                return null;
            }
            BodyPartDef BPDef = BPDefIE.RandomElement();

            List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(h => h.def == hd))
            {
                if (!bprToExclude.Contains(hediff.Part))
                    bprToExclude.Add(hediff.Part);
            }

            BodyPartRecord bodyPart = null;
            if (bprToExclude.NullOrEmpty())
            {
                pawn.RaceProps.body.GetPartsWithDef(BPDef).TryRandomElement(out bodyPart);
            }
            else
            {
                pawn.RaceProps.body.GetPartsWithDef(BPDef).Where(bp => !bprToExclude.Contains(bp)).TryRandomElement(out bodyPart);
            }

            if (bodyPart == null)
            {
                Tools.Warn(pawn.Label + "GetBPRecord - did not find " + bodyPartDef.defName + " without " + hd.defName, myDebug);
            }
            else
                Tools.Warn(pawn.Label + "GetBPRecord - did find " + bodyPartDef.defName, myDebug);

            return bodyPart;
        }

        public static bool NegligeablePawn(this Pawn pawn)
        {
            return pawn == null || pawn.Map == null || !pawn.Spawned;
        }

        public static bool HasRestNeed(this Pawn p)
        {
            return p.needs.rest != null;
        }

        public static bool HasFoodNeed(this Pawn p)
        {
            return p.needs.food != null;
        }
    }
}
