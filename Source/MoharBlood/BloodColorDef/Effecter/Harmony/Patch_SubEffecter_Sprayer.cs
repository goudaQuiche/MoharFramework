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
    public class Harmony_Sprayer_MakeMote
    {
        private static string patchName = nameof(Verse_SubEffecter_Sprayer_MakeMote_HarmonyPatch.MakeMote_Transpile);
        private static readonly Type patchType = typeof(Verse_SubEffecter_Sprayer_MakeMote_HarmonyPatch);

        // Verse.Effecter.Trigger
        public static bool Try_SubEffecter_Sprayer_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(SubEffecter_Sprayer), "MakeMote");
                /*
                HarmonyMethod Prefix = new HarmonyMethod(typeof(Verse_SubEffecter_Sprayer_MakeMote_HarmonyPatch), patchName);
                HarmonyMethod Postfix = null;
                myPatch.Patch(Method, Prefix, Postfix);
                */
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

        public static class Verse_SubEffecter_Sprayer_MakeMote_HarmonyPatch
        {
            /*
            public static bool MakeMote_Prefix(Effecter __instance, TargetInfo A, TargetInfo B)
            {

                Log.Warning("MakeMote_Prefix - " + A.HasThing + " - " + A.Thing?.def.defName);
                //Log.Warning(p?.LabelShort + "VerseEffecterTrigger_Prefix");

                return true;
            }
            */

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
                        Color mitigatedColor = MitigateFleckColor.GetMitigatedColor(newColor, fmc.mitigation);
                        Log.Warning(" GetDamageEffecterColor " + SEDef.fleckDef.defName + "\n newColor:" + newColor + " - mitigatedColor:" + mitigatedColor);
                        return mitigatedColor;
                    }
                    else
                        return alreadySetColor;

                }

                //return ColoringWayUtils.bugColor;
                return alreadySetColor;
            }

            public static IEnumerable<CodeInstruction> MakeMote_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo callEffectiveColorInfo = AccessTools.Method(typeof(SubEffecter), "get_EffectiveColor");
                FieldInfo rotationInfo = AccessTools.Field(typeof(FleckCreationData), "rotation");

                FieldInfo DefInfo = AccessTools.Field(typeof(SubEffecter_Sprayer), "def");

                List<CodeInstruction> instructionList = instructions.ToList();

                //yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(LogWarning)));

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    if (/*i > 600 && i < 800 &&*/
                        instruction.IsLdarg(0) &&
                        instructionList[i + 1].Calls(callEffectiveColorInfo) &&
                        instructionList[i - 2].StoresField(rotationInfo)
                        /*
                        instruction.Calls(callEffectiveColorInfo) && 
                        instructionList[i - 1].IsLdarg(0) && 
                        instructionList[i - 3].StoresField(rotationInfo)
                        */
                        )
                    {

                        /*
                        Log.Error(
                            "found Fleck Instance color call - " + i + "\n" +
                            "[" + (i - 3) + "]" + instructionList[i - 3].ToString() + "\n" +
                            "[" + (i - 2) + "]" + instructionList[i - 2].ToString() + "\n" +
                            "[" + (i - 1) + "]" + instructionList[i - 1].ToString() + "\n" +
                            "[" + i + "]" + instruction.ToString() + "\n" +
                            "[" + (i + 1) + "]" + instructionList[i + 1].ToString()
                        );
                        */

                        //GetDamageEffecterColor( A, this.def, this.EffectiveColor)
                        // A
                        yield return new CodeInstruction(OpCodes.Ldarg_1);

                        //this.def
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, DefInfo);

                        //this.EffectiveColor
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, callEffectiveColorInfo);

                        // = GetDamageEffecterColor (
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(GetDamageEffecterColor)));

                        // skipping this.EffectiveColor ; this = the line; EffectiveColor = next line
                        i += 1;
                        
                        //yield return instruction;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }


            public static IEnumerable<CodeInstruction> MakeMote_PileOfShit_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo MoteInfo = AccessTools.Field(typeof(SubEffecter_Sprayer), "mote");
                FieldInfo DefInfo = AccessTools.Field(typeof(SubEffecter_Sprayer), "def");
                FieldInfo InstanceColorInfo = AccessTools.Field(typeof(Verse.Mote), nameof(Mote.instanceColor));

                MethodInfo callEffectiveColorInfo = AccessTools.Method(typeof(SubEffecter), "get_EffectiveColor");

                List<CodeInstruction> instructionList = instructions.ToList();

                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(LogWarning)));

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    //if (i > 15 && instruction.LoadsField(InstanceColorInfo) &&  instructionList[i-1].Calls(callEffectiveColorInfo))
                    //if (i > 300 && instruction.Calls(callEffectiveColorInfo) && instructionList[i + 1].StoresField(InstanceColorInfo) && instructionList[i - 2].LoadsField(MoteInfo))
                    if (i > 400 && i < 550 && instruction.StoresField(InstanceColorInfo) && instructionList[i - 1].Calls(callEffectiveColorInfo) && instructionList[i - 3].LoadsField(MoteInfo))
                    {
                        Log.Error(
                            "found Instance color call - " + i + "\n" +
                            "[" + (i - 3) + "]" + instructionList[i - 3].ToString() + "\n" +
                            "[" + (i - 2) + "]" + instructionList[i - 2].ToString() + "\n" +
                            "[" + (i - 1) + "]" + instructionList[i - 1].ToString() + "\n" +
                            "[" + i + "]" + instruction.ToString() + "\n" +
                            "[" + (i + 1) + "]" + instructionList[i + 1].ToString()
                        );



                        yield return instruction;

                        // does access A
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(LogWarningTarget)));
                        Log.Error(" added LogWarningTarget");

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, DefInfo);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(LogWarningSubEffecterDef)));
                        Log.Error(" added LogWarningSubEffecterDef");

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, MoteInfo);
                        yield return new CodeInstruction(OpCodes.Ldfld, InstanceColorInfo);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(LogWarningColor)));
                        Log.Error(" added LogWarningColor");

                        /*
                        // this.mote.instanceColor = GetDamageEffecterColor( A, this.def, this.mote.instanceColor)

                        // this.mote
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, MoteInfo);

                        // A
                        yield return new CodeInstruction(OpCodes.Ldarg_1);

                        //this.def
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, DefInfo);

                        //this.mote.instanceColor
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, MoteInfo);
                        yield return new CodeInstruction(OpCodes.Ldfld, InstanceColorInfo);

                        // = GetDamageEffecterColor (
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(GetDamageEffecterColor)));

                        // .instanceColor =
                        yield return new CodeInstruction(OpCodes.Stfld, InstanceColorInfo);
                        */

                    }
                    else
                    {
                        yield return instruction;
                    }

                    /*
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
                    */
                }
            }
        }
    }
}
