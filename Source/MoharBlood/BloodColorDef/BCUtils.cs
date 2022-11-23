using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;

namespace MoharBlood
{
    public static class BCUtils
    {
        public static BloodColorSet GetColorSet(this Pawn pawn)
        {
            return MyDefs.AllBloodColorDefs.SelectMany(x => x.bloodSetList.Where(y => y.restriction.race == pawn.def)).FirstOrFallback();
        }

        // Default values color
        public static Color GetDefaultColor(this Pawn pawn)
        {
            if (!(pawn.GetColorSet() is BloodColorSet BCS))
                return Color.white;

            return BCS.defaultValues.color;
        }

        // returns fleshTypeWound data and upper level default color
        public static bool GetPawnFleshTypeWound(this Pawn pawn, out FleshTypeWound fleshTypeWound, out Color defaultColor, bool debug = false)
        {
            if (debug) Log.Warning(pawn.LabelShort + " - Entering GetPawnFleshTypeWound");

            if (!(pawn.GetColorSet() is BloodColorSet bcs))
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetPawnFleshTypeWound - found no blood color set - KO");
                defaultColor = ColoringWayUtils.bugColor;
                fleshTypeWound = null;
                return false;
            }

            if (bcs.fleshTypeWound.fleshTypeDef == pawn.RaceProps.FleshType)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetPawnFleshTypeWound - found blood color set - OK");
                defaultColor = pawn.GetPawnBloodColor(bcs.defaultValues.colorWay);
                fleshTypeWound = bcs.fleshTypeWound;
                return true;
            }

            if (debug) Log.Warning(pawn.LabelShort + " - GetPawnFleshTypeWound - found no blood color set for that fleshtype - KO");
            defaultColor = ColoringWayUtils.bugColor;
            fleshTypeWound = null;
            return false;
        }

        // returns DamageEffecter data and upper level default color
        public static bool GetPawnDamageEffecter(this Pawn pawn, SubEffecter subEffecter, out DamageEffecter damageEffecter, out Color defaultColor, bool debug = false)
        {
            if (debug) Log.Warning(pawn.LabelShort + " - Entering GetPawnDamageEffecter");

            FleckDef fleckDef = subEffecter.def.fleckDef;

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasDamageEffecter)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetPawnDamageEffecter - " + fleckDef?.defName + " found no blood color set - KO");
                defaultColor = ColoringWayUtils.bugColor;
                damageEffecter = null;
                return false;
            }

            if (bcs.damageEffecter.damageEffecterDef == subEffecter.parent.def && bcs.damageEffecter.affectedFleckList.Any(x => x.fleckDef == fleckDef))
            {
                defaultColor = pawn.GetPawnBloodColor(bcs.defaultValues.colorWay);
                damageEffecter = bcs.damageEffecter;

                if (debug) Log.Warning(pawn.LabelShort + " - GetPawnDamageEffecter - " + fleckDef?.defName + "found blood color set - OK - defaultColor:" + defaultColor);

                return true;
            }

            if (debug) Log.Warning(pawn.LabelShort + " - GetPawnDamageEffecter - found no blood color set for that fleshtype - KO");
            defaultColor = ColoringWayUtils.bugColor;
            damageEffecter = null;
            return false;
        }

    }


}
