using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public static class SpawnerUtils
    {
        public static bool TrySpawnPawn(this HediffComp_RandySpawnUponDeath comp, IntVec3 position, int randomQuantity, Map map)
        {
            ThingSettings TS = comp.ChosenItem;

            /*
             * public PawnGenerationRequest(
             1   PawnKindDef kind, Faction faction = null, PawnGenerationContext context = PawnGenerationContext.NonPlayer, int tile = -1, bool forceGenerateNewPawn = false, 
             2   bool newborn = false, bool allowDead = false, bool allowDowned = false, bool canGeneratePawnRelations = true, bool mustBeCapableOfViolence = false, 
             3   float colonistRelationChanceFactor = 1, bool forceAddFreeWarmLayerIfNeeded = false, bool allowGay = true, bool allowFood = true, bool allowAddictions = true,
             4   bool inhabitant = false, bool certainlyBeenInCryptosleep = false, bool forceRedressWorldPawnIfFormerColonist = false, bool worldPawnFactionDoesntMatter = false, float biocodeWeaponChance = 0,
             5   Pawn extraPawnForExtraRelationChance = null, float relationWithExtraPawnChanceFactor = 1, Predicate<Pawn> validatorPreGear = null, Predicate<Pawn> validatorPostGear = null, IEnumerable<TraitDef> forcedTraits = null,
             6   IEnumerable<TraitDef> prohibitedTraits = null, float? minChanceToRedressWorldPawn = null, float? fixedBiologicalAge = null, float? fixedChronologicalAge = null, Gender? fixedGender = null,
             7   float? fixedMelanin = null, string fixedLastName = null, string fixedBirthName = null, RoyalTitleDef fixedTitle = null);
                */

            //PawnGenerationContext PGC = randomlyChosenItemfaction==Faction.OfPlayer ? PawnGenerationContext.

            PawnKindDef PKD = comp.PawnOfChoice;

            PawnGenerationRequest request = new PawnGenerationRequest(PKD, comp.randomlyChosenItemfaction, PawnGenerationContext.NonPlayer, -1, false, TS.newBorn);

            for (int i = 0; i < randomQuantity; i++)
            {
                Pawn NewPawn = PawnGenerator.GeneratePawn(request);

                comp.SetAge(NewPawn);
                comp.SetName(NewPawn);

                GenSpawn.Spawn(NewPawn, position, map, WipeMode.Vanish);

                if (comp.HasFilth)
                    FilthMaker.TryMakeFilth(position, map, comp.FilthToSpawn, 1);
            }

            return true;
        }

        public static bool TrySpawnThing(this HediffComp_RandySpawnUponDeath comp, Thing thing, int randomQuantity, Map map)
        {
            if (comp.Props.spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 curCell = thing.Position + GenAdj.AdjacentCellsAndInside[i];
                    if (!curCell.InBounds(map))
                    {
                        continue;
                    }
                    List<Thing> thingList = (curCell).GetThingList(map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def == comp.ChosenItem.thingToSpawn)
                        {
                            num += thingList[j].stackCount;
                            if (num >= comp.Props.spawnMaxAdjacent)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            int numSpawned = 0;
            int remainingSpawnCount = randomQuantity;
            int loopBreaker = 0;

            while (numSpawned < randomQuantity)
            {
                if (comp.TryFindSpawnCell(thing, randomQuantity, map, out IntVec3 center))
                {
                    Thing newThing = ThingMaker.MakeThing(comp.ChosenItem.thingToSpawn, null);
                    newThing.stackCount = remainingSpawnCount;
                    if (newThing.def.stackLimit > 0)
                        if (newThing.stackCount > newThing.def.stackLimit)
                        {
                            newThing.stackCount = newThing.def.stackLimit;
                        }

                    numSpawned += newThing.stackCount;
                    remainingSpawnCount -= newThing.stackCount;

                    GenPlace.TryPlaceThing(newThing, center, map, ThingPlaceMode.Direct, out Thing t, null);
                    if (comp.Props.spawnForbidden)
                    {
                        t.SetForbidden(true, true);
                    }

                }

                if (loopBreaker++ > 10)
                {
                    Tools.Warn("Had to break the loop", comp.MyDebug);
                    return false;
                }

            }

            if (remainingSpawnCount <= 0)
                return true;

            return false;
        }

        public static bool TryDoSpawn(this HediffComp_RandySpawnUponDeath comp, Thing thing, int randomQuantity, Map map)
        {

            if (thing.Negligeable())
            {
                Tools.Warn("TryDoSpawn - pawn null", comp.MyDebug);
                return false;
            }

            if (comp.HasChosenPawn)
            {
                Tools.Warn("TryDoSpawn -> TrySpawnPawn", comp.MyDebug);
                return comp.TrySpawnPawn(thing.Position, randomQuantity, map);
            }
            else if (comp.HasChosenThing)
            {
                Tools.Warn("TryDoSpawn -> TrySpawnPawn", comp.MyDebug);
                return comp.TrySpawnThing(thing, randomQuantity, map);
            }

            return false;

            //else if (CurIP.ThingSpawner)
            // Thing case NON animal
            // Trying to stack with an existing pile

            
        }

        public static bool TryFindSpawnCell(this HediffComp_RandySpawnUponDeath comp, Thing refThing, int randomQuantity, Map map, out IntVec3 result)
        {
            ThingDef thingDef = comp.ChosenItem.thingToSpawn;

            if (refThing.Negligeable())
            {
                result = IntVec3.Invalid;
                Tools.Warn("TryFindSpawnCell Null - pawn null", comp.MyDebug);
                return false;
            }

            foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(refThing).InRandomOrder(null))
            {
                if (current.Walkable(map))
                {
                    Building edifice = current.GetEdifice(map);
                    if (edifice == null || !thingDef.IsEdifice())
                    {
                        if (!(edifice is Building_Door building_Door) || building_Door.FreePassage)
                        {
                            if (GenSight.LineOfSight(refThing.Position, current, map, false, null, 0, 0))
                            {
                                bool flag = false;
                                List<Thing> thingList = current.GetThingList(map);
                                for (int i = 0; i < thingList.Count; i++)
                                {
                                    Thing thing = thingList[i];
                                    if (thing.def.category == ThingCategory.Item)
                                        if (thing.def != thingDef || thing.stackCount > thingDef.stackLimit - randomQuantity)
                                        {
                                            flag = true;
                                            break;
                                        }
                                }
                                if (!flag)
                                {
                                    result = current;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            Tools.Warn("TryFindSpawnCell Null - no spawn cell found", comp.MyDebug);
            result = IntVec3.Invalid;
            return false;

        }
    }
}