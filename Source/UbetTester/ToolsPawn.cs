using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class ToolsPawn
    {

        public static string PawnResumeString(this Pawn pawn)
        {
            return (pawn?.LabelShort.CapitalizeFirst() +
                    ", " +
                    (int)pawn?.ageTracker?.AgeBiologicalYears + " y/o" +
                    " " + pawn?.gender.ToString() +
                    ", " + "curLifeStage: " + pawn?.ageTracker.CurLifeStageRace.minAge + "=>" + pawn?.ageTracker.CurLifeStageRace.def.ToString()
                    );
        }
    }
}
