using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HeDiffCompProperties_HediffExclusive : HediffCompProperties
	{
        //what
        public List<HediffDef> hediffToNullify;
        public List<string> hediffPatternToNullify;

        public HediffDef hediffToApply = null;

        public BodyDef bodyDef;

        public List<BodyDef> bodyDefWhiteList;
        public List<BodyDef> bodyDefBlackList;

        public BodyPartDef bodyPartDef;

        public bool debug = false;

        public HeDiffCompProperties_HediffExclusive()
		{
			this.compClass = typeof(HeDiffComp_HediffExclusive);
		}
	}
}
