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
        private static readonly string DrawHediffRow_Prefix_patchName = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.DrawHediffRow_Prefix);
        private static readonly string nestedPatchName = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.NestedDrawHediffRow_Transpile);
        //private static readonly string nestedPrefix = nameof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch.NestedDrawHediffRow_Prefix);

        private static readonly Type patchType = typeof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch);
        private static readonly Type patchUtilsType = typeof(HealthCardUtility_DrawHediffRow_Utils);
        private static readonly Type patchHarmonyUtilsType = typeof(Harmony_Utils);

        // RimWorld HealthCardUtility DrawHediffRow
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
                FieldInfo bleedingInfo = AccessTools.Field(typeof(HealthCardUtility).GetNestedTypes(AccessTools.all).First(x => x.Name.Contains("c__DisplayClass31_1")), "bleedingIcon");
                List<CodeInstruction> instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    if (i > 10 && instructionList[i - 1].LoadsField(bleedingInfo))
                    {
                        // saved in the harmony prefix DrawHediffRow_Prefix
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(RimWorld_HealthCardUtility_DrawHediffRow_HarmonyPatch), "curPawn"));
                        //yield return CodeInstruction.Call(patchUtilsType, nameof(HealthCardUtility_DrawHediffRow_Utils.DisplayBloodDrop));
                        yield return CodeInstruction.Call(patchUtilsType, nameof(HealthCardUtility_DrawHediffRow_Utils.DisplayCachedBloodDrop));
                    }
                    else
                        yield return instruction;
                }

                //Log.Error(errLog);
            }

        }
    }
}
