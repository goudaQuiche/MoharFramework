using System;
using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace CustomLifeSpan
{
    public static class Lifespan_Utility
    {
        public static IEnumerable<ThoughtDef> deathThought = new List<ThoughtDef>
        {
            ThoughtDefOf.KnowColonistDied,
            ThoughtDefOf.PawnWithGoodOpinionDied,
            ThoughtDefOf.WitnessedDeathFamily,
            ThoughtDefOf.WitnessedDeathAlly
        };

        public static bool IsDeathThought(this ThoughtDef tDef)
        {
            return (deathThought.Contains(tDef));
        }

        public static Thing ThingInCaseOfDeath(Pawn p)
        {
            Thing refThing = null;
            if (p.Dead)
            {
                if (p.Corpse == null)
                    return null;
                refThing = p.Corpse;
            }
            else
                refThing = p;

            return refThing;
        }

        public static void TrySpawnFilth(Thing refT, float filthRadius, ThingDef filthDef)
        {
            if (refT.Map != null && CellFinder.TryFindRandomReachableCellNear(refT.Position, refT.Map, filthRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors), (IntVec3 x) => x.Standable(refT.Map), (Region x) => true, out IntVec3 result))
            {
                FilthMaker.TryMakeFilth(result, refT.Map, filthDef);
            }
        }

        public static void ThrowCustomSmoke(ThingDef moteDef, Vector3 loc, Map map, float size)
        {
            if (loc.ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority)
            {
                MoteThrown obj = (MoteThrown)ThingMaker.MakeThing(moteDef);
                obj.Scale = Rand.Range(1.5f, 2.5f) * size;
                obj.rotationRate = Rand.Range(-30f, 30f);
                obj.exactPosition = loc;
                obj.SetVelocity(Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
                GenSpawn.Spawn(obj, loc.ToIntVec3(), map);
            }
        }


        public static bool TryDoSpawn(Pawn pawn, ThingDef thingDef, int thingNum, int spawnMaxAdjacent, bool tryToUnstack, bool inheritFaction, bool spawnForbidden, bool showMessageIfOwned)
        {
            IntVec3 spawnPos = IntVec3.Invalid;
            Map map = null;
            Thing refThing = null;

            refThing = ThingInCaseOfDeath(pawn);
            if (refThing == null)
                return false;
            else
            {
                map = refThing.Map;
                spawnPos = refThing.Position;
            }


            if (spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 c = spawnPos + GenAdj.AdjacentCellsAndInside[i];
                    if (!c.InBounds(map))
                    {
                        continue;
                    }
                    List<Thing> thingList = c.GetThingList(map);

                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def == thingDef)
                        {
                            if (tryToUnstack) continue;

                            num += thingList[j].stackCount;
                            if (num >= spawnMaxAdjacent)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            if (TryFindSpawnCell(refThing, thingDef, thingNum, tryToUnstack, out IntVec3 result))
            {
                Thing thing = ThingMaker.MakeThing(thingDef);
                thing.stackCount = thingNum;
                if (thing == null)
                {
                    Log.Error("Could not spawn anything for " + refThing);
                }
                if (inheritFaction && thing.Faction != refThing.Faction)
                {
                    thing.SetFaction(refThing.Faction);
                }
                GenPlace.TryPlaceThing(thing, result, map, ThingPlaceMode.Direct, out Thing lastResultingThing);
                if (spawnForbidden)
                {
                    lastResultingThing.SetForbidden(value: true);
                }
                if (showMessageIfOwned && refThing.Faction == Faction.OfPlayer)
                {
                    Messages.Message("MessageCompSpawnerSpawnedItem".Translate(thingDef.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
                }
                return true;
            }
            return false;
        }

        public static bool TryFindSpawnCell(Thing parent, ThingDef thingToSpawn, int spawnCount, bool tryToUnstack, out IntVec3 result)
        {
            foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(parent).InRandomOrder())
            {
                if (item.Walkable(parent.Map))
                {
                    Building edifice = item.GetEdifice(parent.Map);
                    if (edifice == null || !thingToSpawn.IsEdifice())
                    {
                        Building_Door building_Door = edifice as Building_Door;
                        if ((building_Door == null || building_Door.FreePassage) && (parent.def.passability == Traversability.Impassable || GenSight.LineOfSight(parent.Position, item, parent.Map)))
                        {
                            bool flag = false;
                            List<Thing> thingList = item.GetThingList(parent.Map);

                            for (int i = 0; i < thingList.Count; i++)
                            {


                                Thing thing = thingList[i];
                                if (thing.def.category == ThingCategory.Item && (thing.def != thingToSpawn || thing.stackCount > thingToSpawn.stackLimit - spawnCount))
                                {
                                    flag = true;
                                    break;
                                }
                            }

                            if (!flag)
                            {
                                result = item;
                                return true;
                            }
                        }
                    }
                }
            }
            result = IntVec3.Invalid;
            return false;
        }

        public static bool RemoveBadMemoriesOfDeadPawn(Pawn deadPawn, bool myDebug = false)
        {
            bool didIt = false;
            if (deadPawn == null)
            {
                Tools.Warn("removingRelationAndThoughts, null pawn", myDebug);
                return didIt;
            }
            string deadName = deadPawn.LabelShortCap;
            Tools.Warn(">>>>>" + deadName + " dissappeared, the world must not know", myDebug);

            foreach( Pawn p in Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(
                    pH =>
                    pH != deadPawn
                    && pH.needs.mood?.thoughts?.memories != null
                    && pH.needs.mood.thoughts.memories.AnyMemoryConcerns(deadPawn)
                    )){

                Tools.Warn(p.LabelShortCap + " has memories of " + deadName);

                Tools.Warn("pre removal mem count: " + p.needs.mood.thoughts.memories.Memories.Count(), myDebug);
                p.needs.mood.thoughts.memories.Memories.RemoveAll(TM => TM.otherPawn == deadPawn && TM.MoodOffset() <= 0f);
                Tools.Warn("post removal mem count: " + p.needs.mood.thoughts.memories.Memories.Count(), myDebug);
            }

            return didIt;
        }

        public static void removingRelationAndThoughts(Pawn deadPawn, bool myDebug=false)
        {
            if (deadPawn == null)
            {
                Tools.Warn("removingRelationAndThoughts, null pawn", myDebug);
                return;
            }
            string deadName = deadPawn.LabelShortCap;

            Tools.Warn(">>>>>" + deadName + " dissappeared, the world must not know", myDebug);
            foreach (
                Pawn p in Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(
                    pH =>
                    //!pH.AnimalOrWildMan() &&
                    pH != deadPawn
                    && !pH.GetRelations(deadPawn).EnumerableNullOrEmpty()
                    )
                )
            {
                string pName = p.LabelShortCap;
                Tools.Warn("Considering :" + pName, myDebug);
                IEnumerable<PawnRelationDef> relationT = PawnRelationUtility.GetRelations(deadPawn, p);
                if (relationT.EnumerableNullOrEmpty())
                    continue;
                /*
                foreach (PawnRelationDef PRD in relationT)
                {
                    Tools.Warn("Relation between '" + deadName + "' and '" + pName + "' : " + PRD.defName, myDebug);
                    bool didIt = p.relations.TryRemoveDirectRelation(PRD, pawn);
                    Tools.Warn("Relation removed: " + didIt, myDebug);
                }
                */

                List<Thought> pThoughts = new List<Thought>();
                if (p.needs.mood == null || p.needs.mood.thoughts == null)
                    continue;
                //p.needs.mood.thoughts.memories.AnyMemoryConcerns()
                if (pThoughts.NullOrEmpty())
                    return;
                int tNum = 0;
                foreach (Thought thought in pThoughts)
                {
                    Tools.Warn(pName + "'s Thought n°" + tNum, myDebug);
                    tNum++;

                    if (thought.pawn == null || deadPawn == null)
                        continue;

                    if (IsDeathThought(thought.def))
                    {
                        if (!(thought is Thought_MemorySocial TMS && TMS.otherPawn != null && TMS.otherPawn == deadPawn))
                            continue;

                        deadPawn.needs.mood.thoughts.memories.RemoveMemory(TMS);
                        //thought.remo
                        Tools.Warn("removed " + pName + "'s thought " + thought.def.defName, myDebug);
                    }
                }
            }
            Tools.Warn("<<<<<" + deadName, myDebug);
        }
    }
}