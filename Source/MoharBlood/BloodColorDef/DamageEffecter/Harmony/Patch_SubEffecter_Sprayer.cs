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
        private static readonly string patchName = nameof(Verse_SubEffecter_Sprayer_MakeMote_HarmonyPatch.MakeMote_Transpile);
        private static readonly Type patchType = typeof(Verse_SubEffecter_Sprayer_MakeMote_HarmonyPatch);
        private static readonly Type patchUtilsType = typeof(SubEffecter_Sprayer_Utils);

        // Verse SubEffecter_Sprayer
        public static bool Try_SubEffecter_Sprayer_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(Verse.SubEffecter_Sprayer), "MakeMote");
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
            public static IEnumerable<CodeInstruction> MakeMote_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo callEffectiveColorInfo = AccessTools.Method(typeof(SubEffecter), "get_EffectiveColor");
                MethodInfo callMakeThingInfo = AccessTools.Method(typeof(ThingMaker), nameof(ThingMaker.MakeThing));
                FieldInfo rotationInfo = AccessTools.Field(typeof(FleckCreationData), "rotation");
                FieldInfo DefInfo = AccessTools.Field(typeof(SubEffecter), "def");
                FieldInfo moteDefInfo = AccessTools.Field(typeof(SubEffecterDef), "moteDef");

                FieldInfo InstanceColorInfo = AccessTools.Field(typeof(Verse.Mote), nameof(Mote.instanceColor));
                FieldInfo MoteInfo = AccessTools.Field(typeof(SubEffecter_Sprayer), "mote");

                List<CodeInstruction> instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    // effectWorking uses motes
                    // this.mote = (Mote) ThingMaker.MakeThing(this.def.moteDef);
                    // replacing this.def.moteDef
                    // with SubEffecter_Sprayer_Utils.GetJobMote(A, this)
                    if (MyDefs.HasJobMote && instruction.IsLdarg(0) && instructionList[i - 1].IsLdarg(0)
                        && instructionList[i + 1].LoadsField(DefInfo) && instructionList[i + 2].LoadsField(moteDefInfo)
                        && instructionList[i + 4].Calls(callMakeThingInfo)
                        && instructionList[i + 6].StoresField(MoteInfo)
                        )
                    {
                        /*
                        Log.Error("found this.mote = (Mote) ThingMaker.MakeThing(this.def.moteDef)");
                        Harmony_Utils.LogAround(instructionList, i, -1, 6);
                        */

                        //GetJobMoteReplacement( A, B, this)
                        // A
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        // B
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        //this
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        // = GetJobMote ( ... )
                        yield return CodeInstruction.Call(patchUtilsType, nameof(SubEffecter_Sprayer_Utils.GetJobMoteReplacement));

                        // skipping this.def.moteDef ; [i] = this ; [i+1] = def ; [i+2] = moteDef 
                        i += 2;
                    }
                    // effectWorking uses motes
                    // this.mote.instanceColor = this.EffectiveColor;
                    // replacing this.EffectiveColor
                    //else if (MyDefs.HasJobMote && instruction.StoresField(InstanceColorInfo) && instructionList[i - 1].Calls(callEffectiveColorInfo) && instructionList[i - 3].LoadsField(MoteInfo))
                    else if (MyDefs.HasJobMote 
                        && instruction.IsLdarg(0) && instructionList[i + 1].Calls(callEffectiveColorInfo)
                        && instructionList[i + 2].StoresField(InstanceColorInfo) && instructionList[i - 1].LoadsField(MoteInfo))
                    {
                        //Log.Error("found this.mote.instanceColor = this.EffectiveColor ");
                        //LogAround(instructionList, i, -3, 3);

                        //GetJobMoteColor( A, B, this)
                        // A
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        // B
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        //this
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        // = GetJobMoteColor ( ... )
                        yield return CodeInstruction.Call(patchUtilsType, nameof(SubEffecter_Sprayer_Utils.GetJobMoteColor));

                        // skipping this.EffectiveColor ; [i] = this ; [i+1] = EffectiveColor 
                        i += 1;
                    }
                    // DamageEffecter uses flecks 
                    // instanceColor = new Color?(this.EffectiveColor),
                    // replacing this.EffectiveColor
                    else if (MyDefs.HasDamageEffecter && instruction.IsLdarg(0) && instructionList[i + 1].Calls(callEffectiveColorInfo) && instructionList[i - 2].StoresField(rotationInfo))
                    {
                        //GetDamageEffecterColor( A, this)
                        // A
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        //this
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        // = GetDamageEffecterColor (
                        yield return CodeInstruction.Call(patchUtilsType, nameof(SubEffecter_Sprayer_Utils.GetDamageEffecterColor));

                        // skipping this.EffectiveColor ; this = the line; EffectiveColor = next line
                        i += 1;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}
