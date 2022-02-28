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
    public class EquipPatch
    {
        public static bool TryPatch_WeaponChanged(Harmony myPatch)
        {
            // Notify_EquipmentRemoved Notify_EquipmentAdded

            try
            {
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_DraftController), "Notify_PrimaryWeaponChanged");
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_PrimaryWeaponChanged), "Postfix_PrimaryWeaponChanged");

                myPatch.Patch(MyMethod, null, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW YAHA failed ApplyPatch_PrimaryWeaponChanged: " + e);
                return false;
            }

            return true;
        }

        static class ApplyPatch_PrimaryWeaponChanged
        {
            static void Postfix_PrimaryWeaponChanged(Pawn ___pawn)
            {

                Log.Warning("This is Notify_PrimaryWeaponChanged; p=" + ___pawn.Name );


                IEnumerable<Hediff> allYahaHediffs = ___pawn.health.hediffSet.hediffs.Where(hi => hi.TryGetComp<HediffComp_YetAnotherHediffApplier>() != null);

                if (allYahaHediffs.EnumerableNullOrEmpty())
                    return;

                YahaUtility.CheckTriggeredAssociations(allYahaHediffs, TriggerEvent.equipWeapon);
            }
        }
    }

}
