using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoharBlood
{
    public static class CachedHealthCard
    {
        // HealthCard
        public static bool GetCache(int thingIdNum, out bool isEligible, out Texture texture, out Material material)
        {
            isEligible = false;
            texture = null;
            material = null;

            if (StaticCollections.HealthCardCache.TryGetValue(thingIdNum) is MappedHealthCard mhc)
            {
                isEligible = mhc.isEligible;
                texture = mhc.texture;
                material = mhc.material;
                if (isEligible)
                    material.color = mhc.color;

                return true;
            }

            return false;
        }

        public static void AddCache(int thingIdNum, bool new_isEligible, Texture new_texture, Material new_material, Color new_color)
        {
            if (StaticCollections.HealthCardCache.ContainsKey(thingIdNum))
                return;

            StaticCollections.HealthCardCache[thingIdNum] = new MappedHealthCard
            {
                isEligible = new_isEligible,
                texture = new_texture,
                material = new_material,
                color = new_color
            };
        }

        public static void AddIneligibleCache(int thingIdNum)
        {
            AddCache(thingIdNum, false, null, null, Color.white);
        }

        public static void AddEligibleCache(int thingIdNum, Texture new_texture, Material new_material)
        {
            AddCache(thingIdNum, true, new_texture, new_material, new_material.color);
        }

    }

    public class MappedHealthCard
    {
        public bool isEligible { get; set; }
        public Texture texture { get; set; }
        public Material material { get; set; }
        public Color color { get; set; }
    }
}
