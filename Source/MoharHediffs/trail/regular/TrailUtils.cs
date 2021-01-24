using RimWorld;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Verse;

namespace MoharHediffs
{
    public static class TrailUtils
    {
        public static bool PingPongPickColor(this ColorRange colorRange, Color oldColor, bool ToTheTop, out Color newColor)
        {
            newColor = oldColor;
            float iterationDice = Rand.Range(0, colorRange.variationPerIteration);
            
            if (ToTheTop)
            {
                float redVariation = (oldColor.r >= colorRange.maxColor.r) ? 0 : Math.Min(colorRange.maxColor.r - oldColor.r, Rand.Range(0, iterationDice));
                float greenVariation = (oldColor.g >= colorRange.maxColor.g) ? 0 : Math.Min(colorRange.maxColor.g - oldColor.g, Rand.Range(0, redVariation));
                float blueVariation = (oldColor.b >= colorRange.maxColor.b) ? 0 : Math.Min(colorRange.maxColor.b - oldColor.b, Rand.Range(0, greenVariation));
            }
            else
            {

            }

            //Color newColor = new Color(128,128,128);


        }

        public static void TryMoteSpawn(this Vector3 loc, Map map, float rot, float scale, ThingDef moteDef, bool debug = false)
        {
            if (loc.ForbiddenMote(map))
                return;
            //if(Pawn.story.bodyType == BodyTypeDefOf.
            if (moteDef == null)
            {
                if (debug) Log.Warning("null mote");
                return;
            }

            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);
            if (moteThrown == null)
                return;

            moteThrown.Scale = scale;
            moteThrown.exactRotation = rot;
            moteThrown.exactPosition = loc;
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            if (debug) Log.Warning("mote loc:" + loc + " mote.ep:"+ moteThrown.exactPosition);

        }
    }
}
