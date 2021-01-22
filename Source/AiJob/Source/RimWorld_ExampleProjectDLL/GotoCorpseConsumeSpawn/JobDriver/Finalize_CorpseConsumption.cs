using Verse;
using System;
using Verse.AI;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace MoharAiJob
{
    public static class Finalize_CorpseConsumption
    {
        public static Toil SpawnProductDespawnCorpse(this CorpseProduct CP, Pawn ParentPawn, IntVec3 SpawnPos, Corpse corpse, Map map, bool MyDebug = false)
        {
            return new Toil {
                initAction = delegate
                {
                    CP.SpawnConsumptionProduct(corpse, ParentPawn, SpawnPos, map, MyDebug);
                    corpse.Destroy(DestroyMode.KillFinalize);
                },
                atomicWithPrevious = true
            };
        }

        public static void DestroyCorpse(this Corpse corpse)
        {
            Log.Warning("DestroyCorpse");
            
        }

        public static bool MakeManhunter(this Pawn p, bool MyDebug = false)
        {
            if (p.NegligiblePawn())
                return false;

            MentalStateDef manhunterState = MentalStateDefOf.Manhunter;
            if(MyDebug) Log.Warning(p.LabelShort + " trying to go " + manhunterState.defName);

            if (p.mindState == null || p.mindState.mentalStateHandler == null)
            {
                if (MyDebug) Log.Warning(p.LabelShort + " null mindstate", MyDebug);
                return false;
            }

            if (MyDebug) Log.Warning(p.LabelShort + " got applied " + manhunterState.defName);
            return p.mindState.mentalStateHandler.TryStartMentalState(manhunterState);
        }

        public static void SpawnConsumptionProduct(this CorpseProduct CP, Corpse corpse, Pawn ParentPawn, IntVec3 SpawnPos, Map map, bool MyDebug = false)
        {
            string DebugStr = MyDebug? ParentPawn.ThingID + " SpawnConsumptionProduct " : null;
            //Log.Warning("SpawnConsumptionProduct");
            //PawnKindDef PKD = CP.pawnKind.RandomElementByWeight()
            if (CP == null)
            {
                if(MyDebug) Log.Warning( DebugStr + "No CorpseProduct found");
                return;
            }

            if (CP.HasRelevantCombatPowerPerMass)
            {
                if (MyDebug) Log.Warning(DebugStr + "CombatPowerPerMass");
                int potentialCP = (int)(corpse.GetStatValue(StatDefOf.Mass) * CP.combatPowerPerMass);
                int NewCombatPowerLimit = CP.HasRelevantCombatPowerLimit ? Math.Min(CP.combatPowerLimit, potentialCP) : potentialCP;

                if (MyDebug) Log.Warning(DebugStr + "corpse:" + corpse.Label + "potentialCP:" + potentialCP + " NewCombatPowerLimit:" + NewCombatPowerLimit);

                int CombatPowerSoFar = 0;
                int loopBreaker = 20;
                do
                {
                    CombatPowerSoFar += CP.SpawnSinglePawn(ParentPawn, SpawnPos, map, MyDebug);
                    if (MyDebug) Log.Warning(DebugStr + "Spawned "+ CombatPowerSoFar+"/"+ NewCombatPowerLimit);
                    loopBreaker--;
                } while (loopBreaker > 0 && CombatPowerSoFar <= NewCombatPowerLimit);

                if (MyDebug && loopBreaker <= 0) Log.Warning(DebugStr + "Got stuck in an infinite loop, had to break it");
            }
            else if (CP.HasRelevantCombatPowerLimit)
            {
                if (MyDebug) Log.Warning(DebugStr + "CombatPowerLimit");

                int CombatPowerSoFar = 0;
                int loopBreaker = 20;
                do
                {
                    CombatPowerSoFar += CP.SpawnSinglePawn(ParentPawn, SpawnPos, map, MyDebug);
                    if (MyDebug) Log.Warning(DebugStr + "Spawned " + CombatPowerSoFar + "/" + CP.combatPowerLimit);
                    loopBreaker--;
                } while (loopBreaker > 0 && CombatPowerSoFar <= CP.combatPowerLimit);

                if (MyDebug && loopBreaker <= 0) Log.Warning(DebugStr + "Got stuck in an infinite loop, had to break it");
            }
            else
            {
                int SpawnedPawnNum = CP.pawnNum.RandomInRange;
                for (int i = 0; i < SpawnedPawnNum; i++)
                    CP.SpawnSinglePawn(ParentPawn, SpawnPos, map, MyDebug);
            }

        }

        public static int SpawnSinglePawn(this CorpseProduct CP, Pawn ParentPawn, IntVec3 SpawnPos, Map map, bool MyDebug=false)
        {
            string DebugStr = MyDebug ? "SpawnSinglePawn " : null;

            PawnGenOption PGO = CP.pawnKind.RandomElementByWeight(p => p.selectionWeight);
            if (PGO == null)
            {
                if (MyDebug) Log.Warning(DebugStr + " No PawnGenOption found");
                return 0;
            }

            PawnKindDef PKD = PGO.kind;
            Faction RandomFaction = CP.HasWeightedFaction ? CP.forcedFaction?.GetFaction(ParentPawn) : null;
            if (PKD == null)
            {
                if (MyDebug) Log.Warning(DebugStr + " No PKD found");
                return 0;
            }
            if (MyDebug) Log.Warning(DebugStr + "PKD:" + PKD.label + " faction:" + RandomFaction);

            bool wantNewBorn = Rand.Chance(CP.newBornChance);

            float combatPowerMultiplier = wantNewBorn ? CP.newBornCombatPowerRatio : 1;

            PawnGenerationRequest request =
                new PawnGenerationRequest(
                    kind: PKD, faction: RandomFaction, context: PawnGenerationContext.NonPlayer, tile: -1, forceGenerateNewPawn: false,
                    newborn: wantNewBorn, colonistRelationChanceFactor: 0, allowAddictions: false, allowFood: false, relationWithExtraPawnChanceFactor: 0
                );
            Pawn NewPawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(NewPawn, SpawnPos, map, WipeMode.Vanish);

            if (CP.HasRelevantManhunterChance && Rand.Chance(CP.manhunterChance))
                NewPawn.MakeManhunter(MyDebug);

            if(CP.inheritSettingsFromParent && NewPawn.InheritParentSettings(ParentPawn, RandomFaction) && MyDebug)
                Log.Warning(DebugStr + "applied parent settings");

            if(CP.setRelationsWithParent && NewPawn.AddParentRelations(ParentPawn) && MyDebug)
                Log.Warning(DebugStr + "added relations");

            return (int)(combatPowerMultiplier * NewPawn.kindDef.combatPower);
        }

        public static Faction GetFaction(this List<WeightedFaction> myFactions, Pawn ParentPawn)
        {
            WeightedFaction myWF = myFactions.RandomElementByWeightWithFallback(f => f.weight, null);
            if (myWF == null)
                return null;

            FactionDef factionDef = null;
            if (myWF.inheritFromParent)
            {
                return ParentPawn.Faction;
            }
            factionDef = myWF.factionDef;
            return Find.FactionManager.AllFactions.Where(F => F.def == factionDef).FirstOrFallback();
        }

        public static bool InheritParentSettings(this Pawn p, Pawn hatcheeParent, Faction hatcheeFaction)
        {
            if (p.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
            {
                p.playerSettings.AreaRestriction = hatcheeParent.playerSettings.AreaRestriction;
                return true;
            }
            return false;
        }

        public static bool AddParentRelations(this Pawn p, Pawn hatcheeParent)
        {
            if (!p.RaceProps.IsMechanoid)
            {
                p.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
                return true;
            }
            return false;
        }
    }
}
