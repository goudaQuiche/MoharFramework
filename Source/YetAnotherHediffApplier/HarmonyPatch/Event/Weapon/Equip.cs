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
        public static bool TryPatch_WeaponEquiped(Harmony myPatch)
        {
            try
            {
                //MethodBase MyMethod = AccessTools.Method(typeof(Pawn_DraftController), "Notify_PrimaryWeaponChanged");
                MethodBase MyMethod = AccessTools.Method(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded");
                //HarmonyMethod Postfix = new HarmonyMethod(typeof(ApplyPatch_PrimaryWeaponChanged), "Postfix_PrimaryWeaponChanged");
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
            static void Postfix_PrimaryWeaponChanged(Pawn ___pawn, ThingWithComps eq)
            {
                Log.Warning("This is Notify_PrimaryWeaponChanged; p=" + ___pawn.Name );

                if (eq.def.equipmentType != EquipmentType.Primary)
                    return;

                YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.weapon);
            }
        }
    }

}
