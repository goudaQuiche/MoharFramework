using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public static class SpawnerUtils
    {
        public static bool TrySpawnPawn(this HediffComp_RandySpawnUponDeath comp, Thing refThing, int randomQuantity)
        {
            string debugStr = comp.MyDebug ? (comp.Pawn.LabelShort + " TrySpawnPawn ") : "";

            ThingSettings TS = comp.ChosenItem;

            IntVec3 position = refThing.Position;
            Map map = refThing.Map;
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

            //bool allowDead = false; bool allowDowned = false; bool canGeneratePawnRelations = true; bool mustBeCapableOfViolence = false;
            //float colonistRelationChanceFactor = 1; bool forceAddFreeWarmLayerIfNeeded = false; bool allowGay = true; bool allowFood = true; bool allowAddictions = true;
            //bool inhabitant = false; bool certainlyBeenInCryptosleep = false; bool forceRedressWorldPawnIfFormerColonist = false; bool worldPawnFactionDoesntMatter = false; float biocodeWeaponChance = 0;
            //Pawn extraPawnForExtraRelationChance = null; float relationWithExtraPawnChanceFactor = 1;
            //Predicate<Pawn> validatorPreGear = null; Predicate<Pawn> validatorPostGear = null; IEnumerable<TraitDef> forcedTraits = null; IEnumerable<TraitDef> prohibitedTraits = null; 
            //float? minChanceToRedressWorldPawn = null; float? fixedBiologicalAge = null; float? fixedChronologicalAge = null; Gender? fixedGender = null;
            //PawnGenerationContext PGC = randomlyChosenItemfaction==Faction.OfPlayer ? PawnGenerationContext.

            PawnKindDef PKD = comp.PawnOfChoice;

            PawnGenerationRequest request = 
                new PawnGenerationRequest(
                    kind: PKD, faction: comp.RandomFaction, context: PawnGenerationContext.NonPlayer, tile: -1, forceGenerateNewPawn: false,
                    newborn: TS.newBorn, colonistRelationChanceFactor: 0, allowAddictions: false, allowFood: false, relationWithExtraPawnChanceFactor:0
                );

            for (int i = 0; i < randomQuantity; i++)
            {
                Pawn NewPawn = PawnGenerator.GeneratePawn(request);

                comp.SetAge(NewPawn);

                if (TS.IsCopier)
                {
                    comp.SetName(NewPawn);
                    comp.SetGender(NewPawn);

                    comp.SetMelanin(NewPawn);
                    comp.SetAlienSkinColor(NewPawn);
                    comp.SetBodyType(NewPawn);
                    comp.SetCrownType(NewPawn);
                    comp.SetHair(NewPawn);
                    comp.SetHairColor(NewPawn);

                    comp.SetHediff(NewPawn);

                    PawnCopyUtils.InitRememberBackstories(out Backstory rememberChildBS, out Backstory rememberAdultBS);
                    if (comp.ChosenItem.copyParent.passions || comp.ChosenItem.copyParent.traits)
                    {
                        comp.RememberBackstories(NewPawn, out rememberChildBS, out rememberAdultBS);
                        comp.ResetBackstories(NewPawn);
                        //comp.ResetDisabledWorks(NewPawn, comp.MyDebug);

                        comp.SetPassions(NewPawn);
                        comp.SetSkills(NewPawn);
                        comp.SetTraits(NewPawn);
                        //comp.CopyDisabledWorks(NewPawn, comp.MyDebug);
                    }
                    if (rememberChildBS != null || rememberAdultBS != null)
                        comp.ReinjectBackstories(NewPawn, rememberChildBS, rememberAdultBS);

                    comp.SetBackstories(NewPawn);
                    comp.UpdateDisabilities(NewPawn);
                }

                if (TS.IsRedresser)
                {
                    comp.DestroyApparel(NewPawn);
                    comp.DestroyEquipment(NewPawn);
                    comp.DestroyInventory(NewPawn);
                }

                if (comp.HasContainerSpawn)
                {
                    if( refThing is Building_Casket Casket)
                    {
                        if (!Casket.TryAcceptThing(NewPawn))
                        {
                            if (comp.MyDebug) Log.Warning(debugStr + " tried to add " + NewPawn.LabelShort + " to " + refThing.Label + ", but failed");
                        }
                    }
                }
                else
                    GenSpawn.Spawn(NewPawn, position, map, WipeMode.Vanish);

                if (comp.ChosenItem.HasMentalStateParams)
                {
                    comp.ComputeRandomMentalState();
                    if (comp.RandomMS != null)
                        NewPawn.mindState.mentalStateHandler.TryStartMentalState(comp.RandomMS, null, forceWake: false, causedByMood: false, otherPawn: null, transitionSilently: true);
                }

                comp.TrySpawnAllFilth(refThing);

                if(comp.MyDebug)Log.Warning("------------------");
            }

            return true;
        }

        public static void TrySpawnAllFilth(this HediffComp_RandySpawnUponDeath comp, Thing refThing, bool debug = false) {
            if(debug)Log.Warning(comp.Pawn.LabelShort + " - TrySpawnAllFilth");

            if (!comp.HasFilth)
            {
                if (debug) Log.Warning("no filth found");
                return;
            }


            int randFilthNum = comp.FilthNum.RandomInRange;

            for (int i = 0; i < randFilthNum; i++)
            {
                if (debug) Log.Warning(
                    "filth " + i + "/" + randFilthNum +
                    " - fDef:" + comp.FilthToSpawn +
                    " - pos:" + refThing.Position + 
                    " - map null?" + (refThing.Map == null)
                    );

                TrySpawnFilth(refThing, comp.FilthRadius.RandomInRange, comp.FilthToSpawn);
            }

        }

        public static void TrySpawnFilth(Thing refT, float filthRadius, ThingDef filthDef)
        {
            if (refT.Map != null && CellFinder.TryFindRandomReachableCellNear(refT.Position, refT.Map, filthRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors), (IntVec3 x) => x.Standable(refT.Map), (Region x) => true, out IntVec3 result))
            {
                FilthMaker.TryMakeFilth(result, refT.Map, filthDef);
            }
        }

        public static bool TrySpawnThing(this HediffComp_RandySpawnUponDeath comp, Thing thing, int randomQuantity)
        {
            Map map = thing.Map;

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
                    if (comp.MyDebug) Log.Warning("Had to break the loop");
                    return false;
                }

            }

            if (remainingSpawnCount <= 0)
                return true;

            return false;
        }

        public static bool TryDoSpawn(this HediffComp_RandySpawnUponDeath comp, Thing thing, int randomQuantity)
        {
            if (thing.Negligible())
            {
                if(comp.MyDebug)Log.Warning("TryDoSpawn - negligeable");
                return false;
            }

            if (comp.HasChosenPawn)
            {
                if (comp.MyDebug) Log.Warning("TryDoSpawn -> TrySpawnPawn");
                return comp.TrySpawnPawn(thing, randomQuantity);
            }
            else if (comp.HasChosenThing)
            {
                if (comp.MyDebug) Log.Warning("TryDoSpawn -> TrySpawnPawn", comp.MyDebug);
                return comp.TrySpawnThing(thing, randomQuantity);
            }

            return false;
        }

        public static bool TryFindSpawnCell(this HediffComp_RandySpawnUponDeath comp, Thing refThing, int randomQuantity, Map map, out IntVec3 result)
        {
            ThingDef thingDef = comp.ChosenItem.thingToSpawn;

            if (refThing.Negligible())
            {
                result = IntVec3.Invalid;
                if (comp.MyDebug) Log.Warning("TryFindSpawnCell Null - pawn null");
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

            if (comp.MyDebug) Log.Warning("TryFindSpawnCell Null - no spawn cell found");
            result = IntVec3.Invalid;
            return false;

        }
    }
}