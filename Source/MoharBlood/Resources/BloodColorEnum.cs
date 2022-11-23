using AlienRace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;


namespace MoharBlood
{
    public enum ColoringWay
    {
        [Description("Skin first color")]
        SkinFirst = 1,
        [Description("Skin second color")]
        SkinSecond = 2,

        [Description("Insect")]
        Insect = 3,
        [Description("Human")]
        Human = 4,

        [Description("BloodFilth")]
        BloodFilth = 5,

        [Description("defaultColor")]
        DefaultColor = 6,

        [Description("Debug")]
        Debug = 7,

       [Description("Unset")]
        Unset = 8
    }

    public static class ColoringWayUtils
    {
        // bright pink
        public static Color debugColor = Color.magenta;
        public static Color bugColor = Color.white;


        // Alien 1st skin color
        public static Color GetSkinFirstColor(this Pawn pawn)
        {
            if (!(pawn.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp alienComp))
            {
                return bugColor;
            }

            return alienComp.GetChannel("skin").first;
        }

        // Alien 2nd skin color
        public static Color GetSkinSecondColor(this Pawn pawn)
        {
            if (!(pawn.TryGetComp<AlienPartGenerator.AlienComp>() is AlienPartGenerator.AlienComp alienComp))
            {
                return bugColor;
            }

            return alienComp.GetChannel("skin").second;
        }

        // ThingDef_AlienRace/race/bloodDef - filth
        public static Color GetBloodFilthColor(this Pawn pawn)
        {
            return pawn.RaceProps.BloodDef?.graphicData.color ?? bugColor;
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
                    return debugColor;

                case ColoringWay.Unset:
                    return bugColor;

                default:
                    return bugColor;
            }
        }
    }
}
