using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace MoharHediffs
{
	public class HediffComp_RandySpawner : HediffComp
	{
        private int ticksUntilSpawn;
        private int initialTicksUntilSpawn = 0;
        public int graceTicks = 0;

        private int calculatedQuantity;
        private float calculatedDaysB4Next => (float)ticksUntilSpawn / 60000;

        public int hungerReset = 0;
		public int healthReset = 0;

        private bool blockSpawn = false;

        private int pickedItem = -1;

        readonly float minDaysB4NextErrorLimit = .001f;
        readonly int spawnCountErrorLimit = 750;

        public HediffCompProperties_RandySpawner Props => (HediffCompProperties_RandySpawner)this.props;

        public bool MyDebug => Props.debug;
        public bool HasGraceDelay => graceTicks > 0;
        public ItemParameter CurIP => (pickedItem != -1 && !Props.itemParameters.NullOrEmpty() && pickedItem < Props.itemParameters.Count) ? Props.itemParameters[pickedItem] : null;
        public bool HasValidIP => CurIP != null;

        public bool RequiresFood => Props.hungerRelative && Pawn.IsHungry(MyDebug);
        public bool RequiresHealth => Props.healthRelative && Pawn.IsInjured(MyDebug);

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksUntilSpawn, "ticksUntilSpawn");
            Scribe_Values.Look(ref initialTicksUntilSpawn, "initialTicksUntilSpawn");

            Scribe_Values.Look(ref calculatedQuantity, "calculatedQuantity");

            Scribe_Values.Look(ref hungerReset, "LTF_hungerReset");
            Scribe_Values.Look(ref healthReset, "LTF_healthReset");

            Scribe_Values.Look(ref graceTicks, "graceTicks");

            Scribe_Values.Look(ref pickedItem, "pickedItem");
        }

        public override void CompPostMake()
        {
            Tools.Warn(">>> " + Pawn?.Label + " - " + parent.def.defName + " - CompPostMake start", MyDebug);

            DumpProps();
            CheckProps();
            CalculateValues();
            CheckCalculatedValues();
            DumpCalculatedValues();

            // Only on first spawn ?
            if (initialTicksUntilSpawn == 0)
            {
                Tools.Warn("Reseting countdown bc initialTicksUntilSpawn == 0 (comppostmake)", MyDebug);
                ResetCountdown();
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligeable())
                return;

            if (blockSpawn)
                return;

            if (HasGraceDelay)
            {
                graceTicks--;
                return;
            }

            if(this.SetRequirementGraceTicks())
                return;

            if (CheckShouldSpawn())
            {
                Tools.Warn("Reseting countdown bc spawned thing", MyDebug);

                CalculateValues();
                CheckCalculatedValues();
                ResetCountdown();
            }
        }

        private void DumpProps()
        {
            Tools.Warn(
            "hungerRelative: " + Props.hungerRelative + "; " +
            "healthRelative: " + Props.healthRelative + "; "
            , MyDebug);

            for (int i = 0; i < Props.itemParameters.Count; i++)
            {
                ItemParameter IP = Props.itemParameters[i];
                IP.LogParams(MyDebug);
            }
        }

        private void CalculateValues()
        {
            pickedItem = this.GetWeightedRandomIndex();
            Props.itemParameters[pickedItem].ComputeRandomParameters(out ticksUntilSpawn, out graceTicks, out calculatedQuantity);
        }

        private void CheckCalculatedValues()
        {
            if (calculatedQuantity > spawnCountErrorLimit)
            {
                BlockAndDestroy(">ERROR< calculatedQuantity is too high: " + calculatedQuantity + "(>" + spawnCountErrorLimit + "), check and adjust your hediff props", MyDebug);
                return;
            }

            if (calculatedDaysB4Next < minDaysB4NextErrorLimit)
            {
                BlockAndDestroy(">ERROR< calculatedMinDaysB4Next is too low: " + calculatedDaysB4Next + "(<" + minDaysB4NextErrorLimit + "), check and adjust your hediff props", MyDebug);
                return;
            }
        }

        private void DumpCalculatedValues()
        {
            Tools.Warn(
                "<<< " +
                " calculatedDaysB4Next: " + calculatedDaysB4Next +
                "; CalculatedQuantity: " + calculatedQuantity + "; "
                , MyDebug
            );
        }

        private void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            Tools.Warn(ErrorLog, myDebug);
            blockSpawn = true;
            Tools.DestroyParentHediff(parent, myDebug);
        }

        private void CheckProps()
        {
            if (Props.itemParameters.NullOrEmpty())
                BlockAndDestroy(Pawn.Label + " props: no itemParameters - giving up", MyDebug);

            // Logical checks
            for (int i = 0; i < Props.itemParameters.Count; i++)
            {
                ItemParameter IP = Props.itemParameters[i];
                if(IP.spawnCount.min > spawnCountErrorLimit || IP.spawnCount.max > spawnCountErrorLimit)
                {
                    BlockAndDestroy(Pawn.Label + " props: SpawnCount is too high: >" + spawnCountErrorLimit, MyDebug);
                    return;
                }
                    
                if(IP.daysB4Next.min < minDaysB4NextErrorLimit)
                {
                    BlockAndDestroy(Pawn.Label + " props: minDaysB4Next is too low: " + IP.daysB4Next.min + "<" + minDaysB4NextErrorLimit, MyDebug);
                    return;
                }

                if(!IP.ThingSpawner && !IP.PawnSpawner)
                {
                    BlockAndDestroy(Pawn.Label + " props: not a thing nor pawn spawner bc no def for either", MyDebug);
                    return;
                }
            }
        }

        private bool CheckShouldSpawn()
        {
            ticksUntilSpawn--;
            if (ticksUntilSpawn <= 0)
            {
                bool didSpawn = TryDoSpawn();
                Tools.Warn("TryDoSpawn: " + didSpawn, MyDebug);

                if (didSpawn)
                    pickedItem = -1;

                return didSpawn;
            }

            return false;
        }

        /*
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
        */

            public bool TrySpawnPawn

        public bool TryDoSpawn()
        {
            if (Pawn.Negligeable())
            {
                Tools.Warn("TryDoSpawn - pawn null", MyDebug);
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
            if (Pawn.Negligeable())
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

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (!HasValidIP)
                    return result;

                if (HasGraceDelay)
                {
                    if (CurIP.PawnSpawner)
                    {
                        result = " No " + CurIP.pawnKindToSpawn.label + " for " + (graceTicks).ToStringTicksToPeriod();
                    }
                    else if(CurIP.ThingSpawner)
                    {
                        result = " No " + CurIP.pawnKindToSpawn.label + " for " + (graceTicks).ToStringTicksToPeriod();
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
                    if (CurIP.PawnSpawner)
                    {
                        result += CurIP.pawnKindToSpawn.label;
                    }
                    else
                    {
                        result += CurIP.pawnKindToSpawn.label;
                    }

                    result += " " + CurIP.spawnVerb + "(" + calculatedQuantity + "x)";

                }

                return result;
            }
        }

	}
}
