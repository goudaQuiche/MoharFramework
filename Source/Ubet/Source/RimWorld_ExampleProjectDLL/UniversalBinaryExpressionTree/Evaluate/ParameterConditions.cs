using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class ParameterConditions
    {

        //
        public static bool PawnBelongsToLifeStage(this Pawn p, List<string> parameters)
        {
            foreach(string s in parameters)
            {
                LifeStageDef LSD = DefDatabase<LifeStageDef>.GetNamed(s);
                if (p.ageTracker.CurLifeStage == LSD)
                    return true;
            }
            return false;
        }

        public static bool PawnIsPawnKind(this Pawn p, List<string> parameters)
        {
            foreach (string s in parameters)
            {
                PawnKindDef PKD = DefDatabase<PawnKindDef>.GetNamed(s);
                if (p.kindDef == PKD)
                    return true;
            }
            return false;
        }

        public static bool PawnHasTrait(this Pawn p, List<string> parameters)
        {
            foreach (string s in parameters)
            {
                TraitDef TD = DefDatabase<TraitDef>.GetNamed(s);
                if (p.story.traits.allTraits.Any(t => t.def == TD))
                    return true;
            }
            return false;
        }

        public static bool PawnIsPerformingJob(this Pawn p, List<string> parameters)
        {
            if (p.CurJob == null)
            {
                if (parameters.NullOrEmpty())
                    return true;

                return false;
            }
                

            foreach (string s in parameters)
            {
                JobDef JD = DefDatabase<JobDef>.GetNamed(s);
                if (p.CurJobDef == JD)
                    return true;
            }
            return false;
        }

        public static bool PawnMapWeather(this Pawn p, List<string> parameters)
        {
            if (p.Map == null)
                return false;

            foreach (string s in parameters)
            {
                WeatherDef WD = DefDatabase<WeatherDef>.GetNamed(s);
                if (p.Map.weatherManager.curWeather == WD)
                    return true;
            }
            return false;
        }

        public static bool PawnMapSeason(this Pawn p, List<string> parameters)
        {
            if (p.Map == null)
                return false;
            Season mapSeason = GenLocalDate.Season(p.Map);

            foreach (string s in parameters)
            {
                switch (s)
                {
                    case "Spring":
                        if (mapSeason == Season.Spring)
                            return true;
                        continue;

                    case "Summer":
                        if (mapSeason == Season.Summer)
                            return true;
                        continue;

                    case "Fall":
                        if (mapSeason == Season.Fall)
                            return true;
                        continue;

                    case "Winter":
                        if (mapSeason == Season.Winter)
                            return true;
                        continue;

                    case "PermanentSummer":
                        if (mapSeason == Season.PermanentSummer)
                            return true;
                        continue;

                    case "PermanentWinter":
                        if (mapSeason == Season.PermanentWinter)
                            return true;
                        continue;

                    default: continue;
                }

            }
            return false;
        }
    }
}
