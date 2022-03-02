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
    public class RemovePatch
    {
        public static bool TryPatch_WeaponRemoved(Harmony myPatch)
        {
            try
            {
                //MethodBase MyMethod = AccessTools.Method(typeof(Pawn_DraftController), "Notify_PrimaryWeaponChanged");
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved");
                //HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_PrimaryWeaponChanged), "Postfix_PrimaryWeaponChanged");
                HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_PrimaryWeaponRemoved), "Postfix_Notify_EquipmentRemoved");

                myPatch.Patch(MyMethod, null, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MFW YAHA failed ApplyPatch_PrimaryWeaponChanged: " + e);
                return false;
            }

            return true;
        }

        static class ApplyPatch_PrimaryWeaponRemoved
        {
            static void Postfix_Notify_EquipmentRemoved(Pawn ___pawn, ThingWithComps eq)
            {
                Log.Warning("This is Notify_EquipmentRemoved; p=" + ___pawn.Name );

                if (eq.def.equipmentType != EquipmentType.Primary)
                    return;

                YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.weapon);
            }
        }
    }

}
