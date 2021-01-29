using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class RandomPicker
    {
        public static List<HediffItem> GetCompatibleItems(this HediffComp_AnotherRandom comp)
        {
            //HediffCondition defaultCondition = hcp.defaultCondition;

            List<HediffItem> answer = new List<HediffItem>();

            foreach (HediffItem hi in comp.Props.hediffPool)
            {
                HediffCondition calculatedHC = ConditionBuilder.GetDefaultPlusSpecificHediffCondition(comp.Props?.defaultCondition ?? null, hi?.specificCondition ?? null, comp.HighVerbosity);

                if (
                    calculatedHC.HasBodypartCondition ? calculatedHC.bodyPart.GetBPRFromHediffCondition(comp.Pawn, out _) : true &&
                    calculatedHC.HasPawnCondition ? calculatedHC.pawn.ValidateCompatibilityOfHediffWithPawn(comp.Pawn) : true
                    )
                    answer.Add(hi);
            }

            if (!answer.NullOrEmpty())
                return answer;

            return null;
        }

        public static List<HediffItem> GetRemainingItems(this List<HediffItem> hediffItems, List<HediffItem> AlreadyPickedItems)
        {
            return hediffItems.Where(hi => !AlreadyPickedItems.Contains(hi)).ToList();
        }

        public static float GetWeight(this List<HediffItem> HL)
        {
            float answer = 0;

            foreach(HediffItem hi in HL)
            {
                answer += hi.weight;
            }

            return answer;
        }

        public static HediffItem PickRandomWeightedItem(this List<HediffItem> HL, bool debug=false)
        {

            float TotalWeight = HL.GetWeight();

            float DiceThrow = Rand.Range(0, TotalWeight);

            for (int i = 0; i < HL.Count; i++)
            {
                if ((DiceThrow -= HL[i].weight) < 0)
                {
                    Tools.Warn("PickRandomWeightedItem : returning " + i, debug);
                    return HL[i];
                }
            }

            return null;
        }

    }
}
