using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;

using Verse;

using HarmonyLib;

namespace MoharBlood
{
    public class Harmony_DamageFlash
    {
        private static readonly string BodyPrefix_patchName = nameof(Verse_BodyDamageFlash_HarmonyPatch.OverrideMaterialIfNeeded_Prefix);
        private static readonly string BodyTranspile_patchName = nameof(Verse_BodyDamageFlash_HarmonyPatch.DamagedMatPool_GetDamageFlashMat_Transpile);

        private static readonly string HeadPrefix_patchName = nameof(Verse_HeadDamageFlash_HarmonyPatch.HeadMatAt_Prefix);
        private static readonly string HeadPostfix_patchName = nameof(Verse_HeadDamageFlash_HarmonyPatch.HeadMatAt_Postfix);
        //private static readonly string HeadTranspile_patchName = nameof(Verse_HeadDamageFlash_HarmonyPatch.HeadMat_Transpile);

        private static readonly Type bodyPatchType = typeof(Verse_BodyDamageFlash_HarmonyPatch);
        private static readonly Type headPatchType = typeof(Verse_HeadDamageFlash_HarmonyPatch);

        private static readonly Type patchUtilsType = typeof(OverrideMaterialIfNeeded_Utils);
        private static readonly Type patchHarmonyUtilsType = typeof(Harmony_Utils);

        // Verse PawnRenderer OverrideMaterialIfNeeded
        public static bool Try_OverrideMaterialIfNeeded_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(Verse.PawnRenderer), "OverrideMaterialIfNeeded");
                HarmonyMethod Prefix = new HarmonyMethod(bodyPatchType, "OverrideMaterialIfNeeded_Prefix");
                HarmonyMethod Postfix = new HarmonyMethod(bodyPatchType, "OverrideMaterialIfNeeded_Postfix");
                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + BodyPrefix_patchName + " failed\n" + e);
                return false;
            }
            return true;
        }

        public static bool Try_DamagedMatPool_GetDamageFlashMat_Transpile(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(Verse.DamagedMatPool), "GetDamageFlashMat");
                HarmonyMethod Transpiler = new HarmonyMethod(bodyPatchType, BodyTranspile_patchName);
                myPatch.Patch(Method, transpiler: Transpiler);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + BodyTranspile_patchName + " failed  - " + e);
                return false;
            }
            return true;
        }

        // Verse PawnGraphicSet HeadMatAt
        public static bool Try_HeadMatAt_Patch(Harmony myPatch)
        {
            try
            {
                //MethodBase Method = AccessTools.Method(typeof(Verse.PawnGraphicSet), "HeadMatAt");
                Verse.PawnRenderNodeWorker_Head.

                MethodBase Method = AccessTools.Method(typeof(Verse.PawnGraphicSet), "HeadMatAt");
                //Verse.PawnRenderer.
                
                HarmonyMethod Prefix = new HarmonyMethod(headPatchType, HeadPrefix_patchName);
                HarmonyMethod Postfix = new HarmonyMethod(headPatchType, HeadPostfix_patchName);
                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + HeadPrefix_patchName + "/" + HeadPostfix_patchName + " failed\n" + e);
                return false;
            }
            return true;
        }
        /*
        public static bool Try_HeadMatAt_Transpile(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(Verse.DamagedMatPool), "GetDamageFlashMat");
                HarmonyMethod Transpiler = new HarmonyMethod(headPatchType, HeadTranspile_patchName);
                myPatch.Patch(Method, transpiler: Transpiler);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + HeadTranspile_patchName + " failed  - " + e);
                return false;
            }
            return true;
        }
        */
        public static class Verse_HeadDamageFlash_HarmonyPatch
        {
            /*
            public static bool isEligible = false;
            public static Color newColor = ColoringWayUtils.bugColor;
            */

            public static bool HeadMatAt_Prefix(PawnGraphicSet __instance)
            {
                if (__instance.pawn.GetDamageFlash(out Color gotColor))
                {
                    Verse_BodyDamageFlash_HarmonyPatch.newColor = gotColor;
                    Verse_BodyDamageFlash_HarmonyPatch.isEligible = true;
                }

                return true;
            }

            public static void HeadMatAt_Postfix()
            {
                Verse_BodyDamageFlash_HarmonyPatch.isEligible = false;
                Verse_BodyDamageFlash_HarmonyPatch.newColor = MyDefs.BugColor;
            }

            /*
            public static IEnumerable<CodeInstruction> HeadMat_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo colorInfo = AccessTools.Field(typeof(Verse.DamagedMatPool), "DamagedMatStartingColor");

                List<CodeInstruction> instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];
                    if (instruction.LoadsField(colorInfo))
                    {
                        Log.Error("found DamagedMatStartingColor");

                        Harmony_Utils.LogAround(instructionList, i, -3, 2);

                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Verse_HeadDamageFlash_HarmonyPatch), "isEligible"));
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Verse_HeadDamageFlash_HarmonyPatch), "newColor"));
                        //public static Color BloodColorIfEligible(bool eligible, Color defaultColor, Color bloodColor)
                        yield return CodeInstruction.Call(patchUtilsType, nameof(OverrideMaterialIfNeeded_Utils.BloodColorIfEligible));

                    }
                    else
                        yield return instruction;
                }
            }
            */
        }

        // Verse PawnRenderer OverrideMaterialIfNeeded
        public static class Verse_BodyDamageFlash_HarmonyPatch
        {
            public static bool isEligible = false;
            public static Color newColor = MyDefs.BugColor;

            public static bool OverrideMaterialIfNeeded_Prefix(Pawn pawn)
            {
                if (pawn.GetDamageFlash(out Color gotColor))
                {
                    newColor = gotColor;
                    isEligible = true;
                }

                return true;
            }

            public static void OverrideMaterialIfNeeded_Postfix()
            {
                isEligible = false;
                newColor = MyDefs.BugColor;
            }

            public static IEnumerable<CodeInstruction> DamagedMatPool_GetDamageFlashMat_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo colorInfo = AccessTools.Field(typeof(Verse.DamagedMatPool), "DamagedMatStartingColor");

                List<CodeInstruction> instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];
                    if (instruction.LoadsField(colorInfo))
                    {
                        //Log.Error("found DamagedMatStartingColor");
                        //Harmony_Utils.LogAround(instructionList, i, -3, 2);

                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Verse_BodyDamageFlash_HarmonyPatch), "isEligible"));
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Verse_BodyDamageFlash_HarmonyPatch), "newColor"));
                        //public static Color BloodColorIfEligible(bool eligible, Color defaultColor, Color bloodColor)
                        yield return CodeInstruction.Call(patchUtilsType, nameof(OverrideMaterialIfNeeded_Utils.BloodColorIfEligible));

                    }
                    else
                        yield return instruction;
                }
            }
        }
    }
}
