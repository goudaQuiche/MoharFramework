using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class StackCalculator
    {
        public static float CompletudeRatio(this Pawn pawn, bool myDebug = false)
        {
            float pawnWeightedMeat = pawn.GetStatValue(StatDefOf.MeatAmount);
            float pawnBasisMeat = pawn.def.statBases.GetStatValueFromList(StatDefOf.MeatAmount, 75);
            float result;
            if (pawnBasisMeat == 0)
                result = pawn.health.summaryHealth.SummaryHealthPercent;
            else
                result = pawnWeightedMeat / pawnBasisMeat;

            Tools.Warn("pawnWeightedMeat:" + pawnWeightedMeat + "; pawnBasisMeat:" + pawnBasisMeat + "=> ratio:" + result, myDebug);

            return result;
        }

        public static int ComputeSpawnCount(this HediffComp_RandySpawnUponDeath comp)
        {
            float answer = comp.NumberToSpawn;
            if (comp.WeightedSpawn)
                answer *= comp.Pawn.CompletudeRatio();

            return (int)answer;
        }
    }
}