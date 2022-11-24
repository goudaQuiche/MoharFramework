using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoharBlood
{
    [DefOf]
    public static class MyEffecterDefOf
    {
        public static EffecterDef Surgery;
        public static EffecterDef ButcherFlesh;

        static MyEffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MyEffecterDefOf));
        }
    }


}
