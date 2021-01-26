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

        public static bool AllowedMoteSpawn(this Map map, Vector3 pos)
        {
            if (map == null || map.moteCounter.SaturatedLowPriority || !pos.ShouldSpawnMotesAt(map))
                return false;

            return true;
        }

        public static bool AllowedMoteSpawn(this Pawn p)
        {
            if (p.NegligiblePawn())
                return false;

            return p.Position.ShouldSpawnMotesAt(p.Map) && !p.Map.moteCounter.Saturated;
        }
        public static bool AllowedMoteSpawn(this Thing t)
        {
            if (t.NegligibleThing())
                return false;

            return t.Position.ShouldSpawnMotesAt(t.Map) && !t.Map.moteCounter.Saturated;
        }
        public static bool AllowedMoteSpawn(this Vector3 vector3, Map map)
        {
            return vector3.ToIntVec3().ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority;
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
