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

            //string PatchReport = ReportDump(successStr, failureStr);
            string PatchReport = SmallReport(successStr, failureStr);

            Log.Message(PatchReport);
        }

        static string SmallReport(List<string> successStr, List<string> failureStr)
        {
            string PatchReport = "MoharFW YAHA - ";

            if (!successStr.NullOrEmpty())
            {
                PatchReport += "successfuly completed "+successStr.Count+" harmony patches. ";
            }
            if (!failureStr.NullOrEmpty())
            {
                PatchReport += "Patch failures:" + failureStr.Count + ".";
            }
            return PatchReport;
        }

        static string ReportDump(List<string> successStr, List<string> failureStr)
        {
            string PatchReport = "MoharFW YAHA - ";

            if (!successStr.NullOrEmpty())
            {
                PatchReport += "Patch success:";
                for (int i = 0; i < successStr.Count; i++)
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

            return PatchReport;
        }
    }
}
