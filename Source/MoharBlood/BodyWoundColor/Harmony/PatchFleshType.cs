using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace MoharBlood
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            Harmony MoharBlood_HarmonyPatch = new Harmony("MoharFW.MoharBlood");

            // Check if any def requiring patching
            if (!MyDefs.FTBCD.EnumerableNullOrEmpty())
            {
                if (HarmonyPatch_FleshTypeDef_Trigger.Try_FleshTypeDef_ResolveWound_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched FleshTypeDef.ChooseWoundOverlay successfully.");
            }

        }
    }

    public class HarmonyPatch_FleshTypeDef_Trigger
    {

        // FleshTypeDef.ResolvedWound
        public static bool Try_FleshTypeDef_ResolveWound_Patch(Harmony myPatch)
        {
            string patchName = nameof(FleshTypeDef_HarmonyPatch.FleshTypeDef_ChooseWoundOverlay_Prefix);

            try
            {
                MethodBase Method = AccessTools.Method(typeof(FleshTypeDef), "ChooseWoundOverlay");
                HarmonyMethod Prefix = new HarmonyMethod(typeof(FleshTypeDef_HarmonyPatch), patchName);
                HarmonyMethod Postfix = null;
                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + patchName + " failed  - " + e);
                return false;
            }
            return true;
        }

        public static class FleshTypeDef_HarmonyPatch
        {
            public static FleshTypeDef.ResolvedWound MyResolve(FleshTypeDef.Wound wound, Pawn pawn, FleshTypeWoundColor ftbc)
            {
                Shader shader = wound.tintWithSkinColor ? ShaderDatabase.WoundSkin : ShaderDatabase.Wound;
                if (wound.texture != null)
                {
                    BloodColor pickedColor = ftbc.fallbackColor;

                    if (ftbc.woundColor.Where(w => w.textures.Contains(wound.texture)).FirstOrFallback() is WoundColorAssociation wca)
                    {
                        pickedColor = wca.color;
                        //Log.Warning(pawn.LabelShort + " found WoundColorAssociation for " + wound.texture + " : " + pickedColor.DescriptionAttr());
                    }

                    Color newColor = pawn.GetWoundColor(pickedColor, wound.color);
                    //Log.Warning(pawn.LabelShort + " Color : " + newColor);

                    return new FleshTypeDef.ResolvedWound(wound, MaterialPool.MatFrom(wound.texture, shader, newColor));
                }
                return new FleshTypeDef.ResolvedWound(
                    wound,
                    MaterialPool.MatFrom(wound.textureSouth, shader, wound.color),
                    MaterialPool.MatFrom(wound.textureEast, shader, wound.color),
                    MaterialPool.MatFrom(wound.textureNorth, shader, wound.color),
                    MaterialPool.MatFrom(wound.textureWest, shader, wound.color),
                    wound.flipSouth, wound.flipEast, wound.flipNorth, wound.flipWest
                );

            }

            static FleshTypeWoundColor GetFleshTypeBloodColor(Hediff hediff)
            {
                return
                MyDefs.FTBCD.Where(
                    f => f.fleshTypeWoundColor.Any(
                        x => x.fleshTypeDef == hediff.pawn.RaceProps.FleshType
                    )
                )
                .SelectMany(x => x.fleshTypeWoundColor)
                .FirstOrFallback();

                /*
                foreach (FleshTypeBloodColorDef ftbcd in MyDefs.FTBCD)
                    if (ftbcd.fleshTypeBloodColor.Where(x => x.fleshTypeDef == hediff.pawn.RaceProps.FleshType).FirstOrFallback() is FleshTypeBloodColor answer)
                        return answer;
                        */
                /*
                foreach (FleshTypeBloodColorDef ftbcd in MyDefs.FTBCD)
                    foreach (FleshTypeBloodColor ftbc in ftbcd.fleshTypeBloodColor)
                        if (ftbc.fleshTypeDef == hediff.pawn.RaceProps.FleshType)
                            return ftbc;
                */
            }

            public static bool FleshTypeDef_ChooseWoundOverlay_Prefix(FleshTypeDef __instance, Hediff hediff, ref FleshTypeDef.ResolvedWound __result)
            {
                FleshTypeWoundColor ChosenFtbc = GetFleshTypeBloodColor(hediff);

                if (ChosenFtbc == null)
                {
                    // if we did not find a pawn with the fleshType we use regular method
                    return true;
                }

                if (__instance.genericWounds == null)
                {
                    __result = null;
                    return false; ;
                }
                if (__instance.hediffWounds != null)
                {
                    foreach (FleshTypeDef.HediffWound hediffWound in __instance.hediffWounds)
                    {
                        if (hediffWound.hediff == hediff.def)
                        {
                            FleshTypeDef.ResolvedWound resolvedWound = hediffWound.ChooseWoundOverlay(hediff);
                            if (resolvedWound != null)
                            {
                                if (hediff.IsTended())
                                {
                                    __result = __instance.ChooseBandagedOverlay();
                                    return false;
                                }
                                __result = resolvedWound;
                                return false;
                            }
                        }
                    }
                }
                Hediff_MissingPart hediff_MissingPart;
                if (!(hediff is Hediff_Injury) && ((hediff_MissingPart = (hediff as Hediff_MissingPart)) == null || !hediff_MissingPart.IsFresh))
                {
                    __result = null;
                    return false;
                }
                if (hediff.IsTended())
                {
                    __result = __instance.ChooseBandagedOverlay();
                    return false;
                }

                FieldInfo myWoundsResolvedFieldInfo = __instance.GetType().GetField("woundsResolved", BindingFlags.NonPublic | BindingFlags.Instance);
                List<FleshTypeDef.ResolvedWound> myWoundsResolved = (List<FleshTypeDef.ResolvedWound>)myWoundsResolvedFieldInfo.GetValue(__instance);

                if (myWoundsResolved == null)
                {
                    myWoundsResolved = (
                            from wound in __instance.genericWounds
                                // select wound.Resolve()
                            select MyResolve(wound, hediff.pawn, ChosenFtbc)
                        ).ToList<FleshTypeDef.ResolvedWound>();
                }
                __result = myWoundsResolved.RandomElement<FleshTypeDef.ResolvedWound>();
                //return __instance.woundsResolved.RandomElement<FleshTypeDef.ResolvedWound>();

                return false;

            }
        }
    }
}