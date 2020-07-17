using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

using AlienRace;


namespace OLB
{
    public static class GfxEffects
    {
        public static Vector3 HeadPos(this Pawn pawn)
        {
            Vector3 answer = pawn.DrawPos;

            if (pawn.Rotation == Rot4.North || pawn.Rotation == Rot4.South)
                return answer + new Vector3(0f, 0f, 0.38f);

            //return drawPos + new Vector3(0f, 0f, 0.32f);
            if (pawn.gender == Gender.Male)
            {
                if (pawn.story.bodyType == BodyTypeDefOf.Male)
                {
                    if (pawn.Rotation == Rot4.East)
                    {
                        answer += new Vector3(-0.015f, 0f, 0.375f);
                    }
                    else if (pawn.Rotation == Rot4.West)
                    {
                        answer += new Vector3(0.015f, 0f, 0.375f);
                    }
                }else if (pawn.story.bodyType == BodyTypeDefOf.Thin)
                {
                    if (pawn.Rotation == Rot4.East)
                    {
                        answer += new Vector3(-0.015f, 0f, 0.375f);
                    }
                    else if (pawn.Rotation == Rot4.West)
                    {
                        answer += new Vector3(0.015f, 0f, 0.375f);
                    }
                }
                else if (pawn.story.bodyType == BodyTypeDefOf.Hulk)
                {
                    if (pawn.Rotation == Rot4.East)
                    {
                        answer += new Vector3(0.049f, 0f, 0.375f);
                    }
                    else if (pawn.Rotation == Rot4.West)
                    {
                        answer += new Vector3(0.079f, 0f, 0.375f);
                    }
                }
                else if (pawn.story.bodyType == BodyTypeDefOf.Fat)
                {
                    if (pawn.Rotation == Rot4.East)
                    {
                        answer += new Vector3(-0.015f, 0f, 0.375f);
                    }
                    else if (pawn.Rotation == Rot4.West)
                    {
                        answer += new Vector3(0.015f, 0f, 0.375f);
                    }
                }
                return answer;

            }
            else if(pawn.gender == Gender.Female)
            {
                if (pawn.Rotation == Rot4.East)
                    answer += new Vector3(.042f, 0f, 0.39f);

                if (pawn.Rotation == Rot4.West)
                    answer += new Vector3(-.042f, 0f, 0.39f);

                return answer;
            }

            //should not happen
            return answer + new Vector3(0f, 0f, 0.38f);
        }

        public static Thing SpawnMote(MoteDecoration decoration, MoteTracing tracer, Building building, Pawn worker)
        {
            if (building.Negligeable())
                return null;

            Map map = building.Map;

            // drawpos and actual position
            MyDefs.GetDrawPos(tracer.origin, building, worker, out IntVec3 cell, out Vector3 drawPos);

            if (Tools.ImpossibleMote(map, cell))
                return null;

            MoteThrown mote = (MoteThrown)ThingMaker.MakeThing(decoration.moteDef, null);

            // more drawpos
            Vector3 randomV3 = new Vector3(decoration.randomOffset.RandomInRange, 0, decoration.randomOffset.RandomInRange);
            Vector3 myOffset = new Vector3(Offset.GetOffset(building.Rotation).x + randomV3.x, 0, Offset.GetOffset(building.Rotation).y + randomV3.y);
            mote.exactPosition = drawPos + myOffset.RotatedBy(building.Rotation.AsAngle);
            //mote.exactPosition = drawPos + Vector3.up.RotatedBy(building.Rotation.AsAngle);

            // rotation
            mote.rotationRate = decoration.rotationRate.RandomInRange;
            mote.exactRotation = decoration.exactRotation.RandomInRange;
            //scale
            mote.Scale = decoration.scale.RandomInRange;
            // velocity
            mote.SetVelocity(decoration.xVelocity.RandomInRange, decoration.yVelocity.RandomInRange);

            return GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);
        }



    }
}
