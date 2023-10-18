using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace CSSU
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            if (!MyDefs.IsRoyaltyLoaded)
            {
                Log.Warning("Mohar CSSU - No royalty, nothing to be done");
                return;
            }
            
            if(!MyDefs.HasDefsWithCSS)
            {
                Log.Warning("Mohar CSSU - Found no definition with CompSpawnSubplant, nothing to be done");
                return;
            }

            //Harmony.DEBUG = true;
            Harmony CSSU_HarmonyPatch = new Harmony("MoharFW.CompSpawnSubplantUnlocker");

                
            foreach(ThingDef td in MyDefs.AllDefsWithCSS)
            {
                Log.Warning("Found " + td.defName);
            }

            /*
            if (MyDefs.HasFleshTypeWound)
            {
                if (Harmony_FleshTypeDef.Try_FleshTypeDef_ResolveWound_Patch(MoharBlood_HarmonyPatch))
                    Log.Message(MoharBlood_HarmonyPatch.Id + " patched FleshTypeDef.ChooseWoundOverlay successfully.");
            }
            else if(MyDefs.HasDebug) Log.Warning("No fleshTypeWound found");
            */
        }
    }
}