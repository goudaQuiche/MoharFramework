using System;
using UnityEngine;
using MoharGfx;

using Verse;

namespace MoharHediffs
{
    public static class MoteSpawnUtils
    {
        
        public static Thing TryMoteSpawn(this Vector3 loc, Map map, float rot, float scale, ThingDef moteDef, bool debug = false)
        {
            if (loc.ForbiddenMote(map))
                return null;
            //if(Pawn.story.bodyType == BodyTypeDefOf.
            if (moteDef == null)
            {
                if (debug) Log.Warning("null mote");
                return null;
            }

            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);
            if (moteThrown == null)
                return null;

            moteThrown.Scale = scale;
            moteThrown.exactRotation = rot;
            moteThrown.exactPosition = loc;

            //if (debug) Log.Warning("mote loc:" + loc + " mote.ep:"+ moteThrown.exactPosition);
            return GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);

        }

        public static Thing TryAnyMoteSpawn(this Vector3 loc, Map map, float rot, float scale, ThingDef moteDef, bool debug = false)
        {
            if (loc.ForbiddenMote(map))
                return null;
            //if(Pawn.story.bodyType == BodyTypeDefOf.
            if (moteDef == null)
            {
                if (debug) Log.Warning("null mote");
                return null;
            }
            Type moteType = moteDef.thingClass;
            if(moteType == typeof(CustomTransformation_Mote))
            {
                CustomTransformation_Mote castedMote = (CustomTransformation_Mote)ThingMaker.MakeThing(moteDef);
                return castedMote.FinalizeMoteSpawn(loc, map, rot, scale);
            }else if(moteType == typeof(MoteThrown))
            {
                MoteThrown castedMote = (MoteThrown)ThingMaker.MakeThing(moteDef);
                return castedMote.FinalizeMoteSpawn(loc, map, rot, scale);
            }

            return null;
        }

        public static Thing FinalizeMoteSpawn(this CustomTransformation_Mote mote, Vector3 loc, Map map, float rot, float scale)
        {
            mote.Scale = scale;
            mote.exactRotation = rot;
            mote.exactPosition = loc;

            return GenSpawn.Spawn(mote, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

        public static Thing FinalizeMoteSpawn(this MoteThrown mote, Vector3 loc, Map map, float rot, float scale)
        {
            mote.Scale = scale;
            mote.exactRotation = rot;
            mote.exactPosition = loc;

            return GenSpawn.Spawn(mote, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        /*
        public static Thing FinalizeMoteSpawn(this MoteAttached mote, Vector3 loc, Map map, float rot, float scale)
        {
            mote.Scale = scale;
            mote.exactRotation = rot;
            mote.exactPosition = loc;

            return GenSpawn.Spawn(mote, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        */
    }
}
