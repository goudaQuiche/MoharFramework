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
            return MyDefs.AllBloodColorDefs.SelectMany(x => x.bloodSetList.Where(y => y.restriction.race.Contains(pawn.def))).FirstOrFallback();
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

            defaultColor = MyDefs.BugColor;
            fleshTypeWound = null;

            if (!(pawn.GetColorSet() is BloodColorSet bcs))
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetPawnFleshTypeWound - found no blood color set - KO");
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
                defaultColor = MyDefs.BugColor;
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
            defaultColor = MyDefs.BugColor;
            damageEffecter = null;
            return false;
        }

        public static bool GetJobMote(this Pawn pawn, SubEffecter subEffecter, out JobMote jobMote, bool debug = false)
        {
            if (debug) Log.Warning(pawn.LabelShort + " - Entering GetJobMote");

            ThingDef moteDef = subEffecter.def.moteDef;
            jobMote = null;

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasJobMote)
            {
                if (debug) Log.Warning(pawn.LabelShort + " - GetJobMote - " + moteDef?.defName + " found no jobMote - KO");
                return false;
            }

            //if (bcs.damageEffecter.damageEffecterDef == subEffecter.parent.def && bcs.damageEffecter.affectedFleckList.Any(x => x.fleckDef == fleckDef))
            if (bcs.jobMote.Where(x => x.effectWorking == subEffecter.parent.def).FirstOrFallback() is JobMote jm && subEffecter.def.moteDef == jm.originMote)
            {
                jobMote = jm;

                if (debug) Log.Warning(pawn.LabelShort + " - GetJobMote - " + jm.originMote.defName + "found jobMote - OK - " + jm.effectWorking);

                return true;
            }

            if (debug) Log.Warning(pawn.LabelShort + " - GetJobMote - found no blood color set for that fleshtype - KO");
            return false;
        }

        public static bool GetJobMoteCorpse(this Corpse corpse, SubEffecter subEffecter, out JobMote jobMote, bool debug = false)
        {
            ThingDef moteDef = subEffecter.def.moteDef;
            jobMote = null;
            if (corpse.InnerPawn == null)
                return false;

            Pawn pawn = corpse.InnerPawn;

            return pawn.GetJobMote(subEffecter, out jobMote);
        }

        public static bool GetJobMotePawnColor(this Pawn pawn, SubEffecter subEffecter, out Color defaultColor, bool debug = false)
        {
            if (debug) Log.Warning(pawn.LabelShort + " - Entering GetJobMoteColor");

            FleckDef fleckDef = subEffecter.def.fleckDef;

            if (!(pawn.GetColorSet() is BloodColorSet bcs) || !bcs.HasJobMote)
            {
                defaultColor = MyDefs.BugColor;
                if (debug) Log.Warning(pawn.LabelShort + " - GetJobMoteColor - " + defaultColor + " found no jobMote - KO");
                return false;
            }

            if (bcs.jobMote.Where(x => x.effectWorking == subEffecter.parent.def).FirstOrFallback() is JobMote jm && subEffecter.def.moteDef == jm.originMote)
            {
                defaultColor = pawn.GetPawnBloodColor(jm.HasColorWay ? jm.colorSet.colorWay : bcs.defaultValues.colorWay);
                if (debug) Log.Warning(pawn.LabelShort + " - GetJobMoteColor - found blood color set - OK - defaultColor:" + defaultColor);

                return true;
            }

            if (debug) Log.Warning(pawn.LabelShort + " - GetJobMoteColor - found no blood color set for that fleshtype - KO");
            defaultColor = MyDefs.BugColor;
            return false;
        }

        public static bool GetJobMoteCorpseColor(this Corpse corpse, SubEffecter subEffecter, out Color defaultColor, bool debug = false)
        {
            defaultColor = MyDefs.BugColor;
            if (corpse.InnerPawn == null)
                return false;

            Pawn pawn = corpse.InnerPawn;

            return pawn.GetJobMotePawnColor(subEffecter, out defaultColor);
        }



    }


}
