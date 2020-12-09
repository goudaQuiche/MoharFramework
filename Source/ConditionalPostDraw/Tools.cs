using RimWorld;
using Verse;
using System.Reflection;

namespace ConPoDra
{
    public static class Tools
    {
        public static bool Negligeable(this Thing thing)
        {
            return (thing == null || !thing.Spawned || thing.Map == null || thing.Position == null);
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }


        public static bool IsFueled(this CompRefuelable comp)
        {
            return comp != null && comp.Fuel > 0;
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
