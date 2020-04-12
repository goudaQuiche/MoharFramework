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
        private int initialTicksUntilSpawn=0;

        int hungerReset=0;
		int healthReset=0;
        int graceTicks = 0;

		Pawn pawn = null;

        float ageWeightedmaxDaysB4Next = 2;
        float ageWeightedminDaysB4Next = 1;

        int ageWeightedQuantity = 1;

        readonly bool myDebug = false;

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

            Scribe_Values.Look(ref ageWeightedminDaysB4Next, "ageWeightedminDaysB4Next");
            Scribe_Values.Look(ref ageWeightedmaxDaysB4Next, "ageWeightedmaxDaysB4Next");
            Scribe_Values.Look(ref ageWeightedQuantity, "ageWeightedQuantity");
            //Scribe_Values.Look(ref hungerReset, "LTF_hungerReset");
            //Scribe_Values.Look(ref healthReset, "LTF_healthReset");

            Scribe_Values.Look(ref graceTicks, "graceTicks");
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();

            Tools.Warn(">>>" + parent.def.defName + " - CompPostMake start", myDebug);
            Tools.Warn(
                parent.def.defName + "Props: " +
                "minDaysB4Next: " + Props.minDaysB4Next + "; " +
                "maxDaysB4Next: " + Props.maxDaysB4Next + "; " +
                "graceDays: " + Props.graceDays + "; " +
                "hungerRelative: " + Props.hungerRelative + "; " +
                "healthRelative:" + Props.healthRelative + "; " +
                "randomGrace:" + Props.randomGrace + "; "
            , myDebug);

            if (Props.animalThing)
                Tools.Warn(
                "animalThing: " + Props.animalThing + "; " +
                "animalName: " + Props.animalToSpawn.defName + "; " +
                "factionOfPlayerAnimal: " + Props.factionOfPlayerAnimal + "; "
                , myDebug);

            if (Props.ageWeighted)
                Tools.Warn(
                "ageWeighted:" + Props.ageWeighted + "; " +
                "olderMoreOften:" + Props.olderMoreOften + "; " +
                "olderBiggerQuantity:" + Props.olderBiggerQuantity + "; " +
                "relativeQuantity:" + Props.relativeQuantity + "; " +
                "exponentialQuantity:" + Props.exponentialQuantity + "; " +
                "exponentialRatioLimit:" + Props.exponentialRatioLimit + "; ",
                myDebug);

            // Checks
            if (Props.spawnCount > 200)
            {
                Tools.Warn("SpawnCount is too high: " + Props.spawnCount + ",  some people just want to see the world burn", myDebug);
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            if (Props.minDaysB4Next >= Props.maxDaysB4Next)
            {
                Tools.Warn("minDaysB4Next should be lower than maxDaysB4Next", myDebug);
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }
            if (Props.minDaysB4Next < .001)
            {
                Tools.Warn("minDaysB4Next is too low", myDebug);
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            // We spawn an animal
            if (Props.animalThing)
            {
                if ((Props.animalToSpawn == null) || Props.animalToSpawn.defName.NullOrEmpty())
                {
                    Tools.Warn("Props.animalThing=" + Props.animalThing + "; but no Props.animalName", myDebug);
                    Tools.DestroyParentHediff(parent);
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
                Tools.Warn("Trying to DefDatabase for thing", myDebug);
                ThingDef myThingDef = DefDatabase<ThingDef>.AllDefs.Where((ThingDef b) => b == Props.thingToSpawn).RandomElement();
                if (myThingDef == null)
                {
                    Tools.Warn("Could not find Props.thingToSpawn in DefDatabase", myDebug);
                    Tools.DestroyParentHediff(parent);
                    return;
                }
            }

            // Only on first spawn ?
            if (initialTicksUntilSpawn == 0)
            {
                Tools.Warn("Reseting countdown bc initialTicksUntilSpawn == 0 (comppostmake)", myDebug);
                ResetCountdown();
            }

            if (Props.ageWeighted) {

                // pawn age / pawn life expectany
                float ageRatio = Tools.GetPawnAgeOverlifeExpectancyRatio(parent.pawn, myDebug);
                Tools.Warn("AgeRatio: " + ageRatio, myDebug);

                /* weighted days depending on age */
                /* ****************************** */
                float daysAgeRatio = ageRatio;
                if (!Props.olderMoreOften)
                    daysAgeRatio = 1 - daysAgeRatio;

                // logical limits checking
                daysAgeRatio = (daysAgeRatio <= 0) ? .1f : daysAgeRatio;
                // lifeexpectancy can be > 1
                //daysAgeRatio = (daysAgeRatio > 1) ? 1f : daysAgeRatio;

                // apllying ratio to days range
                ageWeightedmaxDaysB4Next = Props.maxDaysB4Next * daysAgeRatio;
                ageWeightedminDaysB4Next = Props.minDaysB4Next * daysAgeRatio;

                Tools.Warn(
                    " ageWeightedmaxDaysB4Next" + ageWeightedmaxDaysB4Next + "; " +
                    " ageWeightedminDaysB4Next" + ageWeightedminDaysB4Next + "; "
                    , myDebug);

                /* weighted quantity depending on age */
                /* ********************************** */
                float quantityAgeRatio = Props.olderBiggerQuantity ? 1 - ageRatio : ageRatio;
                Tools.Warn("quantityAgeRatio: " + quantityAgeRatio, myDebug);

                if (Props.relativeQuantity && Props.exponentialQuantity)
                {
                    Tools.Warn("cant have relativeQ and expoQ", myDebug);
                    Tools.DestroyParentHediff(parent, myDebug);
                    return;
                }
                if (Props.exponentialQuantity && (Props.exponentialRatioLimit > 50))
                {
                    Tools.Warn("expoRatioLimit too low while expoQuantity is set, some people just want to see the world burn", myDebug);
                    Tools.DestroyParentHediff(parent, myDebug);
                    return;
                }

                Tools.Warn("ageWeight Checks ok", myDebug);

                Tools.Warn(
                    "relativeQuantity: "+ Props.relativeQuantity +
                    "; exponentialQuantity: " + Props.exponentialQuantity
                    , myDebug);
                if (Props.relativeQuantity)
                {
                    ageWeightedQuantity = (int)Math.Round((double)Props.spawnCount * (1 + quantityAgeRatio));
                }
                else if (Props.exponentialQuantity)
                {
                    if (quantityAgeRatio <= 0 || quantityAgeRatio > 2)
                    {
                        Tools.Warn("quantityAgeRatio is f* up : " + quantityAgeRatio, myDebug);
                        Tools.DestroyParentHediff(parent, myDebug);
                        return;
                    }

                    float expoFactor = 1 / quantityAgeRatio;
                    expoFactor = (expoFactor > Props.exponentialRatioLimit) ? Props.exponentialRatioLimit : expoFactor;

                    ageWeightedQuantity = (int)Math.Round((double)Props.spawnCount * expoFactor);
                }
                else
                {
                    ageWeightedQuantity = (int)Math.Round((double)Props.spawnCount * quantityAgeRatio);
                }

                if (ageWeightedQuantity <= 0)
                {
                    Tools.Warn("ageWeightedQuantity is f* up : " + ageWeightedQuantity, myDebug);
                    Tools.DestroyParentHediff(parent, myDebug);
                    return;
                }

                ageWeightedQuantity = (ageWeightedQuantity < 1) ? 1 : ageWeightedQuantity;

                Tools.Warn(
                "<<<" +
                "Props.olderMoreOften=" + Props.olderMoreOften + "; " +
                "Props.olderBiggerquantities=" + Props.olderBiggerQuantity + "; " +
                " ageRatio=" + ageRatio + "; " +
                " daysAgeRatio=" + daysAgeRatio + "; " +
                " Props.maxDaysB4Next" + Props.maxDaysB4Next + "; " +
                " Props.minDaysB4Next" + Props.minDaysB4Next + "; " +
                " ageWeightedmaxDaysB4Next" + ageWeightedmaxDaysB4Next + "; " +
                " ageWeightedminDaysB4Next" + ageWeightedminDaysB4Next + "; " +
                " quantityAgeRatio=" + quantityAgeRatio + "; " +
                " ageWeightedQuantity=" + ageWeightedQuantity + "; "
                , myDebug
                );
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {

            pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                return;
            }

            if (graceTicks <= 0)
            {
                int randomGraceTicks2wait = (int)(RandomGraceDays() * 60000);

                if (IsHungry())
                {
                    hungerReset++;
                    graceTicks = randomGraceTicks2wait;
                    return;
                }
                else if (IsInjured())
                {
                    healthReset++;
                    graceTicks = randomGraceTicks2wait;
                    return;
                }
                else
                {
                    hungerReset = healthReset = 0;

                    if (CheckShouldSpawn())
                    {
                        Tools.Warn("Reseting countdown bc spawned thing", myDebug);
                        ResetCountdown();

                        if (Rand.Chance(Props.randomGrace))
                        {
                            graceTicks = randomGraceTicks2wait;
                        }
                    }
                }
                return;
            }

            graceTicks--;
        }

        private int CalculatedQuantity
        {
            get
            {
                return (Props.ageWeighted ? ageWeightedQuantity : Props.spawnCount);
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

                //Pawn newAnimal = PawnGenerator.GeneratePawn(myPawnKindDef, animalFaction);
                PawnGenerationRequest request = new PawnGenerationRequest(
                    Props.animalToSpawn, animalFaction, PawnGenerationContext.NonPlayer, -1, false, true);
                /*
                PawnGenerator.GeneratePawn
                if (newAnimal == null)
                {
                    Tools.Warn("could not PawnGenerator with myPawnKindDef:" + myPawnKindDef.defName + "; faction:" + ((animalFaction == null) ? "null" : animalFaction.Name), myDebug);
                    return false;
                }
                */

                for (int i = 0; i < CalculatedQuantity; i++)
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
            int remainingSpawnCount = CalculatedQuantity;
            int loopBreaker = 0;

            while (numSpawned < CalculatedQuantity) {
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
                                        if (thing.def != Props.thingToSpawn || thing.stackCount > this.Props.thingToSpawn.stackLimit - CalculatedQuantity)
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
            float result = 0;

            if (Props.ageWeighted)
                result = Rand.Range(ageWeightedminDaysB4Next, ageWeightedmaxDaysB4Next);
            else
                result = Rand.Range(Props.minDaysB4Next, Props.maxDaysB4Next);

            return result;
        }
        private float RandomGraceDays()
        {
            return (this.Props.graceDays * Rand.Range(0f, 1f));
        }

        private bool IsWounded()
		{
			pawn = this.parent.pawn;
			if (pawn != null)
			{
				float num = 0f;
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
                    //if (hediffs[i] is Hediff_Injury && !hediffs[i].IsOld())
                    if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
                    {
						num += hediffs[i].Severity;
					}
				}
				//Log.Warning( pawn.Label + " is wounded ");
				return (num>0);
			}
			return false;
		}
		
		private bool IsHungry()
		{
			string bla = string.Empty;
			pawn = parent.pawn;
			
			if (pawn != null)
			{
				if (Props.hungerRelative == true){
					if (pawn.needs.food != null && pawn.needs.food.CurCategory == HungerCategory.Starving)
					{
						return true;
					}else{
						return false;
					}
				}
				else{
					return false;
				}
				
			}else{
				Log.Warning (" IsHungry Null Error ");
				return false;
			}
			
		}		
		
		private bool IsInjured()
		{
			pawn = parent.pawn;
			if (pawn != null)
			{
				if (Props.healthRelative == true){
					if ( IsWounded() )
					{
						return true;
					}
					else{
						
					}
					return false;
				}
				else
				{
					return false;
				}
			}else{
				Log.Warning (" IsInjured Null Error ");
				return false;	
			}
			
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

                    result += " " + Props.spawnVerb + "(" + (Props.ageWeighted ? ageWeightedQuantity : Props.spawnCount) + "x)";

                }


                return result;
            }
        }

	}
}
