using RimWorld;
using Verse;
using System.Reflection;
using UnityEngine;

namespace OLB
{
    public static class Tools
    {
        public static bool ImpossibleMote(this IntVec3 cell, Map map)
        {
            if (map == null || !cell.IsValid)
                return true;

            return !cell.InBounds(map) || (!cell.ShouldSpawnMotesAt(map)) || map.moteCounter.SaturatedLowPriority;
        }

        public static bool ImpossibleMote(this Vector3 vector3, Map map)
        {
            if (map == null)
                return true;

            return vector3.ToIntVec3().ImpossibleMote(map);
        }

        public static bool Negligeable(this Thing thing)
        {
            return (thing == null || !thing.Spawned || thing.Map == null || thing.Position == null);
        }

        public static bool Negligeable(this Building b)
        {
            return (b == null || !b.Spawned || b.Map == null || b.Position == null);
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

        public static bool HasPower(this CompPowerTrader comp)
        {
            return (comp != null && comp.PowerOn);
        }
        public static bool IsNotEmpty(this CompRefuelable comp)
        {
            return (comp != null && comp.Fuel>0);
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].description;
            else return source.ToString();
        }
    }
}
