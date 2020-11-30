using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace MoharJoy
{
    public static class Tools
    {
        public static void Warn(string warning, bool debug = false)
        {
            if (debug)
                Log.Warning(warning);
        }

        public static bool AllowedMoteSpawn(this Pawn p)
        {
            if (p.NegligeablePawn())
                return false;

            return p.Position.ShouldSpawnMotesAt(p.Map) && !p.Map.moteCounter.Saturated;
        }
        public static bool AllowedMoteSpawn(this Thing t)
        {
            if (t.NegligeableThing())
                return false;

            return t.Position.ShouldSpawnMotesAt(t.Map) && !t.Map.moteCounter.Saturated;
        }
        public static bool AllowedMoteSpawn(this Vector3 vector3, Map map)
        {
            return vector3.ToIntVec3().ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority;
        }

        public static bool NegligeablePawn(this Pawn pawn)
        {
            return pawn == null || !pawn.Spawned || pawn.Map == null || pawn.Position == null;
        }

        public static bool NegligeableThing(this Thing thing)
        {
            return thing == null || !thing.Spawned || thing.Map == null || thing.Position == null;
        }
    }
}
