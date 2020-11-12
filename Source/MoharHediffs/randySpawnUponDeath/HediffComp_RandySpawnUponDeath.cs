using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace MoharHediffs
{
	public class HediffComp_RandySpawnUponDeath : HediffComp
	{
        private bool blockSpawn = false;

        private int randomlyChosenIndex = -1;
        public Faction randomlyChosenItemfaction = null;
        public int randomlyChosenQuantity;

        public bool newBorn = false;

        public readonly float minDaysB4NextErrorLimit = .001f;
        public readonly int spawnCountErrorLimit = 750;

        public HediffCompProperties_RandySpawnUponDeath Props => (HediffCompProperties_RandySpawnUponDeath)this.props;

        public bool MyDebug => Props.debug;

        public PawnOrThingParameter CurIP => 
            (randomlyChosenIndex != -1 && !Props.pawnOrThingParameters.NullOrEmpty() && 
            randomlyChosenIndex < Props.pawnOrThingParameters.Count) ? Props.pawnOrThingParameters[randomlyChosenIndex] : null;

        public bool HasValidIP => CurIP != null;

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref randomlyChosenItemfaction, "randomlyChosenItemfaction");
            Scribe_Values.Look(ref randomlyChosenQuantity, "randomlyChosenQuantity");
            Scribe_Values.Look(ref randomlyChosenIndex, "randomlyChosenIndex");
            Scribe_Values.Look(ref newBorn, "newBorn");
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

        public override void Notify_PawnDied()
        {
            Tools.Warn("Entering HediffComp_RandySpawnUponDeath Notify_PawnDied", MyDebug);
            if (Pawn.Corpse.Negligeable())
            {
                Tools.Warn("Corpse is no more, cant find its position - giving up", MyDebug);
                return;
            }
                

            if (blockSpawn)
            {
                Tools.Warn("blockSpawn for some reason- giving up", MyDebug);
                return;
            }
                

            if (CheckShouldSpawn())
            {
                Tools.Warn("Spawn occured", MyDebug);
            }

            base.Notify_PawnDied();
        }

        private void CalculateValues()
        {
            randomlyChosenIndex = this.GetWeightedRandomIndex();
            if (HasValidIP)
            {
                CurIP.ComputeRandomParameters(out randomlyChosenQuantity);
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
            if (randomlyChosenQuantity > spawnCountErrorLimit)
            {
                BlockAndDestroy(">ERROR< calculatedQuantity is too high: " + randomlyChosenQuantity + "(>" + spawnCountErrorLimit + "), check and adjust your hediff props", MyDebug);
                return;
            }
        }

        private void DumpCalculatedValues()
        {
            Tools.Warn(
                "<<< " +
                "; randomlyChosenQuantity: " + randomlyChosenQuantity + "; "
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
            if (Pawn.Corpse.Negligeable())
                return false;

            bool didSpawn = TryDoSpawn(Pawn.Corpse, Pawn.Corpse.Map);
            Tools.Warn("TryDoSpawn: " + didSpawn, MyDebug);

            return didSpawn;
        }


        public bool TrySpawnPawn(IntVec3 position, Map map)
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
                CurIP.pawnKindToSpawn, randomlyChosenItemfaction, PawnGenerationContext.NonPlayer, -1, false, newBorn);

            for (int i = 0; i < randomlyChosenQuantity; i++)
            {
                Pawn NewPawn = PawnGenerator.GeneratePawn(request);
                GenSpawn.Spawn(NewPawn, position, map, WipeMode.Vanish);

                if(CurIP.HasFilth)
                    FilthMaker.TryMakeFilth(position, map, CurIP.filthDef, 1);
            }

            return true;
        }

        public bool TryDoSpawn(Thing thing, Map map)
        {
            if (thing.Negligeable())
            {
                Tools.Warn("TryDoSpawn - pawn null", MyDebug);
                return false;
            }

            if (CurIP.PawnSpawner)
            {
                return TrySpawnPawn(thing.Position, map);
            }
            else if (CurIP.ThingSpawner)
            // Thing case NON animal
            // Trying to stack with an existing pile

            if (Props.spawnMaxAdjacent >= 0)
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
            int remainingSpawnCount = randomlyChosenQuantity;
            int loopBreaker = 0;

            while (numSpawned < randomlyChosenQuantity) {
                if (this.TryFindSpawnCell(thing, map, out IntVec3 center))
                {
                    Thing newThing = ThingMaker.MakeThing(CurIP.thingToSpawn, null);
                    newThing.stackCount = remainingSpawnCount;
                    if (newThing.def.stackLimit > 0)
                        if (newThing.stackCount > newThing.def.stackLimit)
                        {
                            newThing.stackCount = newThing.def.stackLimit;
                        }

                    numSpawned += newThing.stackCount;
                    remainingSpawnCount -= newThing.stackCount;

                    GenPlace.TryPlaceThing(newThing, center, map, ThingPlaceMode.Direct, out Thing t, null);
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

                /*
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
                */
                return result;
            }
        }

	}
}
