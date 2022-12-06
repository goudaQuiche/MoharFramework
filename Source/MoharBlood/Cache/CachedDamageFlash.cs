using Verse;
using UnityEngine;

namespace MoharBlood
{
    public static class CachedDamageFlash
    {
        //DamageFlash
        public static bool GetCache(Thing thing, out bool isEligible, out Color color)
        {
            isEligible = false;
            color = MyDefs.BugColor;

            if (StaticCollections.DamageFlashCache.TryGetValue(thing) is MappedDamageFlash mdf)
            {
                isEligible = mdf.isEligible;
                color = mdf.color;
                return true;
            }
            return false;
        }

        public static void AddCache(Thing thing, bool new_isEligible, Color new_color)
        {
            if (StaticCollections.DamageFlashCache.ContainsKey(thing))
                return;

            StaticCollections.DamageFlashCache[thing] = new MappedDamageFlash
            {
                isEligible = new_isEligible,
                color = new_color,
            };
        }

        public static void AddIneligibleCache(Thing thing)
        {
            AddCache(thing, false, MyDefs.BugColor);
        }

        public static void AddEligibleCache(Thing thing, Color new_color)
        {
            AddCache(thing, true, new_color);
        }
    }

    public class MappedDamageFlash
    {
        public Color color { get; set; }
        public bool isEligible { get; set; }
    }
}
