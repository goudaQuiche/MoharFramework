using System.Collections.Generic;
using Verse;
using RimWorld;

namespace YAHA
{
    public class AssociatedHediffHistory : IExposable
    {
        public int appliedNum;
        public bool done;
        public int grace;
        public List<Hediff> appliedHediffs;

        public bool HasGraceTime => grace > 0;

        public AssociatedHediffHistory()
        {
            appliedNum = 0;
            done = false;
            grace = 0;

            appliedHediffs = new List<Hediff>();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref appliedNum, "appliedNum");
            Scribe_Values.Look(ref done, "done");
            Scribe_Values.Look(ref grace, "grace");

            Scribe_Collections.Look(ref appliedHediffs, "appliedHediffs", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && appliedHediffs == null)
            {
                appliedHediffs = new List<Hediff>();
            }
        }
    }
}
