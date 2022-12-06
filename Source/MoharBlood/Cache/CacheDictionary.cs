using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoharBlood
{
    public static class StaticCollections
    {
        public static Dictionary<Thing, MappedDamageFlash> DamageFlashCache = new Dictionary<Thing, MappedDamageFlash>();
        public static Dictionary<int, MappedHealthCard> HealthCardCache = new Dictionary<int, MappedHealthCard>();
    }
}
