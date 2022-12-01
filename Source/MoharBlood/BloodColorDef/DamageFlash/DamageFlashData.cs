using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;

namespace MoharBlood
{
    public static class DamageFlashData
    {

        public static bool GetDamageFlash(this Pawn pawn, out Color defaultColor, bool debug = false)
        {
            if (debug) Log.Warning(pawn.LabelShort + " - Entering GetDamageFlash");

            defaultColor = ColoringWayUtils.bugColor;

            if(StaticCollections.DamageFlashGetCache(pawn, out bool isEligible, out Color cacheColor))
            {
                defaultColor = cacheColor;
                if (debug)
                    Log.Warning(
                        pawn.LabelShort + " - GetDamageFlash - found cache : "
                        + " isEligible : " + isEligible
                        + " cacheColor : " + cacheColor
                    );
                return isEligible;
            }

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasDamageFlash)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetDamageFlash - found no damageFlash - KO");

                StaticCollections.DamageFlashAddIneligibleCache(pawn);

                return false;
            }

            if (bcs.damageFlasher is DamageFlash df)
            {
                //damageFlash = df;
                defaultColor = pawn.GetPawnBloodColor(df.HasColorWay ? df.colorSet.colorWay : bcs.defaultValues.colorWay);

                if (debug) Log.Warning(pawn.LabelShort + " - GetDamageFlash - found damageFlash - OK - color : " + defaultColor);
                StaticCollections.DamageFlashAddEligibleCache(pawn, defaultColor);

                return true;
            }

            StaticCollections.DamageFlashAddIneligibleCache(pawn);
            if (debug) Log.Warning(pawn.LabelShort + " - GetDamageFlash - found no blood color set for pawn def - KO");
            return false;
        }


    }


}
