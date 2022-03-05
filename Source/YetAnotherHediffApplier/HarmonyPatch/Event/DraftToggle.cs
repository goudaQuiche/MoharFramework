using Verse;
using Verse.AI;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace YAHA
{
    [StaticConstructorOnStartup]
    public class DraftPatch
    {
        public static bool TryPatch_ClearQueuedJobs(Harmony myPatch)
        {
            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_JobTracker), "ClearQueuedJobs");
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_ClearQueuedJobs), "Postfix_ClearQueuedJobs");

                myPatch.Patch(MyMethod, null, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW YAHA failed ApplyPatch_ClearQueuedJobs" + e);
                return false;
            }

            return true;
        }

        static class ApplyPatch_ClearQueuedJobs
        {
            static void Postfix_ClearQueuedJobs(bool canReturnToPool, Pawn ___pawn)
            {
                if (!canReturnToPool)
                    return;
                //Log.Warning("This is ClearQueuedJobs; p=" + ___pawn.Name + "; Drafted:" + ___pawn.Drafted);

                YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.draft);
            }
        }
    } 
        
}
