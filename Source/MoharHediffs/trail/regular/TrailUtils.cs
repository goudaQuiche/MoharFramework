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
        public static float Clamp(this float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static int GetProgressSign(float limA, float limB, float val)
        {
            //int rD = Math.Sign(limA - limB);
            // 128 256 => -1
            // 256 128 => +1

            if (val <= limA && limA < limB)
                return 1;
            else if (val >= limB && limB > limA)
                return -1;
            
            return Rand.Chance(.5f) ? 1 : -1;
        }

        public static Color RandomPickColor(this ColorRange colorRange, Color oldColor, bool debug = false)
        {
            //float iterationDice = Rand.Range(0, colorRange.variationPerIteration);
            float iterationDice = colorRange.variationPerIteration;
            float rW = Rand.Range(0, iterationDice);
            float gW = Rand.Range(0, iterationDice - rW);
            float bW = iterationDice - rW - gW;

            int rM = GetProgressSign(colorRange.colorA.r, colorRange.colorB.r, oldColor.r);
            int bM = GetProgressSign(colorRange.colorA.g, colorRange.colorB.g, oldColor.g);
            int gM = GetProgressSign(colorRange.colorA.b, colorRange.colorB.b, oldColor.b);

            float rC = Math.Abs(colorRange.colorA.r - colorRange.colorB.r) * rW * rM;
            float gC = Math.Abs(colorRange.colorA.g - colorRange.colorB.g) * gW * gM;
            float bC = Math.Abs(colorRange.colorA.b - colorRange.colorB.b) * bW * bM;

            Color newColor =
                new Color(
                    (oldColor.r + rC).Clamp(colorRange.colorA.r, colorRange.colorB.r),
                    (oldColor.g + gC).Clamp(colorRange.colorA.g, colorRange.colorB.g),
                    (oldColor.b + bC).Clamp(colorRange.colorA.b, colorRange.colorB.b)
                );
            /*
            if (debug)
            {
                Log.Warning(
                    "iD:" + iterationDice +
                    " rW:" + rW.ToString("0.00") +" gW:" + gW.ToString("0.00") + " bW:" + bW.ToString("0.00") +
                    " rM:" + rM + " gM:" + gM + " bM:" + bM +
                    " rC:" + rC.ToString("0.00") + " gC:" + gC.ToString("0.00") + " bC:" + bC.ToString("0.00") +
                    " oldColor:"+ oldColor.ToString() + " newColor:" + newColor.ToString()
                    );
            }
            */
            return newColor;
        }
        /*
        public static bool PingPongPickColor(this ColorRange colorRange, Color oldColor, bool ToTheTop, out Color newColor, bool debug=false)
        {
            newColor = oldColor;
            float iterationDice = Rand.Range(0, colorRange.variationPerIteration);
            float redDelta, greenDelta, blueDelta;

            bool redMax = (oldColor.r >= colorRange.maxColor.r);
            bool greenMax = (oldColor.g >= colorRange.maxColor.g);
            bool blueMax = (oldColor.b >= colorRange.maxColor.b);

            if (ToTheTop)
            {
                redDelta = redMax ? 0 : Math.Min(colorRange.maxColor.r - oldColor.r, Rand.Range(0, iterationDice));
                greenDelta = greenMax ? 0 : Math.Min(colorRange.maxColor.g - oldColor.g, Rand.Range(0, redDelta));
                blueDelta = blueMax ? 0 : Math.Min(colorRange.maxColor.b - oldColor.b, Rand.Range(0, greenDelta));

                newColor.r += redDelta;
                newColor.g += greenDelta;
                newColor.b += blueDelta;
                if (debug) Log.Warning(
                    "oldcolor:" + oldColor.ToString() +
                    " toTheTop:" + ToTheTop +
                    " Dice:" + iterationDice +
                    " rD:" + redDelta +
                    " gD:" + greenDelta +
                    " bD:" + blueDelta +
                    " => newColor:" + newColor.ToString()
                    );
                if (newColor.r >= colorRange.maxColor.r && newColor.g >= colorRange.maxColor.g && newColor.b >= colorRange.maxColor.b)
                    return false;
            }
            else
            {

                redDelta = (oldColor.r <= colorRange.minColor.r) ? 0 : Math.Min(oldColor.r - colorRange.minColor.r, Rand.Range(0, iterationDice));
                greenDelta = (oldColor.g <= colorRange.minColor.g) ? 0 : Math.Min(oldColor.g - colorRange.minColor.g, Rand.Range(0, redDelta));
                blueDelta = (oldColor.b <= colorRange.minColor.b) ? 0 : Math.Min(oldColor.b - colorRange.minColor.b, Rand.Range(0, greenDelta));

                newColor.r -= redDelta;
                newColor.g -= greenDelta;
                newColor.b -= blueDelta;

                if (debug) Log.Warning(
                        "oldcolor:" + oldColor.ToString() +
                        " toTheTop:" + ToTheTop +
                        " Dice:" + iterationDice +
                        " rD:" + redDelta +
                        " gD:" + greenDelta +
                        " bD:" + blueDelta +
                        " => newColor:" + newColor.ToString()
                        );

                if (newColor.r <= colorRange.minColor.r && newColor.g <= colorRange.minColor.g && newColor.b <= colorRange.minColor.b)
                    return true;
            }

            return ToTheTop;
        }
        */
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
    }
}
