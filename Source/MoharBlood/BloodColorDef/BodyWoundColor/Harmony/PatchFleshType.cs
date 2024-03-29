﻿using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace MoharBlood
{
    public class Harmony_FleshTypeDef
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
            public static FleshTypeDef.ResolvedWound MyResolve(FleshTypeDef.Wound wound, Pawn pawn, FleshTypeWound woundData, Color defaultColor)
            {
                Shader shader = wound.tintWithSkinColor ? ShaderDatabase.WoundSkin : ShaderDatabase.Wound;

                if (wound.texture == null)
                {
                    return new FleshTypeDef.ResolvedWound(
                    wound,
                    MaterialPool.MatFrom(wound.textureSouth, shader, wound.color),
                    MaterialPool.MatFrom(wound.textureEast, shader, wound.color),
                    MaterialPool.MatFrom(wound.textureNorth, shader, wound.color),
                    MaterialPool.MatFrom(wound.textureWest, shader, wound.color),
                    wound.flipSouth, wound.flipEast, wound.flipNorth, wound.flipWest
                    );
                }

                Color pickedColor = defaultColor;
                ColoringWay colorOneWay = ColoringWay.Unset;
                ColoringWay colorTwoWay = ColoringWay.Unset;
                int colorNum = 1;

                if (woundData.woundColorList.Where(w => w.textureList.Contains(wound.texture)).FirstOrFallback() is WoundColorAssociation wca)
                {
                    colorOneWay = wca.HasColorOne ? wca.colorOne.colorWay : ColoringWay.Unset;
                    colorTwoWay = wca.HasColorTwo ? wca.colorTwo.colorWay : ColoringWay.Unset;
                    if (wca.HasColorTwo) colorNum += 1;
                    //Log.Warning(pawn.LabelShort + " found WoundColorAssociation for " + wound.texture + " : " + pickedColor.DescriptionAttr());
                }

                Color newColorOne = colorOneWay == ColoringWay.Unset ? defaultColor : pawn.GetPawnBloodColor(colorOneWay);
                Color newColorTwo = colorTwoWay == ColoringWay.Unset ? defaultColor : pawn.GetPawnBloodColor(colorTwoWay);
                //Log.Warning(pawn.LabelShort + " Color : " + newColor);

                if (colorNum == 1)
                    return new FleshTypeDef.ResolvedWound(wound, MaterialPool.MatFrom(wound.texture, shader, newColorOne));

                return new FleshTypeDef.ResolvedWound(wound, GetTwoColorsWoundMaterial(wound.texture, newColorOne, newColorTwo));
            }

            public static Material GetTwoColorsWoundMaterial(string path, Color colorOne, Color colorTwo)
            {
                MaterialRequest MR = default(MaterialRequest);

                MR.mainTex = ContentFinder<Texture2D>.Get(path, reportFailure: true);
                MR.shader = ShaderDatabase.CutoutComplex;
                MR.maskTex = ContentFinder<Texture2D>.Get(path + Graphic_Single.MaskSuffix, reportFailure: true);
                MR.color = colorOne;
                MR.colorTwo = colorTwo;

                Material mat = MaterialPool.MatFrom(MR);

                //Log.Warning(" - GetBloodDropMaterial - trying material - " + path + " - color : " + newColor);
                return mat;
            }

            public static bool FleshTypeDef_ChooseWoundOverlay_Prefix(FleshTypeDef __instance, Hediff hediff, ref FleshTypeDef.ResolvedWound __result)
            {
                //if (!hediff.pawn.GetPawnFleshTypeWound(out FleshTypeWound ftwd, out Color defaultColor, true))
                if (!hediff.pawn.GetPawnFleshTypeWound(out FleshTypeWound ftwd, out Color defaultColor))
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
                            select MyResolve(wound, hediff.pawn, ftwd, defaultColor)
                        ).ToList<FleshTypeDef.ResolvedWound>();
                }
                __result = myWoundsResolved.RandomElement<FleshTypeDef.ResolvedWound>();
                //return __instance.woundsResolved.RandomElement<FleshTypeDef.ResolvedWound>();

                return false;

            }
        }
    }
}