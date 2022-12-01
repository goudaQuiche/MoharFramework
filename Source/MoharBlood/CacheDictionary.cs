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
        // use PAwn spawn/despawn postfix to create/destroy cache ?
        static Color BugColor = ColoringWayUtils.bugColor;


        public static Dictionary<Thing, MappedDamageFlash> DamageFlashCache = new Dictionary<Thing, MappedDamageFlash>();
        public static Dictionary<Thing, MappedHealthCard> HealthCardCache = new Dictionary<Thing, MappedHealthCard>();


        //DamageFlash
        public static bool DamageFlashGetCache(Thing thing, out bool isEligible, out Color color)
        {
            isEligible = false;
            color = BugColor;

            if (DamageFlashCache.TryGetValue(thing) is MappedDamageFlash mdf)
            {
                isEligible = mdf.isEligible;
                color = mdf.color;
                return true;
            }

            return false;
        }

        public static void DamageFlashAddCache(Thing thing, bool new_isEligible, Color new_color)
        {
            if (DamageFlashCache.ContainsKey(thing))
                return;

            DamageFlashCache[thing] = new MappedDamageFlash
            {
                isEligible = new_isEligible,
                color = new_color,
            };
        }

        public static void DamageFlashAddIneligibleCache(Thing thing)
        {
            DamageFlashAddCache(thing, false, BugColor);
        }

        public static void DamageFlashAddEligibleCache(Thing thing, Color new_color)
        {
            DamageFlashAddCache(thing, true, new_color);
        }


        // HealthCard
        public static bool HealthCardGetCache(Thing thing, out bool isEligible, out Texture texture, out Material material)
        {
            isEligible = false;
            texture = null;
            material = null;

            if (HealthCardCache.TryGetValue(thing) is MappedHealthCard mhc)
            {
                isEligible = mhc.isEligible;
                texture = mhc.texture;
                material = mhc.material;
                return true;
            }

            return false;
        }

        public static void HealthCardAddCache(Thing thing, bool new_isEligible, Texture new_texture, Material new_material)
        {
            if (HealthCardCache.ContainsKey(thing))
                return;

            HealthCardCache[thing] = new MappedHealthCard
            {
                isEligible = new_isEligible,
                texture = new_texture,
                material = new_material
            };
        }

        public static void HealthCardAddIneligibleCache(Thing thing)
        {
            HealthCardAddCache(thing, false, null, null);
        }

        public static void HealthCardAddCacheAddEligibleCache(Thing thing, Texture new_texture, Material new_material)
        {
            HealthCardAddCache(thing, true, new_texture, new_material);
        }

    }

    public class MappedDamageFlash
    {
        public Color color { get; set; }
        public bool isEligible { get; set; }
    }

    public class MappedHealthCard
    {
        public bool isEligible { get; set; }
        public Texture texture { get; set; }
        public Material material{ get; set; }
    }
}
