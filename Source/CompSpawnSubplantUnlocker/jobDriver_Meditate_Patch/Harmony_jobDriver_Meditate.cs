using System.Collections.Generic;
using Verse;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

using RimWorld;


namespace CSSU
{
    public class Harmony_jobDriver_Meditate
    {
        private static readonly string TranspilePatchName = nameof(RimWorld_jobDriver_Meditate_HarmonyPatch.JobDriver_Meditate_Transpile);

        private static readonly Type patchType = typeof(RimWorld_jobDriver_Meditate_HarmonyPatch);
        private static readonly Type patchUtilsType = typeof(JobDriver_Meditate_Utils);
        private static readonly Type patchHarmonyUtilsType = typeof(Harmony_Utils);

        // RimWorld jobDriver_Meditate MakeNewToils
        public static bool Try_jobDriver_Meditate_IEnumeratorMoveNext_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(RimWorld.JobDriver_Meditate), "<MakeNewToils>b__15_3");

                if (Method == null)
                    Log.Warning("found no method");

                HarmonyMethod Transpiler = new HarmonyMethod(patchType, TranspilePatchName);
                myPatch.Patch(Method, transpiler: Transpiler);

            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.CSSU " + TranspilePatchName + " failed:\n" + e);
                return false;
            }

            //Log.Warning("Did find method");

            return true;
        }

        public static class RimWorld_jobDriver_Meditate_HarmonyPatch
        {
            public static IEnumerable<CodeInstruction> JobDriver_Meditate_Transpile(IEnumerable<CodeInstruction> instructions)
            {
                FieldInfo pawn = AccessTools.Field(typeof(Verse.AI.JobDriver), "pawn");
                MethodInfo GetPlant_methodInfo = AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetPlant), new[] { typeof(Verse.IntVec3), typeof(Verse.Map) });
                MethodInfo mapProperty = AccessTools.Property(typeof(Verse.Thing), nameof(Thing.Map)).GetGetMethod();

                List<CodeInstruction> instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    CodeInstruction instruction = instructionList[i];

                    // Plant plant = c.GetPlant(pawn.Map);
                    if (i > 10 && i < instructionList.Count - 10 &&
                        instructionList[i].IsLdloc() && instructionList[i + 1].IsLdarg(0) &&
                        instructionList[i + 2].LoadsField(pawn) && instructionList[i + 4].Calls(GetPlant_methodInfo))
                    {
                        //AddProgress(c, pawn.Map)
                        yield return new CodeInstruction(OpCodes.Ldloc_S, (object)4);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return CodeInstruction.LoadField(typeof(Verse.AI.JobDriver), "pawn");
                        yield return new CodeInstruction(OpCodes.Callvirt, mapProperty);
                        yield return CodeInstruction.Call(patchUtilsType, nameof(JobDriver_Meditate_Utils.AddProgress));
                        //yield return CodeInstruction.Call(patchUtilsType, nameof(JobDriver_Meditate_Utils.RawAddProgress));

                        /*
                        Log.Warning(instruction.ToString());
                        Log.Warning(instructionList[i + 1].ToString());
                        Log.Warning(instructionList[i + 2].ToString());
                        Log.Warning(instructionList[i + 3].ToString());
                        Log.Warning(instructionList[i + 4].ToString());
                        */
                    }

                    yield return instruction;

                    /*
                    Log.Warning(instruction.ToString());
                    yield return instruction;
                    */
                }

                //Log.Error(errLog);
            }

        }
    }
}
