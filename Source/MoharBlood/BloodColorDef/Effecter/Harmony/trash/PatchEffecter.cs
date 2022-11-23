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
    public class Harmony_VerseEffecterTrigger
    {
        // Verse.Effecter.Trigger
        public static bool Try_VerseEffecterTrigger_Patch(Harmony myPatch)
        {
            string patchName = nameof(VerseEffecterTrigger_HarmonyPatch.VerseEffecterTrigger_Prefix);

            try
            {
                MethodBase Method = AccessTools.Method(typeof(Effecter), "Trigger");
                HarmonyMethod Prefix = new HarmonyMethod(typeof(VerseEffecterTrigger_HarmonyPatch), patchName);
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

        public static class VerseEffecterTrigger_HarmonyPatch
        {

            public static bool VerseEffecterTrigger_Prefix(Effecter __instance, TargetInfo A, TargetInfo B)
            {
                /*
                Pawn p = (Pawn)A;

                foreach(SubEffecter eff in __instance.children)
                {
                    eff.colorOverride = Color.blue;
                }
                */

                Log.Warning("VerseEffecterTrigger_Prefix");
                //Log.Warning(p?.LabelShort + "VerseEffecterTrigger_Prefix");

                return true;
            }
        }
    }

}