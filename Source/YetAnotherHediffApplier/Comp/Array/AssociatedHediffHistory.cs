using System.Collections.Generic;
using Verse;
using RimWorld;

namespace YAHA
{
    public class AssociatedHediffHistory : IExposable
    {
        public int appliedNum;
        public bool done;
        public List<Hediff> appliedHediffs;

        public AssociatedHediffHistory()
        {
            appliedNum = 0;
            done = false;
            appliedHediffs = new List<Hediff>();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref appliedNum, "appliedNum");
            Scribe_Values.Look(ref done, "done");

            Scribe_Collections.Look(ref appliedHediffs, "appliedHediffs");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && appliedHediffs == null)
            {
                appliedHediffs = new List<Hediff>();
            }
        }
    }
}
