using Verse;
using System.Collections.Generic;

namespace MoharDamage
{
    public static class WeightedRandomOptionPicker
    {
        public static float GetWeight(this List<ExplosionSpawnItem> explosionSpawnItems)
        {
            float weight = 0;
            foreach (ExplosionSpawnItem item in explosionSpawnItems)
            {
                weight += item.weight;
            }
            return weight;
        }

        public static ExplosionSpawnItem PickRandomWeightedItem(this List<ExplosionSpawnItem> ESL)
        {
            if (ESL.NullOrEmpty())
                return null;

            float TotalWeight = ESL.GetWeight();
            float DiceThrow = Rand.Range(0, TotalWeight);

            for (int i = 0; i < ESL.Count; i++)
            {
                if ((DiceThrow -= ESL[i].weight) < 0)
                {
                    return ESL[i];
                }
            }
            return null;
        }
    }
}
