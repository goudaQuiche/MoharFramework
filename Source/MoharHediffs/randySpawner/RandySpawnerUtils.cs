using Verse;
using RimWorld;
using System.Collections.Generic;

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
    }
}