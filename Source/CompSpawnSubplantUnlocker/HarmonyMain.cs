using Verse;
using HarmonyLib;

namespace CSSU
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
       //public readonly static IEnumerable<ThingDef> OnceAllDefsWithCSS = MyDefs.AllDefsWithCSS;

        static HarmonyPatchAll()
        {
            if (!MyDefs.IsRoyaltyLoaded)
            {
                if (Prefs.DevMode) Log.Message("Mohar CSSU - No royalty, nothing to be done");
                return;
            }

            /*
            IEnumerable<ThingDef> OnceAllDefsWithCSS = new List<ThingDef>();
            OnceAllDefsWithCSS = MyDefs.AllDefsWithCSS;
            */

            if(!MyDefs.HasDefsWithCSS)
            //if (OnceAllDefsWithCSS.EnumerableNullOrEmpty())
            {
                if (Prefs.DevMode) Log.Message("Mohar CSSU - Found no definition with CompSpawnSubplant, nothing to be done");
                return;
            }

            //Harmony.DEBUG = true;
            Harmony CSSU_HarmonyPatch = new Harmony("MoharFW.CompSpawnSubplantUnlocker");

            /*    
            foreach(ThingDef td in MyDefs.AllDefsWithCSS)
            {
                Log.Warning("Found " + td.defName);
            }
            */

            if (Harmony_jobDriver_Meditate.Try_jobDriver_Meditate_IEnumeratorMoveNext_Patch(CSSU_HarmonyPatch))
                Log.Message(CSSU_HarmonyPatch.Id + " patched JobDriver_Meditate.MakeNewToils successfully.");

        }
    }
}