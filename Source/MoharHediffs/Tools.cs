using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class Tools
    {
        public static void DestroyParentHediff(Hediff parentHediff, bool debug=false)
        {
            if (parentHediff.pawn != null && parentHediff.def.defName != null)
                Warn(parentHediff.pawn.Label + "'s Hediff: " + parentHediff.def.defName + " says goodbye.", debug);

            parentHediff.Severity = 0;
        }

        public static float GetPawnAgeOverlifeExpectancyRatio(Pawn pawn, bool debug = false)
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
        public static float GetPawnAdultRatio(Pawn pawn, bool debug = false)
        {
            float ratio = 1f;

            if (!OkPawn(pawn))
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
                Warn("pawn is null - wounded ", debug);
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

            Warn(pawn.Label + " is wounded ", debug && (num>0));
            return (num > 0);
        }
        public static bool IsHungry(this Pawn pawn, bool debug = false)
        {
            if (pawn == null)
            {
                Warn("pawn is null - IsHungry ", debug);
                return false;
            }

            bool answer = pawn.needs.food != null && pawn.needs.food.CurCategory == HungerCategory.Starving;
            Warn(pawn.Label + " is hungry ", debug && answer);

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
                Warn(pawn.Label + "GetBPRecord - did not find " + bodyPartDef.defName + " without " + hd.defName, myDebug);
            }
            else
                Warn(pawn.Label + "GetBPRecord - did find " + bodyPartDef.defName, myDebug);

            return bodyPart;
        }

        public static bool Negligeable(this Pawn p)
        {
            return (p == null || !p.Spawned || p.Map == null || p.Position == null);
        }

        public static bool OkPawn(Pawn pawn)
        {
            return ((pawn != null) && (pawn.Map != null));
        }
        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

        public static AlienPartGenerator.AlienComp GetAlien(Pawn pawn = null)
        {
            AlienPartGenerator.AlienComp alienComp = null;
            alienComp = pawn?.TryGetComp<AlienPartGenerator.AlienComp>();

            return alienComp;
        }
    }
}
