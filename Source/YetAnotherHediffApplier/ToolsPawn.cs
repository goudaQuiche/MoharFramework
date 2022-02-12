using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace YAHA
{
    public static class ToolsPawn
    {

        public static bool HasHediff(this Pawn pawn, HediffDef hediffDef)
        {
            return pawn.health.hediffSet.HasHediff(hediffDef);
        }

        public static string PawnResumeString(this Pawn pawn)
        {
            return (pawn?.LabelShort.CapitalizeFirst() +
                    ", " +
                    (int)pawn?.ageTracker?.AgeBiologicalYears + " y/o" +
                    " " + pawn?.gender.ToString() +
                    ", " + "curLifeStage: " + pawn?.ageTracker.CurLifeStageRace.minAge + "=>"+ pawn?.ageTracker.CurLifeStageRace.def.ToString()
                    );
        }

        public static List<BodyPartRecord> GetBP(this Pawn pawn, List<string> BP, bool debug=false)
        {
            IEnumerable<BodyPartRecord> bodyPartRecords = pawn.health.hediffSet.GetNotMissingParts().Where(bpr => BP.Contains(bpr.untranslatedCustomLabel) || BP.Contains(bpr.def.defName));

            if (bodyPartRecords.EnumerableNullOrEmpty())
            {
                if (debug) Log.Warning("Cant find BPR with def/label: " + BP + ", skipping");
                return null;
            }
            //return bodyPartRecords.FirstOrFallback();
            return bodyPartRecords.ToList();
        }

        public static Season GetSeason(this Pawn pawn)
        {
            if (pawn.Map == null)
                return Season.Undefined;

            return GenLocalDate.Season(pawn.Map);
        }

        public static WeatherDef GetWeather(this Pawn pawn)
        {
            if (pawn.Map == null)
                return WeatherDefOf.Clear;

            return pawn.Map.weatherManager.curWeather;
        }
                      
        public static bool GetOutdoor(this Pawn pawn)
        {
            if (pawn.needs.mood == null)
                return false;

            return pawn.needs.mood.recentMemory.TicksSinceOutdoors == 0;
        }
    }
}
