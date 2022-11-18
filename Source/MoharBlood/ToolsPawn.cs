using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
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

        public static Color GetSkinFirstColor(this Pawn pawn)
        {
            if (!(pawn.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp alienComp))
            {
                return Color.white;
            }

            return alienComp.GetChannel("skin").first;
        }

        public static Color GetSkinSecondColor(this Pawn pawn)
        {
            if (!(pawn.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp alienComp))
            {
                return Color.white;
            }

            return alienComp.GetChannel("skin").second;
        }

        public static Color GetBloodColor(this Pawn pawn)
        {
            return pawn.RaceProps.BloodDef?.graphicData.color ?? Color.white;
        }

        public static Color GetWoundColor(this Pawn pawn, BloodColor colorChoice, Color defaultColor)
        {
            switch (colorChoice)
            {
                case BloodColor.Human:
                    return MyDefs.HumanBloodColor;

                case BloodColor.Insect:
                    return MyDefs.InsectBloodColor;

                case BloodColor.SkinFirst:
                    return pawn.GetSkinFirstColor();

                case BloodColor.SkinSecond:
                    return pawn.GetSkinSecondColor();

                case BloodColor.Blood:
                    return pawn.GetBloodColor();

                case BloodColor.DefaultWoundColor:
                    return defaultColor;

                default:
                    return Color.white;
            }
        }
    }


}
