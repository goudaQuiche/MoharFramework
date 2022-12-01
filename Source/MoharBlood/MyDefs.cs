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
        
        public static IEnumerable<BloodColorSet> AllBloodColorSetWithJobMote =
            AllBloodColorDefs.Where(a => a.bloodSetList.Any(x => x.HasJobMote)).SelectMany(b => b.bloodSetList);

        public static IEnumerable<JobMote> AllJobMote = AllBloodColorSetWithJobMote.SelectMany(a => a.jobMote);
        public static IEnumerable<EffecterDef> AllJobMoteEffecterDef = AllJobMote.Select(a => a.effectWorking).Distinct();

        public static bool HasFleshTypeWound = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasFleshTypeWound));
        public static bool HasDamageEffecter = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasDamageEffecter));
        public static bool HasJobMote = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasJobMote));
        public static bool HasHealthTabBleeding = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasHealthTabBleeding));
        public static bool HasBloodFilth = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasBloodFilth));
        public static bool HasDamageFlash = !HasBloodSet ? false : AllBloodColorDefs.Any(x => x.bloodSetList.Any(y => y.HasDamageFlash));

        public static ThingDef HumanBlood = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_Blood").FirstOrFallback();
        public static ThingDef InsectBlood = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_BloodInsect").FirstOrFallback();

        public static Color HumanBloodColor = HumanBlood?.graphicData.color ?? Color.white;
        public static Color InsectBloodColor = InsectBlood?.graphicData.color ?? Color.white;

        /*
        [DefOf]
        public EffecterDefOf.
        */
    }


}
