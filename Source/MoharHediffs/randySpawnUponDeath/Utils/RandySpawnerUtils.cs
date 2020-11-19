using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public static class RandySpawnerUpoenDeathUtils
    {
        public static float ThingsTotalWeight(this HediffComp_RandySpawnUponDeath comp)
        {
            float total = 0;
            if (!comp.Props.settings.HasSomethingToSpawn)
                return 0;

            List<ThingSettings> TSList = comp.Props.settings.things;

            for (int i = 0; i < TSList.Count; i++)
                total += TSList[i].weight;

            return total;
        }

        public static float FactionTotalWeight(this List<FactionPickerParameters> FPP)
        {
            float total = 0;

            for (int i = 0; i < FPP.Count; i++)
                total += FPP[i].weight;

            return total;
        }

        public static void ComputeRandomFaction(this HediffComp_RandySpawnUponDeath comp)
        {
            if (!comp.ChosenItem.HasFactionParams)
                return;


            int FactionIndex = comp.GetWeightedRandomFaction();
            if (FactionIndex == -1)
            {
                Tools.Warn("ComputeRandomFaction - found no index", comp.MyDebug);
                return;
            }

            //comp.newBorn = comp.CurIP.factionPickerParameters[FactionIndex].newBorn;
            FactionPickerParameters FPP = comp.ChosenItem.factionPickerParameters[FactionIndex];
            if (comp.MyDebug)
                FPP.Dump();

            comp.randomlyChosenItemfaction = comp.GetFaction(FPP);
            Tools.Warn("ComputeRandomFaction - found:" + comp.randomlyChosenItemfaction?.GetCallLabel(), comp.MyDebug);

        }

        public static float CompletudeRatio(this Pawn pawn, bool myDebug=false)
        {
            float pawnWeightedMeat = pawn.GetStatValue(StatDefOf.MeatAmount);
            float pawnBasisMeat = pawn.def.statBases.GetStatValueFromList(StatDefOf.MeatAmount, 75);
            float result;
            if (pawnBasisMeat == 0)
                result = pawn.health.summaryHealth.SummaryHealthPercent;
            else
                result = pawnWeightedMeat / pawnBasisMeat;

            Tools.Warn("pawnWeightedMeat:" + pawnWeightedMeat + "; pawnBasisMeat:" + pawnBasisMeat + "=> ratio:"+result, myDebug);
            
            return result;
        }

        public static int ComputeSpawnCount(this HediffComp_RandySpawnUponDeath comp)
        {
            float answer = comp.NumberToSpawn;
            if (comp.WeightedSpawn)
                answer *= comp.Pawn.CompletudeRatio();

            return (int)answer;
        }

        public static int GetWeightedRandomIndex(this HediffComp_RandySpawnUponDeath comp)
        {
            float DiceThrow = Rand.Range(0, comp.ThingsTotalWeight());

            if (comp.Props.settings.HasSomethingToSpawn)
            {
                List<ThingSettings> TSList = comp.Props.settings.things;

                for (int i = 0; i < TSList.Count; i++)
                {
                    if ((DiceThrow -= TSList[i].weight) < 0)
                    {
                        Tools.Warn("GetWeightedRandomIndex : returning thing " + i, comp.MyDebug);
                        return i;
                    }
                }
            }

            Tools.Warn("GetWeightedRandomIndex : failed to return proper index, returning -1", comp.MyDebug);

            return -1;
        }

        public static int GetWeightedRandomFaction(this HediffComp_RandySpawnUponDeath comp)
        {
            if (!(comp.HasChosenPawn && comp.ChosenItem.HasFactionParams))
                return -1;

            List<FactionPickerParameters> RFP = comp.ChosenItem.factionPickerParameters;

            float DiceThrow = Rand.Range(0, RFP.FactionTotalWeight());

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
                return comp.ChosenItem.pawnKindToSpawn?.defaultFactionType ?? null;
            }

            return null;
        }

        public static void SetAge(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.ageRange != null)
                newPawn.ageTracker.AgeBiologicalTicks = 3600000 * comp.ChosenItem.ageRange.RandomInRange;
            else if (comp.ChosenItem.copyParentAge)
                newPawn.ageTracker.AgeBiologicalTicks = comp.parent.pawn.ageTracker.AgeBiologicalTicks;
        }

        public static void SetName(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParentName)
                newPawn.Name = comp.Pawn.Name;
        }
    }
}