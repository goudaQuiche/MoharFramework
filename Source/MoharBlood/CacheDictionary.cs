using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoharBlood
{
    public static class CacheDictionary
    {
        // use PAwn spawn/despawn postfix to create/destroy cache ?

        public static IDictionary<Thing, Color> BodyWoundColorCache = new Dictionary<Thing, Color>();
        public static TwoKeyDictionary<Thing, FleckDef, Color> DamageEffectColorCache = new TwoKeyDictionary<Thing, FleckDef, Color>();
        

        public static void AddBodyWoundColorToDict(Thing thing, Color color)
        {

            if (BodyWoundColorCache.ContainsKey(thing))
                return;

            BodyWoundColorCache[thing] = color;
        }

        public class TwoKeyDictionary<K1, K2, T> : Dictionary<K1, Dictionary<K2, T>> { }
    }
}
