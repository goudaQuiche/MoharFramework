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

        public int calculatedQuantity;

        public int hungerReset = 0;
		public int healthReset = 0;

        private bool blockSpawn = false;

        private int pickedItem = -1;
        public Faction Itemfaction = null;

        public readonly float minDaysB4NextErrorLimit = .001f;
        public readonly int spawnCountErrorLimit = 750;

        public HediffCompProperties_RandySpawner Props => (HediffCompProperties_RandySpawner)this.props;

        public bool MyDebug => Props.debug;

        public bool HasGraceDelay => graceTicks > 0;
        private float CalculatedDaysB4Next => (float)ticksUntilSpawn / 60000;

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

            this.DumpProps();
            this.CheckProps();

            CalculateValues();
            CheckCalculatedValues();
            DumpCalculatedValues();
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
            }
        }



        private void CalculateValues()
        {
            pickedItem = this.GetWeightedRandomIndex();
            if (HasValidIP)
            {
                CurIP.ComputeRandomParameters(out ticksUntilSpawn, out graceTicks, out calculatedQuantity);
                if (CurIP.HasFactionParams)
                    this.ComputeRandomFaction();
            }
            else
            {
                BlockAndDestroy(">ERROR< failed to find an index for IP, check and adjust your hediff props", MyDebug);
                return;
            }

        }

        private void CheckCalculatedValues()
        {
            if (calculatedQuantity > spawnCountErrorLimit)
            {
                BlockAndDestroy(">ERROR< calculatedQuantity is too high: " + calculatedQuantity + "(>" + spawnCountErrorLimit + "), check and adjust your hediff props", MyDebug);
                return;
            }

            if (CalculatedDaysB4Next < minDaysB4NextErrorLimit)
            {
                BlockAndDestroy(">ERROR< calculatedMinDaysB4Next is too low: " + CalculatedDaysB4Next + "(<" + minDaysB4NextErrorLimit + "), check and adjust your hediff props", MyDebug);
                return;
            }
        }

        private void DumpCalculatedValues()
        {
            Tools.Warn(
                "<<< " +
                " calculatedDaysB4Next: " + CalculatedDaysB4Next +
                "; CalculatedQuantity: " + calculatedQuantity + "; "
                , MyDebug
            );
        }

        public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            Tools.Warn(ErrorLog, myDebug);
            blockSpawn = true;
            Tools.DestroyParentHediff(parent, myDebug);
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

        public bool TrySpawnPawn()
        {
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
                CurIP.pawnKindToSpawn, Itemfaction, PawnGenerationContext.NonPlayer, -1, false, CurIP.NewBorn);

            for (int i = 0; i < calculatedQuantity; i++)
            {
                Pawn NewBorn = PawnGenerator.GeneratePawn(request);
                GenSpawn.Spawn(NewBorn, Pawn.Position, Pawn.Map, WipeMode.Vanish);

                if(CurIP.HasFilth)
                    FilthMaker.TryMakeFilth(parent.pawn.Position, parent.pawn.Map, CurIP.filthDef, 1);
            }

            return true;
        }

        public bool TryDoSpawn()
        {
            if (Pawn.Negligeable())
            {
                Tools.Warn("TryDoSpawn - pawn null", MyDebug);
                return false;
            }

            if (CurIP.PawnSpawner)
            {
                return TrySpawnPawn();
            }
            else if (CurIP.ThingSpawner)
            // Thing case NON animal
            // Trying to stack with an existing pile

            if (Props.spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 curCell = Pawn.Position + GenAdj.AdjacentCellsAndInside[i];
                    if (!curCell.InBounds(Pawn.Map))
                    {
                        continue;
                    }
                    List<Thing> thingList = (curCell).GetThingList(Pawn.Map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def == CurIP.thingToSpawn)
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
                if (this.TryFindSpawnCell(out IntVec3 center))
                {
                    Thing thing = ThingMaker.MakeThing(CurIP.thingToSpawn, null);
                    thing.stackCount = remainingSpawnCount;
                    if (thing.def.stackLimit > 0)
                        if (thing.stackCount > thing.def.stackLimit)
                        {
                            thing.stackCount = thing.def.stackLimit;
                        }

                    numSpawned += thing.stackCount;
                    remainingSpawnCount -= thing.stackCount;

                    GenPlace.TryPlaceThing(thing, center, Pawn.Map, ThingPlaceMode.Direct, out Thing t, null);
                    if (Props.spawnForbidden)
                    {
                        t.SetForbidden(true, true);
                    }

                }

                if (loopBreaker++ > 10)
                {
                    Tools.Warn("Had to break the loop", MyDebug);
                    return false;
                }
                    
            }

            if (remainingSpawnCount <= 0)
                return true;


            return false;

        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (!HasValidIP || !Props.logNextSpawn)
                    return result;

                if (HasGraceDelay)
                {
                    if (CurIP.PawnSpawner)
                    {
                        result = " No " + CurIP.pawnKindToSpawn.label + " for " + (graceTicks).ToStringTicksToPeriod();
                    }
                    else if(CurIP.ThingSpawner)
                    {
                        result = " No " + CurIP.thingToSpawn.label + " for " + (graceTicks).ToStringTicksToPeriod();
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
                    else if (CurIP.ThingSpawner)
                    {
                        result += CurIP.thingToSpawn.label;
                    }

                    result += " " + CurIP.spawnVerb + "(" + calculatedQuantity + "x)";

                }

                return result;
            }
        }

	}
}
