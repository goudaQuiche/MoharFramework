using Verse;
using System;
using UnityEngine;

namespace MoharDamage
{
    public static class MoteThrower
    {
        public static bool ImpossibleMote(Map map, IntVec3 cell)
        {
            if (map == null || !cell.IsValid)
                return true;

            return map.moteCounter.SaturatedLowPriority || !cell.InBounds(map) || !cell.ShouldSpawnMotesAt(map);
        }

        public static bool ImpossibleMote(this Map map, Vector3 pos)
        {
            if (map == null || map.moteCounter.SaturatedLowPriority || !pos.ShouldSpawnMotesAt(map))
                return true;

            return false;
        }

        public static void ThrowPawnEffectMote(Vector3 loc, Map map, ThingDef moteDef, bool rotation = true)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef, null);
            moteThrown.Scale = Rand.Range(0.5f, 1.2f);
            if (rotation)
                moteThrown.rotationRate = Rand.Range(-12f, 36f);

            moteThrown.exactPosition = loc;
            moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity(Rand.Range(35, 45), 1.2f);

            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        public static void ThrowSmokeCustom(Vector3 loc, Map map, float size, ThingDef smokeMote)
        {
            if (map.ImpossibleMote(loc))
                return;

            MoteThrown obj = (MoteThrown)ThingMaker.MakeThing(smokeMote);
            obj.Scale = Rand.Range(1.5f, 2.5f) * size;
            obj.rotationRate = Rand.Range(-30f, 30f);
            obj.exactPosition = loc;
            obj.SetVelocity(Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(obj, loc.ToIntVec3(), map);
        }


    }
}
