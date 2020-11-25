using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HediffCompProperties_AnotherRandom : HediffCompProperties
	{
        public HediffCondition conditionsToApplyHediffs;

        public HediffCondition defaultCondition;
        public List<HediffItem> hediffPool;

        public IntRange hediffToApplyNumRange = new IntRange(1, 1);
        public bool excludePickedItems = true;
        public bool excludeRandomlyNotApplied = false;

        public bool debug = false;
        public int verbosity = 1;

        public bool HasConditionsToApplyHediffs => conditionsToApplyHediffs != null;
        public bool HasDefaultCondition => defaultCondition != null;

        public bool HasHediffPool => !hediffPool.NullOrEmpty();

        public HediffCompProperties_AnotherRandom()
		{
			this.compClass = typeof(HediffComp_AnotherRandom);
		}
	}
}
