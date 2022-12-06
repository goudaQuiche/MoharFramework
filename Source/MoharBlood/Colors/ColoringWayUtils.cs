using AlienRace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;


namespace MoharBlood
{
    public static class ColoringWayUtils
    {
        // Alien 1st skin color
        public static Color GetSkinFirstColor(this Pawn pawn)
        {
            if (!(pawn.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp alienComp))
            {
                return MyDefs.BugColor;
            }

            return alienComp.GetChannel("skin").first;
        }

        // Alien 2nd skin color
        public static Color GetSkinSecondColor(this Pawn pawn)
        {
            if (!(pawn.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp alienComp))
            {
                return MyDefs.BugColor;
            }

            return alienComp.GetChannel("skin").second;
        }

        // ThingDef_AlienRace/race/bloodDef - filth
        public static Color GetBloodFilthColor(this Pawn pawn)
        {
            return pawn.RaceProps.BloodDef?.graphicData?.color ?? MyDefs.BugColor;
        }

        // 
        public static Color GetPawnBloodColor(this Pawn pawn, ColoringWay cw)
        {
            switch (cw)
            {
                case ColoringWay.Human:
                    return MyDefs.HumanBloodColor;

                case ColoringWay.Insect:
                    return MyDefs.InsectBloodColor;

                case ColoringWay.SkinFirst:
                    return pawn.GetSkinFirstColor();

                case ColoringWay.SkinSecond:
                    return pawn.GetSkinSecondColor();

                case ColoringWay.BloodFilth:
                    return pawn.GetBloodFilthColor();

                case ColoringWay.DefaultColor:
                    return pawn.GetDefaultColor();

                case ColoringWay.Debug:
                    return MyDefs.DebugColor;

                case ColoringWay.Unset:
                    return MyDefs.BugColor;

                default:
                    return MyDefs.BugColor;
            }
        }
    }
}
