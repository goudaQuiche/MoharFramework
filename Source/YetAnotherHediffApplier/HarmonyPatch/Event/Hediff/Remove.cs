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
    public class HediffRemovedPatch
    {
        public static bool TryPatch_HediffRemoved(Harmony myPatch)
        {
            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(Hediff), "PostRemoved");
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_HediffRemoved), "Postfix_PostRemoved");

                myPatch.Patch(MyMethod, null, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW YAHA failed ApplyPatch_HediffRemoved: " + e);
                return false;
            }

            return true;
        }

        static class ApplyPatch_HediffRemoved
        {
            static void Postfix_PostRemoved(Pawn ___pawn)
            {
                Log.Warning("This is PostRemoved; p=" + ___pawn.Name );

                YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.hediff);
            }
        }
    }

}
