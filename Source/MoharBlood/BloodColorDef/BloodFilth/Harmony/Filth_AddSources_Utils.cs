﻿using Verse;
using UnityEngine;
using System.Collections.Generic;
//using System;
using RimWorld;

using System.Linq;

namespace MoharBlood
{
    public static class Filth_AddSources_Utils
    {
        //public static bool GetBloodfilth(IEnumerable<string> sources, out BloodFilth bloodFilth, out Color defaultColor, bool debug = false)
        public static bool GetBloodfilth(IEnumerable<string> sources, ColorableFilth filth, bool debug = false)
        {
            if (debug) Log.Warning(" Entering GetBloodfilth");

            if (!(PawnsFinder.All_AliveOrDead.Where(p => sources.Contains(p.LabelShort)).FirstOrFallback() is Pawn pawn))
                return false;

            //Log.Warning("found " + pawn.LabelShort);

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasHealthTabBleeding)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found no bloodFilth - KO");
                return false;
            }
            Color newColor = MyDefs.BugColor;

            if (!(bcs.bloodFilth is BloodFilth bf))
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found no blood color set for that fleshtype - KO");
                return false;
            }

            newColor = pawn.GetPawnBloodColor(bf.HasColorWay ? bf.colorSet.colorWay : bcs.defaultValues.colorWay);
            if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found bloodFilth - OK - newColor: " + newColor);
            newColor = bf.HasColorMitigation ? MitigateFleckColor.GetMitigatedColor(newColor, bf.mitigation) : newColor;
            if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found bloodFilth - OK - mitigated(" + bf.HasColorMitigation + "): " + newColor);

            filth.Init(newColor);
            //filth.SetColor(newColor);

            if (debug) Log.Warning("DrawColor:" + filth.DrawColor);

            return true;
        }

        public static float AngleFromRot(this Graphic graphic, Rot4 rot)
        {
            if (graphic.ShouldDrawRotated)
            {
                float asAngle = rot.AsAngle;
                asAngle += graphic.DrawRotatedExtraAngleOffset;
                if ((rot == Rot4.West && graphic.WestFlipped) || (rot == Rot4.East && graphic.EastFlipped))
                {
                    asAngle += 180f;
                }
                return asAngle;
            }
            return 0f;
        }

        /*
        public static bool GetBloodfilth(IEnumerable<string> sources, Filth filth, bool debug = false)
        {
            if (debug) Log.Warning(" Entering GetBloodfilth");

            if (!(PawnsFinder.All_AliveOrDead.Where(p => sources.Contains(p.LabelShort)).FirstOrFallback() is Pawn pawn))
                return false;

            //Log.Warning("found " + pawn.LabelShort);

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasHealthTabBleeding)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found no bloodFilth - KO");
                return false;
            }
            Color newColor = ColoringWayUtils.bugColor;

            if (bcs.bloodFilth is BloodFilth bf)
            {

                newColor = pawn.GetPawnBloodColor(bf.HasColorWay ? bf.colorSet.colorWay : bcs.defaultValues.colorWay);
                if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found bloodFilth - OK - newColor: " + newColor);
                newColor = bf.HasColorMitigation ? MitigateFleckColor.GetMitigatedColor(newColor, bf.mitigation) : newColor;
                if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found bloodFilth - OK - mitigated(" + bf.HasColorMitigation + "): " + newColor);

                filth.DrawColor = newColor;
                //filth.SetColor(newColor);

                if (debug) Log.Warning("DrawColor:" + filth.DrawColor);

                return true;
            }

            if (debug) Log.Warning(pawn.LabelShort + " - GetBloodfilth - found no blood color set for that fleshtype - KO");
            return false;
        }
        */
    }
}
