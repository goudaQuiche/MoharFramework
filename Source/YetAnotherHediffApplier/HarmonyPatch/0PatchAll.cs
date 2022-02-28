using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace YAHA
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            
            Harmony YAHA_HarmonyPatch = new Harmony("goudaQuiche.MoharFramework.YAHA");
            List<string> successStr = new List<string>();
            List<string> failureStr = new List<string>();

            foreach(KeyValuePair<string, Func<Harmony, bool>> entry in PatchDictionary.harmonyDict)
            {
                if (entry.Value(YAHA_HarmonyPatch))
                    successStr.Add(entry.Key);
                else
                    failureStr.Add(entry.Key);
            }

            string PatchReport = "MoharFW YAHA - ";
            if (!successStr.NullOrEmpty())
            {
                PatchReport += "Patch success:";
                for (int i = 0; i < successStr.Count; i ++)
                {
                    PatchReport += ((i == 0) ? "" : "/") + successStr[i];
                }
                PatchReport += "; ";
            }
            if (!failureStr.NullOrEmpty())
            {
                PatchReport += "Patch failure:";
                for (int i = 0; i < failureStr.Count; i++)
                {
                    PatchReport += failureStr[i] + ((i == 0) ? "" : "/");
                }
                PatchReport += ";";
            }

            Log.Message(PatchReport);
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
