using RimWorld;
using Verse;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;

namespace MoharBlood
{
    /*
    				<!-- blood red; 255, 87, 51 ; 1, .3 ,.2 -->
				<!-- blood red; 197, 0, 0  ; 1, .3 ,.2 -->

				<!-- AirPuff(120, 18, 0) -->
				<!-- dark brown -->
				<!-- BloodSplash 210, 50, 0 -->
				<!-- dark orange almost borwn-->
				<!-- BodyImpact 255, 120, 30, 60 -->
				<!-- orange -->
        */

    // Cached value in dictionary
    public class CachedMitigatedColor
    {
        public FleckDef fleckDef;
        public Color color;
    }

    public class RankedColor
    {
        public float ColorValue;
        public int Rank;
        public char ColorComp;

        public RankedColor(float colorValue, char colorComp, int rank)
        {
            ColorValue = colorValue;
            ColorComp = colorComp;
            Rank = rank;
        }
    }

    public static class MitigateFleckColor
    {
        /*
        public static CachedMitigatedColor GetCachedMitigatedColor(FleckMitigatedColor fmc, Pawn pawn)
        {
            CachedMitigatedColor mitigatedColor = new CachedMitigatedColor
            {
                fleckDef = fmc.fleckDef
            };

            Color baseColor = pawn.GetWoundColor(fmc.bloodColor, Color.white);



            return mitigatedColor;
        }
        */

        public static Color GetMitigatedColor(Color color, FleckMitigatedColor fmc)
        {
            if (!fmc.HasColorMitigation)
                return color;

            switch (fmc.mitigation.type)
            {
                case ColorMitigationType.AirPuff:
                    return GetAirPuffLikeColorMitigation(color);
                case ColorMitigationType.BloodSplash:
                    return GetBloodSplashLikeColorMitigation(color);
                case ColorMitigationType.BodyImpact:
                    return GetBodyImpactLikeColorMitigation(color);
                case ColorMitigationType.Custom:
                    return GetCustomColorMitigation(color, fmc.mitigation.customMitigation);

                case ColorMitigationType.NoMitigation:
                default:
                    return color;
            }
        }

        public static List<RankedColor> CreateRanks(Color color)
        {
            List<Tuple<float, char>> colors = new List<Tuple<float, char>>
                {
                    new Tuple<float, char>(color.r, 'r'),
                    new Tuple<float, char>(color.g, 'g' ),
                    new Tuple<float, char>(color.b, 'b')
                };


            List<RankedColor> numberRanks = colors.OrderByDescending(x => (x.Item1, x.Item2)).Select((x, i) => new RankedColor(x.Item1, x.Item2, i + 1)).ToList();

            return numberRanks;
        }

        public static Color GetAirPuffLikeColorMitigation(Color color)
        {
            List<RankedColor> numberRanks = CreateRanks(color);

            Color mitigatedColor = new Color
            {
                a = 1f
            };

            /*foreach (RankedColor rankedColor in numberRanks)
                Log.Warning(rankedColor.ColorValue + " - " + rankedColor.ColorComp + " - " + rankedColor.Rank);*/

            for (int i = 0; i < 3; i++)
            {
                RankedColor curR = numberRanks[i];

                if (curR.ColorComp == 'r')
                    mitigatedColor.r = AirPuffLikeColorMitigation(color.r, i);
                else if (curR.ColorComp == 'g')
                    mitigatedColor.g = AirPuffLikeColorMitigation(color.g, i);
                else if (curR.ColorComp == 'b')
                    mitigatedColor.b = AirPuffLikeColorMitigation(color.b, i);
            }

            return mitigatedColor;
        }

        public static float AirPuffLikeColorMitigation(float originValue, int rank)
        {
            switch (rank)
            {
                case 0:
                    return originValue * .6f;
                case 1:
                    return originValue * .35f;
                case 2:
                    return 0;
                default:
                    return 1;
            }
        }

        public static Color GetBloodSplashLikeColorMitigation(Color color)
        {
            List<RankedColor> numberRanks = CreateRanks(color);

            Color mitigatedColor = new Color
            {
                a = 1f
            };
            /*foreach (RankedColor rankedColor in numberRanks)
    Log.Warning(rankedColor.ColorValue + " - " + rankedColor.ColorComp + " - " + rankedColor.Rank);*/

            for (int i = 0; i < 3; i++)
            {
                RankedColor curR = numberRanks[i];

                if (curR.ColorComp == 'r')
                    mitigatedColor.r = BloodSplashLikeColorMitigation(color.r, i);
                else if (curR.ColorComp == 'g')
                    mitigatedColor.g = BloodSplashLikeColorMitigation(color.g, i);
                else if (curR.ColorComp == 'b')
                    mitigatedColor.b = BloodSplashLikeColorMitigation(color.b, i);
            }
            return mitigatedColor;
        }

        public static float BloodSplashLikeColorMitigation(float originValue, int rank)
        {
            switch (rank)
            {
                case 0:
                    return originValue * .82f;
                case 1:
                    return originValue * .57f;
                case 2:
                    return 0;
                default:
                    return 1;
            }
        }

        public static Color GetBodyImpactLikeColorMitigation(Color color)
        {
            List<RankedColor> numberRanks = CreateRanks(color);

            Color mitigatedColor = new Color
            {
                a = .6f
            };
            /*foreach (RankedColor rankedColor in numberRanks)
    Log.Warning(rankedColor.ColorValue + " - " + rankedColor.ColorComp + " - " + rankedColor.Rank);*/

            for (int i = 0; i < 3; i++)
            {
                RankedColor curR = numberRanks[i];

                if (curR.ColorComp == 'r')
                    mitigatedColor.r = BodyImpactLikeColorMitigation(color.r, i);
                else if (curR.ColorComp == 'g')
                    mitigatedColor.g = BodyImpactLikeColorMitigation(color.g, i);
                else if (curR.ColorComp == 'b')
                    mitigatedColor.b = BodyImpactLikeColorMitigation(color.b, i);
            }
            return mitigatedColor;
        }

        public static float BodyImpactLikeColorMitigation(float originValue, int rank)
        {
            switch (rank)
            {
                case 0:
                    return 1;
                case 1:
                    return .5f;
                    //return Math.Max(1f, originValue * 1.37f);
                case 2:
                    return originValue * .6f;
                default:
                    return 1;
            }
        }

        public static Color GetCustomColorMitigation(Color color, Color customMitigation)
        {
            List<RankedColor> numberRanks = CreateRanks(color);

            Color mitigatedColor = new Color
            {
                a = customMitigation.a
            };
            /*foreach (RankedColor rankedColor in numberRanks)
    Log.Warning(rankedColor.ColorValue + " - " + rankedColor.ColorComp + " - " + rankedColor.Rank);*/

            for (int i = 0; i < 3; i++)
            {
                RankedColor curR = numberRanks[i];

                if (curR.ColorComp == 'r')
                    mitigatedColor.r = CustomColorMitigation(color.r, customMitigation, i);
                else if (curR.ColorComp == 'g')
                    mitigatedColor.g = CustomColorMitigation(color.g, customMitigation, i);
                else if (curR.ColorComp == 'b')
                    mitigatedColor.b = CustomColorMitigation(color.b, customMitigation, i);
            }
            return mitigatedColor;
        }

        // Color color is used as xml interpretable structure here : it's a matrix with weights to change originValue
        // it is not a color
        public static float CustomColorMitigation(float originValue, Color color, int rank)
        {
            switch (rank)
            {
                case 0:
                    return originValue * color.r;
                case 1:
                    return originValue * color.g;
                case 2:
                    return originValue * color.b;
                default:
                    return 1;
            }
        }

    }
}
