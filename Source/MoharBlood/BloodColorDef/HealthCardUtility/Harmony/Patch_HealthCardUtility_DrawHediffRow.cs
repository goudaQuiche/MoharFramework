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

using RimWorld;


namespace MoharBlood
{
    public class Harmony_HealthCardUtility_DrawHediffRow
    {
        private static readonly string patchName = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.DrawHediffRow_Transpile);
        private static readonly string DrawHediffRow_Prefix_patchName = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.DrawHediffRow_Prefix);
        private static readonly string nestedPatchName = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.NestedDrawHediffRow_Transpile);
        //private static readonly string nestedPrefix = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.NestedDrawHediffRow_Prefix);

        private static readonly Type patchType = typeof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch);
        private static readonly Type patchUtilsType = typeof(HealthCardUtility_DrawHediffRow_Utils);
        private static readonly Type patchHarmonyUtilsType = typeof(Harmony_Utils);

        // RimWorld HealthCardUtility DrawHediffRow
        public static bool Try_HealthCardUtility_DrawHediffRow_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(RimWorld.HealthCardUtility), "DrawHediffRow");
                HarmonyMethod Transpiler = new HarmonyMethod(patchType, patchName);
                myPatch.Patch(Method, transpiler: Transpiler);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + patchName + " failed  - " + e);
                return false;
            }
            return true;
        }

        public static bool Try_HealthCardUtility_DrawHediffRow_Prefix(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(RimWorld.HealthCardUtility), "DrawHediffRow");
                HarmonyMethod Prefix = new HarmonyMethod(patchType, "DrawHediffRow_Prefix");
                HarmonyMethod Postfix = new HarmonyMethod(patchType, "DrawHediffRow_Postfix");
                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + DrawHediffRow_Prefix_patchName + " failed\n" + e);
                return false;
            }
            return true;
        }

        // RimWorld HealthCardUtility DrawHediffRow
        public static bool Try_HealthCardUtility_NestedDrawHediffRow_Patch(Harmony myPatch)
        {
            try
            {

                MethodBase Method = AccessTools.Method(typeof(RimWorld.HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("<>c__DisplayClass31_1")), "<DrawHediffRow>b__8");
                //MethodBase Method = AccessTools.Method(typeof(RimWorld.HealthCardUtility).GetNestedType("<DrawHediffRow>b__8", BindingFlags.Instance | BindingFlags.NonPublic), "DrawHediffRow");
                HarmonyMethod Transpiler = new HarmonyMethod(patchType, nestedPatchName);
                myPatch.Patch(Method, transpiler: Transpiler);

            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + nestedPatchName + " failed:\n" + e);
                return false;
            }
            return true;
        }

        public static class RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch
        {
            public static Pawn curPawn;

            public static void DrawHediffRow_Prefix(Pawn pawn)
            {
                curPawn = pawn;
            }

            public static void DrawHediffRow_Postfix()
            {

                curPawn = null;
            }

            public static IEnumerable<CodeInstruction> NestedDrawHediffRow_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo bleedRateInfo = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("c__DisplayClass31_1")), "totalBleedRate");
                FieldInfo bleedingInfo = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("c__DisplayClass31_1")), "bleedingIcon");

                FieldInfo pawnInfo = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("c__DisplayClass31_0")), "pawn");
                FieldInfo pawnInfo2 = AccessTools.Field(typeof(HealthCardUtility), "pawn");
                //FieldInfo pawnInfo3 = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("8__locals1")), "pawn");

                /*
                Log.Error(
                    "type: " + pawnInfo + " 1: " + pawnInfo?.ToString()
                    + "\ntype: " + pawnInfo2 + " 2: " + pawnInfo2?.ToString()
                    //+ "\ntype: " + pawnInfo3 + " 3: " + pawnInfo3?.ToString()
                    );
                    */


                //FieldInfo pawnInfo = AccessTools.Field(typeof(HealthCardUtility), "pawn");
                //MethodInfo DrawTextureInfo = AccessTools.Method("DrawTexture", new[] { typeof(Rect), typeof(Texture) });

                List<CodeInstruction> instructionList = instructions.ToList();

                /*
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, bleedingInfo);
                yield return CodeInstruction.Call(patchHarmonyUtilsType, nameof(Harmony_Utils.LogWarningFloat));
                */
                
                //string errLog = string.Empty;
                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];
                    //errLog += "i:" + i + " - " + instruction.ToString() + "\n";

                    //if (instruction.Calls(DrawTextureInfo) && instructionList[i-1].LoadsField(bleedingInfo) )
                    //if (i > 10 && instruction.IsLdarg(0) && instructionList[i - 1].LoadsField(bleedingInfo))
                    if (i > 10 && instructionList[i - 1].LoadsField(bleedingInfo))
                    {
                        Log.Error("found bleedingIcon = BleedingIcon;");
                        Harmony_Utils.LogAround(instructionList, i, -3, 1);
                        yield return instruction;

                        /*
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, pawnInfo);
                        */
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch), "curPawn"));
                        yield return CodeInstruction.Call(patchHarmonyUtilsType, nameof(Harmony_Utils.LogWarningPawn));

                        //GUI.BeginGroup(rect);

                        /*
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, bleedRateInfo);
                        yield return CodeInstruction.Call(patchHarmonyUtilsType, nameof(Harmony_Utils.LogWarningFloat));
                        */

                        /*
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 3.14f);
                        yield return CodeInstruction.Call(patchHarmonyUtilsType, nameof(Harmony_Utils.LogWarningFloat));
                        */

                        //yield return CodeInstruction.Call(patchHarmonyUtilsType, nameof(Harmony_Utils.LogWarning));

                    }
                    else
                        yield return instruction;
                }

                //Log.Error(errLog);
            }

            public static IEnumerable<CodeInstruction> DrawHediffRow_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo staticBleedingTexture2Info = AccessTools.Field(typeof(RimWorld.HealthCardUtility), "BleedingIcon");
                //FieldInfo bleedingInfo = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("c__DisplayClass31_1")), "bleedingIcon");
                //FieldInfo totalBleedRateInfo = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("c__DisplayClass31_1")), "totalBleedRate");

                //MethodInfo minMethodInfo = AccessTools.Method(typeof(UnityEngine.Mathf), "Min", new[] { typeof(Rect), typeof(float) });
                //MethodInfo minMethodInfo = AccessTools.Method(typeof(UnityEngine.Mathf), nameof(Mathf.Min), new[] { typeof(Rect), typeof(float) });
                //MethodInfo lerpMethodInfo = AccessTools.Method(typeof(Verse.GenMath), nameof(GenMath.LerpDouble));
                //public static Rect ContractedBy(this Rect rect, float margin);
                //MethodInfo contractedByMethodInfo = AccessTools.Method(typeof(Verse.GenUI), nameof(GenUI.ContractedBy), new[] { typeof(Rect), typeof(float) });

                /*
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return CodeInstruction.Call(patchHarmonyUtilsType, nameof(Harmony_Utils.LogWarningPawn));
                */
                List<CodeInstruction> instructionList = instructions.ToList();

                //string errLog = string.Empty;

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    //errLog += "i:" + i + " - " + instruction.ToString() + "\n";
                    if (instruction.LoadsField(staticBleedingTexture2Info) && instructionList[i + 1].opcode == OpCodes.Stfld)
                    {
                        /*
                        Log.Error("found bleedingIcon = BleedingIcon;");
                        Harmony_Utils.LogAround(instructionList, i, -1, 6);
                        */

                        // pawn

                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldsfld, staticBleedingTexture2Info);
                        yield return CodeInstruction.Call(patchUtilsType, nameof(HealthCardUtility_DrawHediffRow_Utils.GetBloodDropTexture));

                        //yield return instruction;
                    }
                    else
                        yield return instruction;
                }

                //Log.Error(errLog);
            }
        }
    }
}
