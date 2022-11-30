using Verse;
using RimWorld;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace MoharComp
{
    public static class itemPicker
    {

        public static float GetTotalWeight(this List<DamageItem> hediffPool)
        {
            float answer = 0;
            foreach (DamageItem hi in hediffPool)
                answer += hi.weight;

            return answer;
        }

        public static DamageItem PickRandomWeightedItem(this List<DamageItem> damagePool)
        {

            float TotalWeight = damagePool.GetTotalWeight();
            float DiceThrow = Rand.Range(0, TotalWeight);

            for (int i = 0; i < damagePool.Count; i++)
                if ((DiceThrow -= damagePool[i].weight) < 0)
                    return damagePool[i];

            return null;
        }

        public static float GetTotalWeight(this List<HediffItem> hediffPool)
        {
            float answer = 0;
            foreach (HediffItem hi in hediffPool)
                answer += hi.weight;

            return answer;
        }

        public static HediffItem PickRandomWeightedItem(this List<HediffItem> damagePool)
        {

            float TotalWeight = damagePool.GetTotalWeight();
            float DiceThrow = Rand.Range(0, TotalWeight);

            for (int i = 0; i < damagePool.Count; i++)
                if ((DiceThrow -= damagePool[i].weight) < 0)
                    return damagePool[i];

            return null;
        }

    }
}