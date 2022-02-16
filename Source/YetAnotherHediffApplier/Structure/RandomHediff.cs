using Verse;
using System.Collections.Generic;
using System.Linq;

namespace YAHA
{
    public static class HediffPicker
    {
        public static float GetTotalWeight(this List<RandomHediffItem> randomHediffPool)
        {
            float answer = 0;

            foreach (RandomHediffItem rh in randomHediffPool)
            {
                answer += rh.weight;
            }

            return answer;
        }

        public static HediffItem PickRandomWeightedItem(this List<RandomHediffItem> HL, bool debug = false)
        {

            float TotalWeight = HL.GetTotalWeight();

            float DiceThrow = Rand.Range(0, TotalWeight);

            for (int i = 0; i < HL.Count; i++)
            {
                if ((DiceThrow -= HL[i].weight) < 0)
                {
                    if(debug) Log.Warning("PickRandomWeightedItem : returning " + i);
                    return HL[i];
                }
            }

            return null;
        }

    }
}
