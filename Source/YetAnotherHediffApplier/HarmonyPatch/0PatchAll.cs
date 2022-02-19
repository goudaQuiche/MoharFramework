using Verse;
using HarmonyLib;
using System.Reflection;

namespace YAHA
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            
            Harmony YAHA_HarmonyPatch = new Harmony("goudaQuiche.MoharFramework.YAHA");
            if (DraftPatch.TryPatch_ClearQueuedJobs(YAHA_HarmonyPatch)) Log.Message("MoharFW YAHA - ClearWorkPatch applied");

            //if (DraftPatch.TryDraftPatch(YAHA_HarmonyPatch)) Log.Message("MoharFW YAHA - DraftPatch applied");

            //YAHA_HarmonyPatch.PatchAll();
        }
    }
    
    /*
    [StaticConstructorOnStartup]
    public class YAHA_Patch
    {
        public static bool TryPatchAll(Harmony myPatch)
        {

            string LogName = "Yet another hediff applier - YAHA - harmony patch ";
            bool MyDebug = true;

            DraftPatch.DraftedPostfix();

            return true;
        }
    }
    */
}
