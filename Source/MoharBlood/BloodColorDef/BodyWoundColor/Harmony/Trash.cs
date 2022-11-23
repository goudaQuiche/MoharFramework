using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoharBlood
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

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].description;
            else return source.ToString();
        }
    }

    
}
