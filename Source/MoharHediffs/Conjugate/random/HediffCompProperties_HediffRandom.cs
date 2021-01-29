using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HediffCompProperties_HediffRandom : HediffCompProperties
	{
        public BodyDef bodyDef;

        public List<HediffDef> hediffPool;
        public List<int> weights;

        public List<BodyPartDef> bodyPartDef;

        public bool debug = false;
        public bool hideBySeverity=true;

        public HediffCompProperties_HediffRandom()
		{
			this.compClass = typeof(HediffComp_HediffRandom);
		}
	}
}
