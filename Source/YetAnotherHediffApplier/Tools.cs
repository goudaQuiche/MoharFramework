using Verse;
using System.Reflection;

namespace YAHA
{
    public static class Tools
    {
        public static bool OkPawn(this Pawn pawn)
        {
            return ((pawn != null) && (pawn.Map != null));
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].description;
            else
                return source.ToString();
        }
    }
}
