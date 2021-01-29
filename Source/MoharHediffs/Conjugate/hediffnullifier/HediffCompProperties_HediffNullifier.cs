using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HediffCompProperties_HediffNullifier : HediffCompProperties
	{
        public int checkPeriod = 240;

        //what
        public List<HediffDef> hediffToNullify;
        public int limitedUsageNumber = -99;

        public List<BodyPartDef> RequiredBodyPart;

        public bool showMessage = false;
        public string nullifyKey = "";
        public bool concatUsageLimit = false;
        public string limitedKey = "";

        public HediffCompProperties_HediffNullifier()
		{
			this.compClass = typeof(HediffComp_HediffNullifier);
		}
	}
}
