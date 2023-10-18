using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSSU
{
    public static class MyDefs
    {
        //CompProperties_SpawnSubplant
        public static IEnumerable<ThingDef> AllDefsWithCSS = DefDatabase<ThingDef>.AllDefs.Where(
            x =>
            //x.defName != "Plant_TreeAnima" &&
            x.modContentPack != null &&
            x.modContentPack.PackageId != ModContentPack.CoreModPackageId &&
            x.modContentPack.PackageId != ModContentPack.RoyaltyModPackageId &&
            x.modContentPack.PackageId != ModContentPack.IdeologyModPackageId &&
            x.modContentPack.PackageId != ModContentPack.BiotechModPackageId &&
            !x.comps.NullOrEmpty() && 
            x.comps.Any(y => y is CompProperties_SpawnSubplant)
        );

        public static bool HasDefsWithCSS => !AllDefsWithCSS.EnumerableNullOrEmpty();
        public static bool IsRoyaltyLoaded => ModLister.CheckRoyalty("Mohar CSSU");

    }
}
