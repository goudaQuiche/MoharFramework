using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace MoharBlood
{
    public class Harmony_DamageWorker_AddInjury_ApplyToPawn
    {

        //private static readonly Type patchType = typeof(DamageWorker_AddInjury_ApplyToPawn_HarmonyPatch);

        /*
        public static bool Try_DamageWorker_AddInjury_ApplyToPawn_Transpiler_Patch(Harmony myPatch)
        {
            string patchName = nameof(DamageWorker_AddInjury_ApplyToPawn_HarmonyPatch.DamageWorker_AddInjury_ApplyToPawn_Transpile);

            try
            {
                MethodBase Method = AccessTools.Method(typeof(DamageWorker_AddInjury), "ApplyToPawn");
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
        
        /*
        public static class DamageWorker_AddInjury_ApplyToPawn_HarmonyPatch
        {

            public static void SetSubEffecterColor( Pawn pawn)
            {
                Effecter effecter = pawn.health.woundedEffecter;

                Log.Warning(pawn.LabelShort + " - SetSubEffecterColor" + " - effecter " + effecter.def.defName);

                if (effecter.def.GetModExtension<PawnColoredEffecterExtension>() is PawnColoredEffecterExtension pcee)
                {
                    Color newColor = pawn.GetWoundColor(pcee.bloodColor, Color.white);

                    foreach (SubEffecter se in effecter.children)
                    {
                        Log.Warning("changing " + se.def.fleckDef.defName + " from " + se.EffectiveColor + " colour to " + newColor);
                        
                        se.colorOverride = newColor;
                    }
                }
            }
            
            public static void LogWarning()
            {
                Log.Warning("DamageWorker_AddInjury_ApplyToPawn_Transpile");
            }

            public static IEnumerable<CodeInstruction> DamageWorker_AddInjury_ApplyToPawn_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo woundedEffecterInfo = AccessTools.Field(typeof(Verse.Pawn_HealthTracker), nameof(Verse.Pawn_HealthTracker.woundedEffecter));
                FieldInfo pawnHealthInfo = AccessTools.Field(typeof(Verse.Pawn), nameof(Pawn.health));

                MethodInfo callTriggersInfo = AccessTools.Method(typeof(Effecter), "Trigger");

                List<CodeInstruction> instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    // [-12]    IL_011b: ldfld        class Verse.Pawn_HealthTracker Verse.Pawn::health
                    // [-11]    IL_0120: ldfld        class Verse.Effecter Verse.Pawn_HealthTracker::woundedEffecter
                    // [0]      IL_013d: callvirt     instance void Verse.Effecter::Trigger(valuetype Verse.TargetInfo, valuetype Verse.TargetInfo, int32)
                    if (i > 15 && instruction.Calls(callTriggersInfo) && instructionList[i - 11].LoadsField(woundedEffecterInfo) && instructionList[i - 12].LoadsField(pawnHealthInfo))
                    {
                        Log.Error(
                            "found Trigger call - " + i + " - " + instruction.ToString() + "\n" +
                            "[-11] :" + instructionList[i - 11].ToString() + "\n" +
                            "[-12] :" + instructionList[i - 12].ToString()
                        );

                        yield return instruction;

                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(SetSubEffecterColor)));

                        //yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(LogWarning)));
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        */
    }
}