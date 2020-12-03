using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HediffCompProperties_OnTheCarpet : HediffCompProperties
	{
        public List<HediffItemToRemove> hediffPool;
        public HediffKeepingCondition defaultCondition;

        public int checkPeriod = 120;
        public int graceTimeBase = 120;
        //public int falseChecksNumBeforeRemoval = 2;

        public bool debug = false;

        public bool HasDefaultCondition => defaultCondition != null;
        public bool HasHediffPool => !hediffPool.NullOrEmpty();
        public int ItemCount => HasHediffPool ? hediffPool.Count : 0;

        public HediffCompProperties_OnTheCarpet()
		{
			compClass = typeof(HediffComp_OnTheCarpet);
		}
	}
}
