using System;
using UnityEngine;
using System.Linq;

using Verse;

namespace MoharHediffs
{
    public static class TrailUtils
    {
        public static bool IsTerrainAllowed(this HediffComp_TrailLeaver comp, Map map, TerrainDef terrain, IntVec3 pPos)
        {
            if (terrain == null || map == null)
                return false;

            if (!comp.Props.HasTerrainRestriction)
                return true;

            if (!comp.Props.terrain.allowedInWater && terrain.IsWater)
                return false;
            if (comp.Props.terrain.HasRelevantSnowRestriction && !comp.Props.terrain.allowedSnowDepth.Includes(map.snowGrid.GetDepth(pPos)))
                return false;
            if (comp.Props.terrain.HasForbiddenTerrains && comp.Props.terrain.forbiddenTerrains.Contains(terrain))
                return false;

            return true;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static int GetProgressSign(float limA, float limB, float val)
        {
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

        public static float GetMoteRotation(this HediffComp_TrailLeaver comp, Vector3 drawPos, out Vector3 normalized)
        {
            normalized = Vector3.zero;
            float rot = comp.Props.dynamicRotation ? comp.GetDynamicRotation(drawPos, out normalized) : 0;
            rot += comp.Props.HasRotationOffset ? comp.Props.rotationOffset : 0;

            if (comp.MyDebug) Log.Warning("GetMoteRotation normalized" + normalized);

            return rot % 360;
        }

        public static float GetDynamicRotation(this HediffComp_TrailLeaver comp, Vector3 drawPos, out Vector3 normalized)
        {
            normalized = (drawPos - comp.lastMotePos).normalized;
            return normalized.AngleFlat();
        }

        public static Vector3 GetFootPrintOffset(this HediffComp_TrailLeaver comp, Vector3 normalized)
        {
            if (!comp.Props.UsesFootPrints)
                return Vector3.zero;

            float angle = comp.lastFootprintRight ? 90 : (-90);
            Vector3 b = normalized.RotatedBy(angle) * comp.Props.footprint.distanceBetweenFeet * Mathf.Sqrt(comp.Pawn.BodySize);

            comp.lastFootprintRight = !comp.lastFootprintRight;

            if (comp.MyDebug)
                Log.Warning($"{comp.Props.footprint.offset} {b}");

            return comp.Props.footprint.offset + b;
        }

        public static Vector3 GetBodyTypeOffset(this HediffComp_TrailLeaver comp)
        {
            if (comp.Pawn.story?.bodyType == null || !comp.Props.HasOffset)
                return comp.Props.defaultOffset;

            BodyTypeOffset BTO = comp.Props.offSetPerBodyType.Where(b => b.bodyType == comp.Pawn.story.bodyType).FirstOrFallback();
            return BTO == null ? comp.Props.defaultOffset : BTO.offset;
        }

        public static void ChangeMoteColor(this HediffComp_TrailLeaver comp, Mote mote)
        {
            if (!comp.Props.HasColorRange || mote == null)
                return;

            if (comp.lastColor == Color.black)
                comp.lastColor = comp.Props.colorRange.colorA;

            comp.lastColor = comp.Props.colorRange.RandomPickColor(comp.lastColor, comp.MyDebug);

            mote.instanceColor = comp.lastColor;
        }
        public static void RecordMotePos(this HediffComp_TrailLeaver comp, Vector3 drawPos)
        {
            if (!comp.Props.dynamicRotation)
                return;

            comp.lastMotePos = drawPos;
        }
    }
}
