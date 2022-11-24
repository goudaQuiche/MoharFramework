//using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
//using System.Collections.Generic;

using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

/*
using RimWorld;

using Verse;
*/


namespace MoharBlood
{
    public static class SubEffecter_Sprayer_Utils
    {
        public static void LogWarning()
        {
            Log.Warning("LogWarning Verse_SubEffecter_Sprayer_MakeMote Transpiler");
        }

        public static void LogWarningTarget(TargetInfo A)
        {
            Log.Warning("LogWarningTarget :" + A.HasThing + " - " + A.Thing?.def);
        }

        public static void LogWarningSubEffecterDef(SubEffecterDef SEDef)
        {
            Log.Warning(
                "LogWarningSubEffecterDef :" + SEDef.fleckDef);
        }

        public static void LogWarningColor(Color color)
        {
            Log.Warning("LogWarningColor :" + color);
        }

        /*
        public static Color? GetDamageEffecterColor(TargetInfo A, SubEffecterDef SEDef, Color alreadySetColor)
        {
            if (!A.HasThing || (!(A.Thing is Pawn pawn)))
            {
                Log.Warning("found no pawn");
                //return ColoringWayUtils.bugColor;
                return alreadySetColor;
            }

            if (pawn.GetPawnDamageEffecter(SEDef.fleckDef, out DamageEffecter damageEffecter, out Color defaultColor, true))
            //if (pawn.GetPawnDamageEffecter(SEDef.fleckDef, out DamageEffecter damageEffecter, out Color defaultColor))
            {
                Color pickedColor = defaultColor;
                ColoringWay coloringWay = ColoringWay.Unset;

                if (damageEffecter.HasColorWay)
                {
                    coloringWay = damageEffecter.colorSet.colorWay;
                    Log.Warning("Found coloringway:" + coloringWay.DescriptionAttr());
                }


                //Log.Warning(pawn.LabelShort + " found WoundColorAssociation for " + wound.texture + " : " + pickedColor.DescriptionAttr());

                Color newColor = coloringWay == ColoringWay.Unset ? defaultColor : pawn.GetPawnBloodColor(coloringWay);

                // Apply color mitigator depending on mitigation
                if (damageEffecter.affectedFleckList.Where(x => x.fleckDef == SEDef.fleckDef).FirstOrFallback() is FleckMitigatedColor fmc)
                {
                    Color mitigatedColor = fmc.HasColorMitigation ? MitigateFleckColor.GetMitigatedColor(newColor, fmc) : newColor;
                    Log.Warning(" GetDamageEffecterColor " + SEDef.fleckDef.defName + "\n newColor:" + newColor + " - mitigatedColor:" + mitigatedColor);
                    return mitigatedColor;
                }
                else
                    return alreadySetColor;

            }

            //return ColoringWayUtils.bugColor;
            return alreadySetColor;
        }
        */

        public static ThingDef GetJobMoteReplacement(TargetInfo A, TargetInfo B, SubEffecter SE)
        {
            //Log.Warning("GetJobMoteReplacement");

            if (!MyDefs.AllJobMoteEffecterDef.Contains(SE.parent.def))
            {
                //Log.Warning("GetJobMoteReplacement effecterDef is not valid");
                //return ColoringWayUtils.bugColor;
                return SE.def.moteDef;
            }


            //Log.Warning("A: " + A.Thing + "B: " + B.Thing);
            if (SE.parent.def == MyEffecterDefOf.Surgery && B.Thing is Pawn pawn)
            {
                //if( pawn.GetJobMote(SE, out JobMote jobMote, true))
                if (pawn.GetJobMote(SE, out JobMote jobMote))
                {
                    //Log.Warning("thing" + B.Thing);
                    return jobMote.replacementMotePool.RandomElementWithFallback(SE.def.moteDef);
                }
            }
            else if (SE.parent.def == MyEffecterDefOf.ButcherFlesh && A.Thing is Pawn butcher)
            {
                //Log.Warning("butcher:" + butcher + " - corpse: " + butcher.CurJob.targetA.Thing + " - " + butcher.CurJob.targetB.Thing + " - " + butcher.CurJob.targetC.Thing);
                if (!(butcher.CurJob.targetB.Thing is Corpse corpse))
                    return SE.def.moteDef;

                if (corpse.GetJobMoteCorpse(SE, out JobMote jobMote))
                {
                    return jobMote.replacementMotePool.RandomElementWithFallback(SE.def.moteDef);
                }
            }

            //Log.Warning("SE.parent: " + SE.parent.def.defName);

            return SE.def.moteDef;
        }

        public static Color GetJobMoteColor(TargetInfo A, TargetInfo B, SubEffecter SE)
        {
            //Log.Warning("GetJobMoteColor");
            Color alreadySetColor = SE.EffectiveColor;

            if (!MyDefs.AllJobMoteEffecterDef.Contains(SE.parent.def))
            {
                //Log.Warning("GetJobMoteColor found no thing B:");
                //Log.Warning("found no pawn");
                return alreadySetColor;
            }

            //Log.Warning("A: " + A.Thing + "B: " + B.Thing);
            if (SE.parent.def == MyEffecterDefOf.Surgery && B.Thing is Pawn pawn)
            {
                //if (pawn.GetJobMotePawnColor(SE, out Color newColor, true))
                if (pawn.GetJobMotePawnColor(SE, out Color newColor))
                {
                    return newColor;
                }
            }
            else if (SE.parent.def == MyEffecterDefOf.ButcherFlesh && A.Thing is Pawn butcher)
            {
                //Log.Warning("butcher:" + butcher + " - corpse: " + butcher.CurJob.targetA.Thing + " - "+ butcher.CurJob.targetB.Thing + " - " + butcher.CurJob.targetC.Thing);

                if (!(butcher.CurJob.targetB.Thing is Corpse corpse))
                    return alreadySetColor;

                if (corpse.GetJobMoteCorpseColor(SE, out Color newColor))
                {
                    return newColor;
                }
            }


            return alreadySetColor;
        }

        public static Color? GetDamageEffecterColor(TargetInfo A, SubEffecter SE)
        {
            SubEffecterDef SEDef = SE.def;
            Color alreadySetColor = SE.EffectiveColor;

            if (!A.HasThing || (!(A.Thing is Pawn pawn)))
            {
                //Log.Warning("found no pawn");
                //return ColoringWayUtils.bugColor;
                return alreadySetColor;
            }

            //if (pawn.GetPawnDamageEffecter(SE, out DamageEffecter damageEffecter, out Color defaultColor, true))
            if (pawn.GetPawnDamageEffecter(SE, out DamageEffecter damageEffecter, out Color defaultColor))
            {
                Color pickedColor = defaultColor;
                ColoringWay coloringWay = ColoringWay.Unset;

                if (damageEffecter.HasColorWay)
                {
                    coloringWay = damageEffecter.colorSet.colorWay;
                    //Log.Warning("Found coloringway:" + coloringWay.DescriptionAttr());
                }

                //Log.Warning(pawn.LabelShort + " found WoundColorAssociation for " + wound.texture + " : " + pickedColor.DescriptionAttr());

                Color newColor = coloringWay == ColoringWay.Unset ? defaultColor : pawn.GetPawnBloodColor(coloringWay);

                // Apply color mitigator depending on mitigation
                if (damageEffecter.affectedFleckList.Where(x => x.fleckDef == SEDef.fleckDef).FirstOrFallback() is FleckMitigatedColor fmc)
                {
                    Color mitigatedColor = fmc.HasColorMitigation ? MitigateFleckColor.GetMitigatedColor(newColor, fmc) : newColor;
                    //Log.Warning(" GetDamageEffecterColor " + SEDef.fleckDef.defName + "\n newColor:" + newColor + " - mitigatedColor:" + mitigatedColor);
                    return mitigatedColor;
                }
                else
                    return alreadySetColor;

            }

            //return ColoringWayUtils.bugColor;
            return alreadySetColor;
        }
    }
}
