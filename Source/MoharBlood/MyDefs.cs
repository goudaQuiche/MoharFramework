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
        //public static IEnumerable<EffecterDef> e = DefDatabase<EffecterDef>.AllDefs.Where(e => e.GetModExtension<PawnColoredEffecterExtension>() != null);
        //public static bool EffecterWithExtension = DefDatabase<EffecterDef>.AllDefs.Any(e => e.HasModExtension<PawnColoredEffecterExtension>());
        //public static bool = 
        //public static Color 
        public static ThingDef HumanBlood = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_Blood").FirstOrFallback();

        public static Color HumanBloodColor = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_Blood").FirstOrFallback()?.graphicData.color ?? Color.white;
        public static Color InsectBloodColor = DefDatabase<ThingDef>.AllDefs.Where(t => t.defName == "Filth_BloodInsect").FirstOrFallback()?.graphicData.color ?? Color.white;
    }


}
