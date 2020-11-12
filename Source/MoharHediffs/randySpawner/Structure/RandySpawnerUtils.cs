using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public static class RandySpawnerUtils
    {
        public static float TotalWeight(this HediffComp_RandySpawner comp)
        {
            float total = 0;

            List<ItemParameter> IPList = comp.Props.itemParameters;

            for (int i = 0; i < IPList.Count; i++)
                total += IPList[i].weight;

            return total;
        }

        public static float TotalWeight(this List<RandomFactionParameter> RFP)
        {
            float total = 0;

            for (int i = 0; i < RFP.Count; i++)
                total += RFP[i].weight;

            return total;
        }

        public static void ComputeRandomFaction(this HediffComp_RandySpawner comp)
        {
            if (comp.CurIP.HasFactionParams)
            {
                int FactionIndex = comp.GetWeightedRandomFaction();
                if (FactionIndex == -1)
                {
                    Tools.Warn("ComputeRandomFaction - found no index", comp.MyDebug);
                    return;
                }

                comp.newBorn = comp.CurIP.randomFactionParameters[FactionIndex].newBorn;
                RandomFactionParameter RFP = comp.CurIP.randomFactionParameters[FactionIndex];
                comp.Itemfaction = comp.GetFaction(RFP);
                Tools.Warn("ComputeRandomFaction - found:" + comp.Itemfaction?.GetCallLabel(), comp.MyDebug);
            }
        }

        public static int GetWeightedRandomIndex(this HediffComp_RandySpawner comp)
        {
            float DiceThrow = Rand.Range(0, comp.TotalWeight());
            List<ItemParameter> IPList = comp.Props.itemParameters;

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

        public static int GetWeightedRandomFaction(this HediffComp_RandySpawner comp)
        {
            if (!comp.HasValidIP || !comp.CurIP.HasFactionParams)
                return -1;

            List<RandomFactionParameter> RFP = comp.CurIP.randomFactionParameters;

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

        public static bool SetRequirementGraceTicks(this HediffComp_RandySpawner comp)
        {
            bool food = comp.RequiresFood;
            bool health = comp.RequiresHealth;
            if (food || health)
            {
                if (food)
                    comp.hungerReset++;
                else
                    comp.healthReset++;

                if(comp.HasValidIP)
                    comp.graceTicks = (int)(comp.CurIP.graceDays.RandomInRange * 60000);
                return true;
            }

            comp.hungerReset = comp.healthReset = 0;
            return false;
        }

        public static void CheckProps(this HediffComp_RandySpawner comp)
        {
            if (comp.Props.itemParameters.NullOrEmpty())
                comp.BlockAndDestroy(comp.Pawn.Label + " props: no itemParameters - giving up", comp.MyDebug);

            // Logical checks
            for (int i = 0; i < comp.Props.itemParameters.Count; i++)
            {
                ItemParameter IP = comp.Props.itemParameters[i];
                if (IP.spawnCount.min > comp.spawnCountErrorLimit || IP.spawnCount.max > comp.spawnCountErrorLimit)
                {
                    comp.BlockAndDestroy(comp.Pawn.Label + " props: SpawnCount is too high: >" + comp.spawnCountErrorLimit, comp.MyDebug);
                    return;
                }

                if (IP.daysB4Next.min < comp.minDaysB4NextErrorLimit)
                {
                    comp.BlockAndDestroy(comp.Pawn.Label + " props: minDaysB4Next is too low: " + IP.daysB4Next.min + "<" + comp.minDaysB4NextErrorLimit, comp.MyDebug);
                    return;
                }

                if (!IP.ThingSpawner && !IP.PawnSpawner)
                {
                    comp.BlockAndDestroy(comp.Pawn.Label + " props: not a thing nor pawn spawner bc no def for either", comp.MyDebug);
                    return;
                }

                if (IP.HasFactionParams)
                {
                    foreach(RandomFactionParameter FRP in IP.randomFactionParameters)
                    {
                        if (!FRP.IsLegitRandomFactionParameter())
                        {
                            comp.BlockAndDestroy(comp.Pawn.Label + " faction props: invalid faction params", comp.MyDebug);
                            return;
                        }
                    }
                }
            }
        }

        public static void DumpProps(this HediffComp_RandySpawner comp)
        {
            Tools.Warn(
            "hungerRelative: " + comp.Props.hungerRelative + "; " +
            "healthRelative: " + comp.Props.healthRelative + "; "
            , comp.MyDebug);

            for (int i = 0; i < comp.Props.itemParameters.Count; i++)
            {
                ItemParameter IP = comp.Props.itemParameters[i];
                IP.LogParams(comp.MyDebug);
            }
        }

        public static bool TryFindSpawnCell(this HediffComp_RandySpawner comp, out IntVec3 result)
        {
            Pawn p = comp.Pawn;
            ThingDef thingDef = comp.CurIP.thingToSpawn;

            if (p.Negligeable())
            {
                result = IntVec3.Invalid;
                Tools.Warn("TryFindSpawnCell Null - pawn null", comp.MyDebug);
                return false;
            }

            foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(p).InRandomOrder(null))
            {
                if (current.Walkable(p.Map))
                {
                    Building edifice = current.GetEdifice(p.Map);
                    if (edifice == null || !thingDef.IsEdifice())
                    {
                        if (!(edifice is Building_Door building_Door) || building_Door.FreePassage)
                        {
                            if (GenSight.LineOfSight(p.Position, current, p.Map, false, null, 0, 0))
                            {
                                bool flag = false;
                                List<Thing> thingList = current.GetThingList(p.Map);
                                for (int i = 0; i < thingList.Count; i++)
                                {
                                    Thing thing = thingList[i];
                                    if (thing.def.category == ThingCategory.Item)
                                        if (thing.def != thingDef || thing.stackCount > thingDef.stackLimit - comp.calculatedQuantity)
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

        public static Faction GetFaction(this HediffComp_RandySpawner comp, RandomFactionParameter RFP)
        {
            FactionDef fDef = comp.GetFactionDef(RFP);
            if (fDef == null)
                return null;
            return Find.FactionManager.AllFactions.Where(F => F.def == fDef).FirstOrFallback();
        }

        public static FactionDef GetFactionDef(this HediffComp_RandySpawner comp, RandomFactionParameter RFP)
        {
            Pawn p = comp.Pawn;

            if (RFP.HasInheritedFaction)
                return p.Faction.def;
            else if (RFP.HasForcedFaction)
                return RFP.forcedFaction;
            else if (RFP.HasPlayerFaction)
                return Faction.OfPlayerSilentFail.def;
            else if (RFP.HasNoFaction)
                return null;
            else if (RFP.HasDefaultPawnKindFaction)
            {
                return comp.CurIP.pawnKindToSpawn?.defaultFactionType ?? null;
            }

            return null;
        }
    }
}