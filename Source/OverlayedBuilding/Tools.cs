using RimWorld;
using Verse;
using System.Reflection;

namespace OLB
{
    public static class Tools
    {
        public static bool ImpossibleMote(Map map, IntVec3 cell)
        {
            if (map == null || !cell.IsValid)
                return true;

            return !cell.InBounds(map) || (!cell.ShouldSpawnMotesAt(map)) || map.moteCounter.SaturatedLowPriority;
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
