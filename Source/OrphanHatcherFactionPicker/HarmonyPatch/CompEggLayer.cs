using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
using System.Reflection;
using HarmonyLib;


namespace OHFP
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            Harmony OHFP_HarmonyPatch = new Harmony("goudaQuiche.MoharOHFP");

            if (CompEggLayer_Patch.TryCompEggLayerPatch(OHFP_HarmonyPatch))
                Log.Message("MoharFW OHFP - CompEggLayer_Patch applied");
        }
    }

    [StaticConstructorOnStartup]
    public class CompEggLayer_Patch
    {
        public static bool TryCompEggLayerPatch(Harmony myPatch)
        {
            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(RimWorld.CompEggLayer), "ProduceEgg");
                HarmonyMethod Prefix = null;
                HarmonyMethod Postfix = new HarmonyMethod(typeof(AddOHFPToCompEggLayer_Patch), "Postfix");

                myPatch.Patch(MyMethod, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW OHFP failed TryCompEggLayerPatch" + e);
                return false;
            }

            return true;
        }

        static class AddOHFPToCompEggLayer_Patch
        {
            static Thing Postfix(Thing __result, Pawn ___fertilizedBy, CompEggLayer __instance)
            {
                if( __result.TryGetComp<Comp_OHFP_Hatcher>() is Comp_OHFP_Hatcher comp )
                {
                    comp.hatcheeFaction = __instance.parent.Faction;
                    if(__instance.parent is Pawn pawn){
                        comp.hatcheeParent = pawn;
                    }
                    if (___fertilizedBy != null)
                    {
                        comp.otherParent = ___fertilizedBy;
                    }
                }

                return __result;
            }
        }

    }
}