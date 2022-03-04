using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DUDOD
{
    public static class Tools
    {
        public static bool NegligeablePawn(this Pawn pawn)
        {
            return pawn == null || !pawn.Spawned || pawn.Map == null || pawn.Position == null;
        }
    }
}
