using Verse;
using UnityEngine;

namespace MoharGfx
{
    public static class Tools
    {
        public static Vector3 BetweenTouchingCells(this IntVec3 A, IntVec3 B)
        {
            return A.ToVector3Shifted() + (B - A).ToVector3().normalized * 0.5f;
        }

        public static bool NegligiblePawn(this Pawn pawn)
        {
            return pawn == null || !pawn.Spawned || pawn.Map == null || pawn.Position == null;
        }

        public static bool NegligibleThing(this Thing thing)
        {
            return thing == null || !thing.Spawned || thing.Map == null || thing.Position == null;
        }
    }
}
