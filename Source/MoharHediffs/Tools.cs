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

        public static void DestroyHediff(this Pawn pawn, Hediff hediff, bool debug = false)
        {
            if (hediff.pawn != null && hediff.def.defName != null)
                Warn(hediff.pawn.Label + "'s Hediff: " + hediff.def.defName + " says goodbye.", debug);

            pawn.health.RemoveHediff(hediff);
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

        public static bool Negligible(this Pawn p)
        {
            return (p == null || !p.Spawned || p.Map == null || p.Position == null);
        }

        public static bool Negligible(this Thing thing)
        {
            return (thing == null || !thing.Spawned || thing.Map == null || thing.Position == null);
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
        public static bool IsAlien(this Pawn pawn)
        {
            return pawn.def.race.Humanlike && pawn.def != ThingDefOf.Human;
        }
        public static bool IsHuman(this Pawn pawn)
        {
            return pawn.def.race.Humanlike && pawn.def == ThingDefOf.Human;
        }

        public static AlienPartGenerator.AlienComp GetAlien(Pawn pawn = null)
        {
            AlienPartGenerator.AlienComp alienComp = null;
            alienComp = pawn?.TryGetComp<AlienPartGenerator.AlienComp>();

            return alienComp;
        }
    }
}
