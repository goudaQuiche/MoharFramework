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
            //Harmony.DEBUG = true;
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

            if (MyDefs.HasHealthTabBleeding)
            {
                if (Harmony_HealthCardUtility_DrawHediffRow.Try_HealthCardUtility_DrawHediffRow_Prefix(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched HealthCardUtility.DrawHediffRow.Prefix successfully.");

                if (Harmony_HealthCardUtility_DrawHediffRow.Try_HealthCardUtility_NestedDrawHediffRow_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched HealthCardUtility.Nested.DrawHediffRow successfully.");
                
            }
            else if (MyDefs.HasDebug) Log.Warning("No HealthTabBleeding found");

            if (MyDefs.HasBloodFilth)
            {
                if (Harmony_Patch_Filth_AddSources.Try_Filth_AddSources_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched Filth.AddSources successfully.");

                if(HarmonyPatch_Verse_Graphic.Try_GraphicPrint_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched Graphic.Print successfully.");

            }
            else if (MyDefs.HasDebug) Log.Warning("No BloodFilth found");

            if (MyDefs.HasDamageFlash)
            {
                if (Harmony_DamageFlash.Try_OverrideMaterialIfNeeded_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched OverrideMaterialIfNeeded successfully.");

                if (Harmony_DamageFlash.Try_HeadMatAt_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched HeadMatAt successfully.");

                if (Harmony_DamageFlash.Try_DamagedMatPool_GetDamageFlashMat_Transpile(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched GetDamageFlashMat successfully.");
            }
            else if (MyDefs.HasDebug) Log.Warning("No damage flash found");


        }
    }
}