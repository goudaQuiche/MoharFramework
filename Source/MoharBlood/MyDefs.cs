using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoharBlood
{
    public static class MyDefs
    {

        public static IEnumerable<MoharBlood.BloodColorDef> AllBloodColorDefs = DefDatabase<MoharBlood.BloodColorDef>.AllDefs;
        public static bool HasBloodColor => !AllBloodColorDefs.EnumerableNullOrEmpty();

        public static bool HasDebug => HasBloodColor && AllBloodColorDefs.Any(x => x.debug);

        public static bool HasBloodSet => HasBloodColor && AllBloodColorDefs.Any(x => !x.bloodSetList.EnumerableNullOrEmpty());

        public static bool HasFleshTypeWound = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasFleshTypeWound));
        public static bool HasDamageEffecter = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasDamageEffecter));
        public static bool HasJobMote = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasJobMote));

        public static ThingDef HumanBlood = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_Blood").FirstOrFallback();

        public static Color HumanBloodColor = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_Blood").FirstOrFallback()?.graphicData.color ?? Color.white;
        public static Color InsectBloodColor = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_BloodInsect").FirstOrFallback()?.graphicData.color ?? Color.white;
    }


}
