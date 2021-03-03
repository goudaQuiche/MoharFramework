using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class MentalStatePicker
    {
        public static float MSTotalWeight(this List<MentalStateOption> MSO)
        {
            float total = 0;

            for (int i = 0; i < MSO.Count; i++)
                total += MSO[i].weight;

            return total;
        }

        public static void ComputeRandomMentalState(this HediffComp_RandySpawnUponDeath comp)
        {
            if (!comp.ChosenItem.HasMentalStateParams)
                return;


            MentalStateDef msd = comp.GetWeightedRandomMentalState();
            if (msd == null)
            {
                if(comp.MyDebug)Log.Warning("ComputeRandomMentalState - found no MentalStateDef");
                return;
            }

            comp.RandomMS = msd;
            if (comp.MyDebug)Log.Warning("ComputeRandomFaction - found:" + comp.RandomFaction?.GetCallLabel());

        }

        public static MentalStateDef GetWeightedRandomMentalState(this HediffComp_RandySpawnUponDeath comp)
        {
            if (!(comp.HasChosenPawn && comp.ChosenItem.HasMentalStateParams))
                return null;

            List<MentalStateOption> MSOL = comp.ChosenItem.mentalState;

            float DiceThrow = Rand.Range(0, MSOL.MSTotalWeight());

            for (int i = 0; i < MSOL.Count; i++)
            {
                if ((DiceThrow -= MSOL[i].weight) < 0)
                {
                    if(comp.MyDebug)Log.Warning("GetWeightedRandomIndex : returning " + i);
                    return MSOL[i].mentalDef;
                }
            }

            if(comp.MyDebug)Log.Warning("GetWeightedRandomMentalState : failed to return proper index, returning null");

            return null;
        }
       
    }
}