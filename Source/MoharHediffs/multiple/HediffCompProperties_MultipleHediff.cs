using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HediffCompProperties_MultipleHediff : HediffCompProperties
	{
        //conditionnal body
        public BodyDef bodyDef;
        //what
        public List<HediffAndBodyPart> hediffAndBodypart;

        public bool debug = false;

        public HediffCompProperties_MultipleHediff()
		{
			this.compClass = typeof(HediffComp_MultipleHediff);
		}
	}

    public class HediffAndBodyPart
    {
        public HediffDef hediff;
        public BodyPartDef bodyPart;
        public string bodyPartLabel;

        public bool prioritizeMissing = false;
        public bool allowMissing = true;
        public bool regenIfMissing = true;
        public bool allowAddedPart = true;
        public bool wholeBodyFallback = true;
    }
}
