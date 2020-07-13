using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OHFP
{
    public static class Tools
    {

        public static bool NegligeablePawn(this Pawn pawn)
        {
            return pawn == null || !pawn.Spawned || pawn.Map == null || pawn.Position == null;
        }
        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

    }
}
