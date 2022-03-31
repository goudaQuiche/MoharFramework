using RimWorld;
using UnityEngine;
using Verse;



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

        public static Thing SpawnMote(MoteDecoration Item, Building building, Pawn worker)
        {
            if (building.Negligeable())
                return null;

            Map map = building.Map;

            //Log.Warning("inside SpawnMote");

            // drawpos and actual position
            DisplayOrigin.GetDrawPos(Item.origin, building, worker, out IntVec3 cell, out Vector3 drawPos);

            if (drawPos.ImpossibleMote(map))
                return null;

            MoteThrown mote = (MoteThrown)ThingMaker.MakeThing(Item.moteDef, null);
            mote.exactPosition = drawPos;
            if(Item.moteDef.graphicData.drawOffset != null)
            {
                mote.exactPosition += Item.moteDef.graphicData.drawOffset;
            }

            if (Item.transformation == null)
                return GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);

            DisplayTransformation myItemData = Item.transformation;

            // more drawpos
            Vector3 randomV3 = new Vector3(myItemData.randomXOffset.RandomInRange, 0, myItemData.randomYOffset.RandomInRange);
            
            if (myItemData.HasOffset)
            {
                //Vector3 myOffset = new Vector3(Offset.GetOffset(building.Rotation).x + randomV3.x, 0, Offset.GetOffset(building.Rotation).y + randomV3.y);
                Vector3 myOffset = new Vector3(myItemData.offset.GetOffset(building.Rotation).x + randomV3.x, 0, myItemData.offset.GetOffset(building.Rotation).y + randomV3.y);
                mote.exactPosition += myOffset.RotatedBy(building.Rotation.AsAngle);
            }

            // rotation
            mote.rotationRate = myItemData.rotationRate.RandomInRange;
            mote.exactRotation = myItemData.exactRotation.RandomInRange;
            if (Rand.Chance(myItemData.randomHalfRotation.RandomInRange))
                mote.exactRotation += 180;

            //scale
            mote.Scale = myItemData.scale.RandomInRange;
            // velocity
            mote.SetVelocity(myItemData.xVelocity.RandomInRange, myItemData.yVelocity.RandomInRange);

            //Log.Warning("SpawnMote end");

            return GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);
        }



    }
}
