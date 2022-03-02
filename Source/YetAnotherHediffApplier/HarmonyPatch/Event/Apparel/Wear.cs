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
    public class WearPatch
    {
        public static bool TryPatch_ApparelWorn(Harmony myPatch)
        {
            //MethodInfo methodInfo = AccessTools.Method(typeof(Pawn_ApparelTracker), "ApparelChanged", null, null);

            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded", new[] { typeof(Apparel) });
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_ApparelAdded), "Postfix_Notify_ApparelAdded");

                myPatch.Patch(MyMethod, null, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW YAHA failed TryPatch_ApparelAdded: " + e);
                return false;
            }

            return true;
        }

        static class ApplyPatch_ApparelAdded
        {
            static void Postfix_Notify_ApparelAdded(Pawn_ApparelTracker __instance)
            {
                if (!(__instance.pawn is Pawn p))
                    return;
                
                Log.Warning("This is Notify_ApparelAdded; p=" + p.Name );

                YahaUtility.UpdateDependingOnTriggerEvent(p, TriggerEvent.apparel);
            }
        }
    }

}
