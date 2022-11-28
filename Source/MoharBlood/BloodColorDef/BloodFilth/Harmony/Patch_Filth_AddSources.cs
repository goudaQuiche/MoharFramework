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
    public class Harmony_Patch_Filth_AddSources
    {
        private static readonly string patchName = nameof(RimWorld_Filth_AddSources_HarmonyPatch.AddSources_Postfix);

        private static readonly Type patchType = typeof(RimWorld_Filth_AddSources_HarmonyPatch);
        private static readonly Type patchUtilsType = typeof(Filth_AddSources_Utils);
        private static readonly Type patchHarmonyUtilsType = typeof(Harmony_Utils);

        // RimWorld HealthCardUtility DrawHediffRow
        public static bool Try_Filth_AddSources_Patch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(RimWorld.Filth), "AddSources");

                HarmonyMethod Prefix = null;
                HarmonyMethod Postfix = new HarmonyMethod(patchType, "AddSources_Postfix");
                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharBlood " + patchName + " failed  - " + e);
                return false;
            }
            return true;
        }

        public static class RimWorld_Filth_AddSources_HarmonyPatch
        {
            public static void AddSources_Postfix(IEnumerable<string> sources, Filth __instance)
            {
                //Log.Warning("AddSources_Postfix");
                if(__instance.def.thingClass == typeof(ColorableFilth))
                {
                    //Log.Warning("Found colorable Filth");
                    Filth_AddSources_Utils.GetBloodfilth(sources, (ColorableFilth)__instance, true);
                    //Filth_AddSources_Utils.GetBloodfilth(sources, (ColorableFilth)__instance);
                }
            }

        }
    }
}
