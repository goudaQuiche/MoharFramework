using Verse;
using System.Collections.Generic;

namespace MoharHediffs
{
	public class HediffCompProperties_RandySpawnUponDeath : HediffCompProperties
	{
        public float requiredMinSeverity = 0f;

        public List<PawnOrThingParameter> pawnOrThingParameters;

        //how
        public int spawnMaxAdjacent = -1;
		public bool spawnForbidden = false;

        public bool debug = false;

        public HediffCompProperties_RandySpawnUponDeath()
		{
			this.compClass = typeof(HediffComp_RandySpawnUponDeath);
		}
	}
}
