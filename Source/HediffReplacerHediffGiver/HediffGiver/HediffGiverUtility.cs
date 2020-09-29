using Verse;
using System.Collections.Generic;

namespace HEREHEGI
{
    public static class HediffGiverUtility
    {
        public static bool RetrieveHediffIndex(this HediffComp_DataHediff hdDH, HediffDef inputHediff, out int index)
        {
            List<HediffDef> Pool = hdDH.Props.InputHediffPool;

            for (int i=0; i < Pool.Count; i++)
            {
                if(inputHediff == Pool[i])
                {
                    index = i;
                    return true;
                }

            }

            index = 0;
            return false;
        }

        public static bool IsNullHediff(this HediffDef hediffDef)
        {
            return hediffDef == MyDefs.NullHediff;
        }

    }
}
