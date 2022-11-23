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
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            Harmony MoharBlood_HarmonyPatch = new Harmony("MoharFW.MoharBlood");

            // Check if any def requiring patching
            if (!MyDefs.HasBloodSet)
            {
                //Log.Warning("No bloodset found");
                return;
            }
                

            if (MyDefs.HasFleshTypeWound)
            {
                if (Harmony_FleshTypeDef.Try_FleshTypeDef_ResolveWound_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched FleshTypeDef.ChooseWoundOverlay successfully.");
            }
            else if(MyDefs.HasDebug) Log.Warning("No fleshTypeWound found");

            if (MyDefs.HasDamageEffecter || MyDefs.HasJobMote)
            {
                if (Harmony_Sprayer_MakeMote.Try_SubEffecter_Sprayer_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched SubEffecter_Sprayer.MakeMote successfully.");
                
                /*
                if (Harmony_DamageWorker_AddInjury_ApplyToPawn.Try_DamageWorker_AddInjury_ApplyToPawn_Transpiler_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched DamageWorker_AddInjury.ApplyToPawn successfully.");
                    */
            }
            else if (MyDefs.HasDebug) Log.Warning("No DamageEffecter or JobMote found");
        }
    }
}