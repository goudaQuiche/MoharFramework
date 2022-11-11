using Verse;
using UnityEngine;

namespace MoharGfx
{
    public static class MoteTools
    {

        public static bool AllowedMoteSpawn(this Map map, Vector3 pos)
        {
            if (map == null || map.moteCounter.SaturatedLowPriority || !pos.ShouldSpawnMotesAt(map))
                return false;

            return true;
        }

        public static bool AllowedMoteSpawn(this Pawn p)
        {
            if (Tools.NegligiblePawn(p))
                return false;

            return p.Position.ShouldSpawnMotesAt(p.Map) && !p.Map.moteCounter.Saturated;
        }

        public static bool AllowedMoteSpawn(this Thing t)
        {
            if (Tools.NegligibleThing(t))
                return false;

            return t.Position.ShouldSpawnMotesAt(t.Map) && !t.Map.moteCounter.Saturated;
        }

        public static bool AllowedMoteSpawn(this Vector3 vector3, Map map)
        {
            return vector3.ToIntVec3().ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority;
        }

    }
}
