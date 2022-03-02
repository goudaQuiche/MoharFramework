using Verse;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace YAHA
{
    public static class PatchDictionary
    {
        readonly public static Dictionary<string, Func<Harmony, bool>> harmonyDict = new Dictionary<string, Func<Harmony, bool>>()
        {
            { "ClearQueuedJobs", new Func<Harmony, bool> (DraftPatch.TryPatch_ClearQueuedJobs) },

            { "Notify_ApparelAdded", new Func<Harmony, bool> (WearPatch.TryPatch_ApparelWorn) } ,
            { "Notify_ApparelRemoved", new Func<Harmony, bool> (UndressPatch.TryPatch_ApparelUndressed) } ,

            { "Notify_EquipmentAdded", new Func<Harmony, bool> (EquipPatch.TryPatch_WeaponEquiped) } ,
            { "Notify_EquipmentRemoved", new Func<Harmony, bool> (RemovePatch.TryPatch_WeaponRemoved) } ,

            { "PostAdd", new Func<Harmony, bool> (HediffAddedPatch.TryPatch_HediffAdded) } ,
            { "PostRemove", new Func<Harmony, bool> (HediffRemovedPatch.TryPatch_HediffRemoved) } ,
        };

    }
}
