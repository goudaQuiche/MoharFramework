using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using System;

namespace OHPLS
{
    public static class MyDefs
    {
        public static HediffDef OHPLS_HediffDef = DefDatabase<HediffDef>.AllDefs.Where((HediffDef h) => h.defName == "OneHediffPerLifeStage").First();

    }
}
