using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using System;

namespace OHPG
{
    public static class MyDefs
    {
        public static HediffDef OHPG_HediffDef = DefDatabase<HediffDef>.AllDefs.Where((HediffDef h) => h.defName == "OneHediffPerGender").First();

    }
}
