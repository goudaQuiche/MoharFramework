using RimWorld;

using System.Linq;
using Verse;


namespace MoharHediffs
{
    public class Tools
    {
        public static void DestroyParentHediff(Hediff parentHediff, bool debug=false)
        {
            if (OkPawn(parentHediff.pawn) && parentHediff.def.defName != null)
                Tools.Warn(parentHediff.pawn.Name.ToStringShort + "'s Hediff called " + parentHediff.def.defName + " says goodbye.", debug);
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

            Tools.Warn(pawn.Name.ToStringShort + " Age: " + pawn.ageTracker.AgeBiologicalYearsFloat + "; lifeExpectancy: " + pawn.RaceProps.lifeExpectancy + "; ratio:" + ratio, debug);
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

        public static bool OkPawn(Pawn pawn)
        {
            return ((pawn != null) && (pawn.Map != null));
        }

        public static string OkStr(bool boolean=false)
        {
            return "[" + ((boolean) ? ("OK") : ("KO")) + "]";
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }
        public static void WarnRare(string warning, int period=300, bool debug = false)
        {
            if (debug)
            {
                bool display = ((Find.TickManager.TicksGame % period)==0);
                if (display)
                    Log.Warning(warning);
            }
        }
        
        public static string PawnResumeString(Pawn pawn)
        {
            return (pawn?.LabelShort.CapitalizeFirst() +
                    ", " +
                    (int)pawn?.ageTracker?.AgeBiologicalYears + " y/o" +
                    //" " + pawn?.gender.ToString()?.Translate()?.ToLower() +
                    " " + pawn?.gender.GetLabel() + 
                    ", " + pawn?.def?.label + "("+pawn.kindDef+")"
                    );
        }

        //debug Toggle kinda pointless
        public static string DebugStatus(bool debug)
        {
            return (debug+ "->" + !debug);
        }

        //PauseOnError for debug purpose
        public static void PauseOnErrorToggle()
        {
            Prefs.PauseOnError = !Prefs.PauseOnError;
        }

    }
}
