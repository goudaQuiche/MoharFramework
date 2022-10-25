using Verse;

namespace CustomLifeSpan
{
    public static class Tools
    {
        public static bool IsPawn(this Thing t)
        {
            return (t is Pawn);
        }

        public static bool Negligeable(this Thing thing)
        {
            return (thing == null || thing.Map == null || thing.Position == null);
        }

        public static bool Negligeable(this Pawn pawn)
        {
            return (pawn == null || !pawn.Spawned || pawn.Map == null || pawn.Position == null);
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

        public static bool IsRaceMember(this Pawn pawn, string raceName)
        {
            return (pawn.def.defName == raceName);
        }

    }
}
