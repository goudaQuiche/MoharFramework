using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoharBlood
{
    public static class CacheDictionary
    {
        public static IDictionary<Thing, ThingDef> bloodtype_gene_pawns = new Dictionary<Thing, ThingDef>();

        public static void AddBloodtypeGenePawnToList(Thing thing, ThingDef thingDef)
        {

            if (!bloodtype_gene_pawns.ContainsKey(thing))
            {
                bloodtype_gene_pawns[thing] = thingDef;
            }
        }

    }


}
