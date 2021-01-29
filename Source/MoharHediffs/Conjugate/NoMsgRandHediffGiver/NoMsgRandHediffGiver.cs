using Verse;
using System;

namespace MoharHediffs
{
    public class NoMsgRandHediffGiver : HediffGiver
    {
        public float mtbDays;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (Rand.MTBEventOccurs(mtbDays, 60000f, 60f))
                TryApply(pawn);
        }
    }
    

}
