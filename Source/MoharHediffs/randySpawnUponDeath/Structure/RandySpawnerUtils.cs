using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public static class RandySpawnerUpoenDeathUtils
    {
        public static float TotalWeight(this HediffComp_RandySpawnUponDeath comp)
        {
            float total = 0;

            List<PawnOrThingParameter> IPList = comp.Props.pawnOrThingParameters;

            for (int i = 0; i < IPList.Count; i++)
                total += IPList[i].weight;

            return total;
        }

        public static float TotalWeight(this List<FactionPickerParameters> FPP)
        {
            float total = 0;

            for (int i = 0; i < FPP.Count; i++)
                total += FPP[i].weight;

            return total;
        }

        public static void ComputeRandomFaction(this HediffComp_RandySpawnUponDeath comp)
        {
            if (comp.CurIP.HasFactionParams)
            {
                int FactionIndex = comp.GetWeightedRandomFaction();
                if (FactionIndex == -1)
                {
                    Tools.Warn("ComputeRandomFaction - found no index", comp.MyDebug);
                    return;
                }

                comp.newBorn = comp.CurIP.factionPickerParameters[FactionIndex].newBorn;
                FactionPickerParameters FPP = comp.CurIP.factionPickerParameters[FactionIndex];
                if (comp.MyDebug)
                    FPP.Dump();

                comp.randomlyChosenItemfaction = comp.GetFaction(FPP);
                Tools.Warn("ComputeRandomFaction - found:" + comp.randomlyChosenItemfaction?.GetCallLabel(), comp.MyDebug);
            }
        }

        public static int GetWeightedRandomIndex(this HediffComp_RandySpawnUponDeath comp)
        {
            float DiceThrow = Rand.Range(0, comp.TotalWeight());
            List<PawnOrThingParameter> IPList = comp.Props.pawnOrThingParameters;

            for (int i = 0; i < IPList.Count; i++)
            {
                if ((DiceThrow -= IPList[i].weight) < 0)
                {
                    Tools.Warn("GetWeightedRandomIndex : returning " + i, comp.MyDebug);
                    return i;
                }
            }

            Tools.Warn("GetWeightedRandomIndex : failed to return proper index, returning -1", comp.MyDebug);

            return -1;
        }

        public static int GetWeightedRandomFaction(this HediffComp_RandySpawnUponDeath comp)
        {
            if (!comp.HasValidIP || !comp.CurIP.HasFactionParams)
                return -1;

            List<FactionPickerParameters> RFP = comp.CurIP.factionPickerParameters;

            float DiceThrow = Rand.Range(0, RFP.TotalWeight());

            for (int i = 0; i < RFP.Count; i++)
            {
                if ((DiceThrow -= RFP[i].weight) < 0)
                {
                    Tools.Warn("GetWeightedRandomIndex : returning " + i, comp.MyDebug);
                    return i;
                }
            }

            Tools.Warn("GetWeightedRandomFaction : failed to return proper index, returning -1", comp.MyDebug);

            return -1;
        }

        public static void CheckProps(this HediffComp_RandySpawnUponDeath comp)
        {
            if (comp.Props.pawnOrThingParameters.NullOrEmpty())
                comp.BlockAndDestroy(comp.Pawn.Label + " props: no itemParameters - giving up", comp.MyDebug);

            // Logical checks
            for (int i = 0; i < comp.Props.pawnOrThingParameters.Count; i++)
            {
                PawnOrThingParameter IP = comp.Props.pawnOrThingParameters[i];
                if (IP.spawnCount.min > comp.spawnCountErrorLimit || IP.spawnCount.max > comp.spawnCountErrorLimit)
                {
                    comp.BlockAndDestroy(comp.Pawn.Label + " props: SpawnCount is too high: >" + comp.spawnCountErrorLimit, comp.MyDebug);
                    return;
                }

                if (!IP.ThingSpawner && !IP.PawnSpawner)
                {
                    comp.BlockAndDestroy(comp.Pawn.Label + " props: not a thing nor pawn spawner bc no def for either", comp.MyDebug);
                    return;
                }

                if (IP.HasFactionParams)
                {
                    foreach(FactionPickerParameters FPP in IP.factionPickerParameters)
                    {
                        if (!FPP.IsLegitRandomFactionParameter())
                        {
                            comp.BlockAndDestroy(comp.Pawn.Label + " faction props: invalid faction params", comp.MyDebug);
                            return;
                        }
                    }
                }
            }
        }

        public static void DumpProps(this HediffComp_RandySpawnUponDeath comp)
        {
            for (int i = 0; i < comp.Props.pawnOrThingParameters.Count; i++)
            {
                PawnOrThingParameter IP = comp.Props.pawnOrThingParameters[i];
                IP.LogParams(comp.MyDebug);
            }
        }

        public static bool TryFindSpawnCell(this HediffComp_RandySpawnUponDeath comp, Thing refThing, Map map, out IntVec3 result)
        {
            ThingDef thingDef = comp.CurIP.thingToSpawn;

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
                                        if (thing.def != thingDef || thing.stackCount > thingDef.stackLimit - comp.randomlyChosenQuantity)
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

        public static Faction GetFaction(this HediffComp_RandySpawnUponDeath comp, FactionPickerParameters FPP)
        {
            FactionDef fDef = comp.GetFactionDef(FPP);
            if (fDef == null)
                return null;
            return Find.FactionManager.AllFactions.Where(F => F.def == fDef).FirstOrFallback();
        }

        public static FactionDef GetFactionDef(this HediffComp_RandySpawnUponDeath comp, FactionPickerParameters FPP)
        {
            Pawn p = comp.Pawn;

            if (FPP.HasInheritedFaction)
                return p.Faction.def;
            else if (FPP.HasForcedFaction)
                return FPP.forcedFaction;
            else if (FPP.HasPlayerFaction)
                return Faction.OfPlayer.def;
            else if (FPP.HasNoFaction)
                return null;
            else if (FPP.HasDefaultPawnKindFaction)
            {
                return comp.CurIP.pawnKindToSpawn?.defaultFactionType ?? null;
            }

            return null;
        }
    }
}