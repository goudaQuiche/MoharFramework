/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace MoharHediffs
{
	public class HediffComp_Spawner : HediffComp
	{
        private int ticksUntilSpawn;
        private int initialTicksUntilSpawn = 0;

        int hungerReset=0;
		int healthReset=0;
        int graceTicks = 0;

		Pawn pawn = null;

        float calculatedMaxDaysB4Next = 2;
        float calculatedMinDaysB4Next = 1;

        int calculatedQuantity = 1;

        bool blockSpawn = false;

        bool myDebug = false;

        readonly float worldBurnMinDaysB4Next = .001f;
        readonly int worldBurnExponentialLimit = 20;
        readonly int worldBurnSpawnCount = 750;

        public HediffCompProperties_Spawner Props
		{
			get
			{
				return (HediffCompProperties_Spawner)this.props;
			}
		}


        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksUntilSpawn, "ticksUntilSpawn");
            Scribe_Values.Look(ref initialTicksUntilSpawn, "initialTicksUntilSpawn");

            Scribe_Values.Look(ref calculatedMinDaysB4Next, "calculatedMinDaysB4Next");
            Scribe_Values.Look(ref calculatedMaxDaysB4Next, "calculatedMaxDaysB4Next");
            Scribe_Values.Look(ref calculatedQuantity, "calculatedQuantity");
            //Scribe_Values.Look(ref hungerReset, "LTF_hungerReset");
            //Scribe_Values.Look(ref healthReset, "LTF_healthReset");

            Scribe_Values.Look(ref graceTicks, "graceTicks");
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();
            myDebug = Props.debug;

            Tools.Warn(">>> " + parent.pawn.Label + " - " + parent.def.defName + " - CompPostMake start", myDebug);
            Tools.Warn(
            "Props => " +
            "minDaysB4Next: " + Props.minDaysB4Next + "; " +
            "maxDaysB4Next: " + Props.maxDaysB4Next + "; " +
            "randomGrace: " + Props.randomGrace + "; " +
            "graceDays: " + Props.graceDays + "; " +
            "hungerRelative: " + Props.hungerRelative + "; " +
            "healthRelative: " + Props.healthRelative + "; "
            , myDebug);

            if (Props.animalThing)
                Tools.Warn(
                "animalThing: " + Props.animalThing + "; " +
                "animalName: " + Props.animalToSpawn.defName + "; " +
                "factionOfPlayerAnimal: " + Props.factionOfPlayerAnimal + "; "
                , myDebug);

            if (Props.ageWeightedQuantity)
            {
                Tools.Warn(
                "ageWeightedQuantity:" + Props.ageWeightedQuantity + "; " +
                "olderBiggerQuantity:" + Props.olderBiggerQuantity + "; " +
                myDebug);
                if (Props.exponentialQuantity)
                Tools.Warn(
                "exponentialQuantity:" + Props.exponentialQuantity + "; " +
                "exponentialRatioLimit:" + Props.exponentialRatioLimit + "; ",
                myDebug);
            }

            if (Props.ageWeightedPeriod) { }
                Tools.Warn(
                "ageWeightedPeriod:" + Props.ageWeightedPeriod + "; " +
                "olderSmallerPeriod:" + Props.olderSmallerPeriod + "; " +
                myDebug);

            // Logical checks
            if (Props.spawnCount > worldBurnSpawnCount)
            {
                Tools.Warn("SpawnCount is too high: " + Props.spawnCount + "(>" + worldBurnSpawnCount + "),  some people just want to see the world burn", myDebug);
                blockSpawn = true;
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            if (Props.minDaysB4Next >= Props.maxDaysB4Next)
            {
                Tools.Warn("minDaysB4Next should be lower than maxDaysB4Next", myDebug);
                blockSpawn = true;
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }
            if (Props.minDaysB4Next < worldBurnMinDaysB4Next)
            {
                Tools.Warn("minDaysB4Next is too low: " + Props.minDaysB4Next + "(<" + worldBurnMinDaysB4Next + "), some people just want to see the world burn", myDebug);
                blockSpawn = true;
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            // We spawn an animal
            if (Props.animalThing)
            {
                if ((Props.animalToSpawn == null) || Props.animalToSpawn.defName.NullOrEmpty())
                {
                    Tools.Warn("Props.animalThing=" + Props.animalThing + "; but no Props.animalName", myDebug);
                    blockSpawn = true;
                    Tools.DestroyParentHediff(parent, myDebug);
                    return;
                }
                else
                {
                    Tools.Warn("Found animal PawnKindDef.defName=" + Props.animalToSpawn.defName, myDebug);
                }
            }
            // We spawn a thing (non animal)
            else
            {
                ThingDef myThingDef = DefDatabase<ThingDef>.AllDefs.Where((ThingDef b) => b == Props.thingToSpawn).RandomElement();
                if (myThingDef == null)
                {
                    Tools.Warn("Could not find Props.thingToSpawn in DefDatabase", myDebug);
                    blockSpawn = true;
                    Tools.DestroyParentHediff(parent, myDebug);
                    return;
                }
                else
                {
                    Tools.Warn("Found ThingDef for " + Props.thingToSpawn.defName + "in DefDatabase", myDebug);
                }
            }

            // pawn age / pawn life expectany
            float ageRatio = Tools.GetPawnAgeOverlifeExpectancyRatio(parent.pawn, myDebug);
            // lifeexpectancy can be > 1
            ageRatio = (ageRatio > 1) ? 1 : ageRatio;

            if (!Props.ageWeightedPeriod && Props.olderSmallerPeriod)
                Tools.Warn("olderSmallerPeriod ignored since ageWeightedPeriod is false ", myDebug);

            //default behavior
            calculatedMinDaysB4Next = Props.minDaysB4Next;
            calculatedMaxDaysB4Next = Props.maxDaysB4Next;
            if (Props.ageWeightedPeriod) {
                /* weighted days depending on age */
                float daysAgeRatio = Props.olderSmallerPeriod ? -ageRatio : ageRatio;

                // apllying ratio to days range
                calculatedMinDaysB4Next = Props.minDaysB4Next * (1 + daysAgeRatio);
                calculatedMaxDaysB4Next = Props.maxDaysB4Next * (1 + daysAgeRatio);

                Tools.Warn(
                " ageWeightedPeriod: " + Props.ageWeightedPeriod +
                " ageRatio: " + ageRatio +
                " minDaysB4Next: " + Props.minDaysB4Next +
                " maxDaysB4Next: " + Props.maxDaysB4Next +
                " daysAgeRatio: " + daysAgeRatio +
                " calculatedMinDaysB4Next: " + calculatedMinDaysB4Next + "; " +
                " calculatedMaxDaysB4Next: " + calculatedMaxDaysB4Next + "; "
                , myDebug);
            }

            if (calculatedMinDaysB4Next < worldBurnMinDaysB4Next)
            {
                Tools.Warn("calculatedMinDaysB4Next is too low: " + calculatedMinDaysB4Next + "(<" + worldBurnMinDaysB4Next + "), some people just want to see the world burn", myDebug);
                blockSpawn = true;
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }
            if (calculatedMinDaysB4Next >= calculatedMaxDaysB4Next)
            {
                Tools.Warn("calculatedMinDaysB4Next should be lower than calculatedMaxDaysB4Next", myDebug);
                blockSpawn = true;
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            if (!Props.ageWeightedQuantity && Props.olderBiggerQuantity)
                Tools.Warn("olderBiggerQuantity ignored since ageWeightedQuantity is false ", myDebug);
            if (!Props.ageWeightedQuantity && Props.exponentialQuantity)
                Tools.Warn("exponentialQuantity ignored since ageWeightedQuantity is false ", myDebug);

            calculatedQuantity = Props.spawnCount;
            if (Props.ageWeightedQuantity) {
                /* weighted quantity depending on age */
                float quantityAgeRatio = Props.olderBiggerQuantity ? ageRatio : -ageRatio;

                Tools.Warn("quantityAgeRatio: " + quantityAgeRatio, myDebug);

                if (Props.exponentialQuantity && (Props.exponentialRatioLimit > worldBurnExponentialLimit))
                {
                    Tools.Warn("expoRatioLimit too low while expoQuantity is set: " + Props.exponentialRatioLimit + "(>" + worldBurnExponentialLimit + "), some people just want to see the world burn", myDebug);
                    blockSpawn = true;
                    Tools.DestroyParentHediff(parent, myDebug);
                    return;
                }

                calculatedQuantity = (int)Math.Round((double)Props.spawnCount * (1 + quantityAgeRatio));

                if (Props.exponentialQuantity)
                {
                    // 
                    quantityAgeRatio = 1-ageRatio;
                    if (quantityAgeRatio == 0)
                    {
                        Tools.Warn("quantityAgeRatio is f* up : " + quantityAgeRatio, myDebug);
                        blockSpawn = true;
                        Tools.DestroyParentHediff(parent, myDebug);
                        return;
                    }
                    float expoFactor = Props.olderBiggerQuantity ? 1 / quantityAgeRatio : quantityAgeRatio * quantityAgeRatio;

                    bool gotLimited = false;
                    bool gotAugmented = false;
                    if(expoFactor > Props.exponentialRatioLimit)
                    {
                        expoFactor = Props.exponentialRatioLimit;
                        gotLimited = true;
                    }
                    calculatedQuantity = (int)Math.Round((double)Props.spawnCount * (expoFactor));
                    if (calculatedQuantity < 1)
                    {
                        calculatedQuantity = 1;
                        gotAugmented = true;
                    }

                    Tools.Warn(
                    " exponentialQuantity: " + Props.exponentialQuantity +
                    "; expoFactor: " + expoFactor +
                    "; gotLimited: " + gotLimited +
                    "; gotAugmented: " + gotAugmented
                    , myDebug);
                }
                Tools.Warn(
                "; Props.spawnCount: " + Props.spawnCount +
                "; calculatedQuantity: " + calculatedQuantity
                , myDebug);
            }

            if (calculatedQuantity > worldBurnSpawnCount)
            {
                Tools.Warn("calculatedQuantity is too high: " + Props.spawnCount + "(>" + worldBurnSpawnCount + "),  some people just want to see the world burn", myDebug);
                blockSpawn = true;
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            // Only on first spawn ?
            if (initialTicksUntilSpawn == 0)
            {
                Tools.Warn("Reseting countdown bc initialTicksUntilSpawn == 0 (comppostmake)", myDebug);
                ResetCountdown();
            }

            Tools.Warn(
            "<<< " +
            ((Props.ageWeightedPeriod) ? ("Props.olderMoreOften: " + Props.olderSmallerPeriod + "; ") : ("")) +
            ((Props.ageWeightedQuantity) ? ("Props.olderBiggerquantities: " + Props.olderBiggerQuantity + "; ") : ("")) +
            ((Props.ageWeightedQuantity || Props.ageWeightedPeriod) ? (" ageRatio: " + ageRatio + "; ") : ("")) +
            " Props.minDaysB4Next: " + Props.minDaysB4Next + "; Props.maxDaysB4Next: " + Props.maxDaysB4Next + "; " +
            " calculatedMinDaysB4Next: " + calculatedMinDaysB4Next + "; calculatedMaxDaysB4Next: " + calculatedMaxDaysB4Next + "; " +
            " Props.spawnCount: " + Props.spawnCount + "; CalculatedQuantity: " + calculatedQuantity + "; "
            , myDebug
            );
        }

        public override void CompPostTick(ref float severityAdjustment)
        {

            pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                return;
            }

            if (blockSpawn)
                return;

            if (graceTicks > 0)
            {
                graceTicks--;
                return;
            }

            
            if (Props.hungerRelative && Tools.IsHungry(pawn, myDebug))
            {
                int randomGraceTicks2wait = (int)(RandomGraceDays() * 60000);
                hungerReset++;
                graceTicks = randomGraceTicks2wait;
                return;
            }
            else if (Props.healthRelative && Tools.IsInjured(pawn, myDebug))
            {
                int randomGraceTicks2wait = (int)(RandomGraceDays() * 60000);
                healthReset++;
                graceTicks = randomGraceTicks2wait;
                return;
            }

            hungerReset = healthReset = 0;

            if (CheckShouldSpawn())
            {
                Tools.Warn("Reseting countdown bc spawned thing", myDebug);
                ResetCountdown();

                if (Rand.Chance(Props.randomGrace))
                {
                    int randomGraceTicks2wait = (int)(RandomGraceDays() * 60000);
                    graceTicks = randomGraceTicks2wait;
                }
            }
        }

        private bool CheckShouldSpawn()
        {
            pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                Tools.Warn("CheckShouldSpawn pawn Null", myDebug);
                return false;
            }

            ticksUntilSpawn--;
            if (ticksUntilSpawn <= 0)
            {
                bool didSpawn = TryDoSpawn();
                Tools.Warn("TryDoSpawn: " + didSpawn, myDebug);
                return true;
            }

            return false;
        }

        PawnKindDef MyPawnKindDefNamed (string myDefName)
        {
            PawnKindDef answer = null;
            foreach (PawnKindDef curPawnKindDef in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (curPawnKindDef.defName == myDefName)
                    return curPawnKindDef;
            }
            return answer;
        }

        public bool TryDoSpawn()
        {
            pawn = this.parent.pawn;

            if (!Tools.OkPawn(pawn))
            {
                Tools.Warn("TryDoSpawn - pawn null", myDebug);
                return false;
            }

            if (this.Props.animalThing) {
                Faction animalFaction = (Props.factionOfPlayerAnimal) ? Faction.OfPlayer : null;

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

                PawnGenerationRequest request = new PawnGenerationRequest(
                    Props.animalToSpawn, animalFaction, PawnGenerationContext.NonPlayer, -1, false, true);

                for (int i = 0; i < calculatedQuantity; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    GenSpawn.Spawn(pawn, parent.pawn.Position, parent.pawn.Map, WipeMode.Vanish);

                    FilthMaker.TryMakeFilth(parent.pawn.Position, parent.pawn.Map, ThingDefOf.Filth_AmnioticFluid, 1);
                }

                return true;
            }
            // Thing case NON animal
            // Trying to stack with an existing pile

            if (Props.spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 curCell = pawn.Position + GenAdj.AdjacentCellsAndInside[i];
                    if (!curCell.InBounds(pawn.Map))
                    {
                        continue;
                    }
                    List<Thing> thingList = (curCell).GetThingList(pawn.Map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def == Props.thingToSpawn)
                        {
                            num += thingList[j].stackCount;
                            if (num >= Props.spawnMaxAdjacent)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            int numSpawned = 0;
            int remainingSpawnCount = calculatedQuantity;
            int loopBreaker = 0;

            while (numSpawned < calculatedQuantity) {
                if (TryFindSpawnCell(out IntVec3 center))
                {
                    Thing thing = ThingMaker.MakeThing(Props.thingToSpawn, null);
                    thing.stackCount = remainingSpawnCount;
                    if (thing.def.stackLimit > 0)
                        if (thing.stackCount > thing.def.stackLimit)
                        {
                            thing.stackCount = thing.def.stackLimit;
                        }

                    numSpawned += thing.stackCount;
                    remainingSpawnCount -= thing.stackCount;

                    GenPlace.TryPlaceThing(thing, center, pawn.Map, ThingPlaceMode.Direct, out Thing t, null);
                    if (Props.spawnForbidden)
                    {
                        t.SetForbidden(true, true);
                    }

                }

                if (loopBreaker++ > 10)
                {
                    Tools.Warn("Had to break the loop", myDebug);
                    return false;
                }
                    
            }

            if (remainingSpawnCount <= 0)
                return true;


            return false;

        }

        private bool TryFindSpawnCell(out IntVec3 result)
        {
            pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                result = IntVec3.Invalid;
                Tools.Warn("TryFindSpawnCell Null - pawn null", myDebug);
                return false;
            }

            foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(pawn).InRandomOrder(null))
            {
                if (current.Walkable(pawn.Map))
                {
                    Building edifice = current.GetEdifice(pawn.Map);
                    if (edifice == null || !Props.thingToSpawn.IsEdifice())
                    {
                        if (!(edifice is Building_Door building_Door) || building_Door.FreePassage)
                        {
                            if (GenSight.LineOfSight(pawn.Position, current, pawn.Map, false, null, 0, 0))
                            {
                                bool flag = false;
                                List<Thing> thingList = current.GetThingList(pawn.Map);
                                for (int i = 0; i < thingList.Count; i++)
                                {
                                    Thing thing = thingList[i];
                                    if (thing.def.category == ThingCategory.Item)
                                        if (thing.def != Props.thingToSpawn || thing.stackCount > Props.thingToSpawn.stackLimit - calculatedQuantity)
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

            Tools.Warn("TryFindSpawnCell Null - no spawn cell found", myDebug);
            result = IntVec3.Invalid;
            return false;

        }

		private void ResetCountdown()
		{
            ticksUntilSpawn = initialTicksUntilSpawn = (int)(RandomDays2wait() * 60000);
        }

        private float RandomDays2wait()
        {
            float result = Rand.Range(calculatedMinDaysB4Next, calculatedMaxDaysB4Next);
            return result;
        }
        private float RandomGraceDays()
        {
            return (this.Props.graceDays * Rand.Range(0f, 1f));
        }

        


        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (this.graceTicks > 0)
                {
                    if (this.Props.animalThing)
                    {
                        result = " No " + Props.animalToSpawn.defName + " for " + (graceTicks).ToStringTicksToPeriod();
                    }
                    else
                    {
                        result = " No " + Props.thingToSpawn.label + " for " + (graceTicks).ToStringTicksToPeriod();
                    }

                    if (hungerReset > 0)
                    {
                        result += "(hunger)";
                    }
                    else if (healthReset > 0)
                    {
                        result += "(injury)";
                    }
                    else
                    {
                        result += "(grace period)";
                    }
                }
                else {

                    result = ticksUntilSpawn.ToStringTicksToPeriod() + " before ";
                    if (Props.animalThing)
                    {
                        result += Props.animalToSpawn.defName;
                    }
                    else
                    {
                        result += Props.thingToSpawn.label;
                    }

                    result += " " + Props.spawnVerb + "(" + calculatedQuantity + "x)";

                }

                return result;
            }
        }

	}
}
