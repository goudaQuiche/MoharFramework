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
            { "Notify_ApparelAdded", new Func<Harmony, bool> (WearPatch.TryPatch_ApparelAdded) } ,
            { "Notify_ApparelRemoved", new Func<Harmony, bool> (UndressPatch.TryPatch_ApparelRemoved) } ,
            //{ "Notify_PrimaryWeaponChanged", new Func<Harmony, bool> (EquipPatch.TryPatch_WeaponChanged) } ,
        };

    }
}
