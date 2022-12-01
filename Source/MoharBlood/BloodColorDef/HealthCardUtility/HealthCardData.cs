using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;

namespace MoharBlood
{
    public static class HealthCardData
    {

        public static bool GetHealthTabBleeding(this Pawn pawn, out HealthTabBleeding healthTabBleeding, out Color defaultColor, bool debug = false)
        {
            if (debug) Log.Warning(pawn.LabelShort + " - Entering GetHealthTabBleeding");

            healthTabBleeding = null;
            defaultColor = ColoringWayUtils.bugColor;

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasHealthTabBleeding)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetHealthTabBleeding - found no healthTabBleeding - KO");
                return false;
            }

            if (bcs.healthTabBleeding is HealthTabBleeding htb)
            {
                healthTabBleeding = htb;
                defaultColor = pawn.GetPawnBloodColor(htb.HasColorWay ? htb.colorSet.colorWay : bcs.defaultValues.colorWay);

                if (debug) Log.Warning(pawn.LabelShort + " - GetHealthTabBleeding - found healthTabBleeding - OK - ");

                return true;
            }

            if (debug) Log.Warning(pawn.LabelShort + " - GetHealthTabBleeding - found no blood color set for that fleshtype - KO");
            return false;
        }


    }


}
