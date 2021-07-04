using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class FactionPickerUtils
    {
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
                if(comp.MyDebug)Log.Warning("ComputeRandomFaction - found no index");
                return;
            }

            //comp.newBorn = comp.CurIP.factionPickerParameters[FactionIndex].newBorn;
            FactionPickerParameters FPP = comp.ChosenItem.faction[FactionIndex];
            if (comp.MyDebug)
                FPP.Dump();

            comp.RandomFaction = comp.GetFaction(FPP);
            if(comp.MyDebug)Log.Warning("ComputeRandomFaction - found:" + comp.RandomFaction?.GetCallLabel());

        }

        public static int GetWeightedRandomFaction(this HediffComp_RandySpawnUponDeath comp)
        {
            if (!(comp.HasChosenPawn && comp.ChosenItem.HasFactionParams))
                return -1;

            List<FactionPickerParameters> RFP = comp.ChosenItem.faction;

            float DiceThrow = Rand.Range(0, RFP.FactionTotalWeight());

            for (int i = 0; i < RFP.Count; i++)
            {
                if ((DiceThrow -= RFP[i].weight) < 0)
                {
                    if(comp.MyDebug)Log.Warning("GetWeightedRandomIndex : returning " + i);
                    return i;
                }
            }

            if(comp.MyDebug)Log.Warning("GetWeightedRandomFaction : failed to return proper index, returning -1");

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

            if (FPP.HasInheritedFaction && p.Faction != null)
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

        
    }
}