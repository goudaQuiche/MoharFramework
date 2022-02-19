using Verse;
using Verse.AI;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace YAHA
{
    [StaticConstructorOnStartup]
    public class DraftPatch
    {
        //private static readonly MethodInfo GetPrivatePawn = AccessTools.Method(type: typeof(Pawn), name: "pawn");
        
        public static bool TryPatch_ClearQueuedJobs(Harmony myPatch)
        {
            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_JobTracker), "ClearQueuedJobs");

                //MethodBase MyMethod = AccessTools.Method(typeof(Pawn_DraftController), "Drafted", new Type[] { MethodType.Setter });
                HarmonyMethod Prefix = null;
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_ClearQueuedJobs), "Postfix_ClearQueuedJobs");

                myPatch.Patch(MyMethod, Prefix, Postfix);
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
                    

                Log.Warning("This is ClearQueuedJobs_Postfix; p=" + ___pawn.Name + "; Drafted:" + ___pawn.Drafted );



                //Pawn myPawn = (Pawn)typeof(PriorityWork).GetField("pawn", BindingFlags.NonPublic).GetValue(typeof(PriorityWork));
                //Pawn myPawn = GetPrivatePawn.Invoke(obj: null, parameters: new object[] { yourDef, typeof(yourDefType) });

                IEnumerable<Hediff> allYahaHediffs = ___pawn.health.hediffSet.hediffs.Where(hi => hi.TryGetComp<HediffComp_YetAnotherHediffApplier>() != null);

                if (allYahaHediffs.EnumerableNullOrEmpty())
                    return;

                foreach(Hediff h in allYahaHediffs)
                {
                    HediffComp_YetAnotherHediffApplier YahaComp = h.TryGetComp<HediffComp_YetAnotherHediffApplier>();
                    bool MyDebug = YahaComp.Props.debug;
                    if (MyDebug)
                        Log.Warning("Found " + h.def.defName + " Yaha hediff");

                    IEnumerable<int> indexes = YahaComp.GetTriggeredHediffAssociationIndex(TriggerEvent.draft);
                    if (indexes.EnumerableNullOrEmpty())
                        return;

                    foreach(int i in indexes)
                    {
                        HediffAssociation CurHA = YahaComp.Props.associations[i];
                        AssociatedHediffHistory CurAHH = YahaComp.Registry[i];

                        YahaComp.UpdateHediffDependingOnConditionsItem(CurHA, CurAHH, false, MyDebug);
                    }
                }
            }
        }
        
        /*
         * // Not applied
        [HarmonyPatch(typeof(PriorityWork), "ClearPrioritizedWorkAndJobQueue")]
        public static void DraftedPostfix(PriorityWork __instance)
        {

            Pawn MyPawn = (Pawn)typeof(PriorityWork).GetField("pawn", BindingFlags.NonPublic).GetValue(typeof(PriorityWork));

            if (MyPawn == null)
                Log.Warning("null pawn");
            else
            {
                bool drafted = MyPawn.Drafted;
                Log.Warning(MyPawn.Name + " is " + (drafted ? "" : "un") + "drafted");
            }
        }
        */
        /*
        [HarmonyPatch(typeof(Pawn_DraftController), "Drafted", MethodType.Setter)]
        public static void DraftedPostfix(Pawn_DraftController __instance)
        {
            bool drafted = __instance.Drafted;
            Pawn pawn = __instance.pawn;

            Log.Warning(pawn.Name + " is " + (drafted ? "" : "un") + "drafted");

        }
        */

        /*
        // Null method
        public static bool TryDraftPatch(Harmony myPatch)
        {
            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_DraftController), "Drafted");
                
                //MethodBase MyMethod = AccessTools.Method(typeof(Pawn_DraftController), "Drafted", new Type[] { MethodType.Setter });
                HarmonyMethod Prefix = null;
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyDraftPatch), "DraftedPostfix");

                myPatch.Patch(MyMethod, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW YAHA failed TryDraftPatch" + e);
                return false;
            }

            return true;
        }

        static class ApplyDraftPatch
        {
            static void DraftedPostfix(Pawn_DraftController __instance)
            {
                bool drafted = __instance.Drafted;
                Pawn pawn = __instance.pawn;

                Log.Warning(pawn.Name + " is " + (drafted ? "" : "un") + "drafted");
            }
        }
        */

    }
    
        /*
        [HarmonyPatch(typeof(Pawn_DraftController))]
        [HarmonyPatch("Drafted", MethodType.Setter)]
        public static class DraftPatch
        {
            public static void DraftedPostfix(Pawn_DraftController __instance)
            {
                bool drafted = __instance.Drafted;
                Pawn pawn = __instance.pawn;

                Log.Warning( pawn.Name + " is "+ (drafted ? "" : "un") + "drafted");

            }
        }
        */
}
