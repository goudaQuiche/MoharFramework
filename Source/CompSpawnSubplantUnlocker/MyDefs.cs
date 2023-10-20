using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace CSSU
{
    public static class MyDefs
    {
        //CompProperties_SpawnSubplant
        
        public static IEnumerable<ThingDef> AllDefsWithCSS = DefDatabase<ThingDef>.AllDefs.Where(
            x =>
            x.modContentPack != null &&
            x.modContentPack.PackageId != ModContentPack.CoreModPackageId &&
            x.modContentPack.PackageId != ModContentPack.RoyaltyModPackageId &&
            x.modContentPack.PackageId != ModContentPack.IdeologyModPackageId &&
            x.modContentPack.PackageId != ModContentPack.BiotechModPackageId &&
            !x.comps.NullOrEmpty() && 
            x.comps.Any(y => y is CompProperties_SpawnSubplant)
        );
        
        //public static IEnumerable<ThingDef> SimpleAllDefsWithCSS = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName == "LTF_Crystal_Cairn");

        public static bool HasDefsWithCSS => !AllDefsWithCSS.EnumerableNullOrEmpty();
        public static bool IsRoyaltyLoaded => ModLister.CheckRoyalty("Mohar CSSU");

    }
    
    [StaticConstructorOnStartup]
    static class StaticDef
    {
        public static IEnumerable<ThingDef> AllDefs = new List<ThingDef>();
        static StaticDef()
        {
            AllDefs = MyDefs.AllDefsWithCSS.ToList();
        }
    }

}
