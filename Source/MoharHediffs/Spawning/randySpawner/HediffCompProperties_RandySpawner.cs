using Verse;
using System.Collections.Generic;

namespace MoharHediffs
{
	public class HediffCompProperties_RandySpawner : HediffCompProperties
	{
        public List<ItemParameter> itemParameters;

        //how
        public int spawnMaxAdjacent = -1;
		public bool spawnForbidden = false;

        // condition
        public bool hungerRelative = false;
        public bool healthRelative = false;

        public bool logNextSpawn = false;

        public bool debug = false;

        public HediffCompProperties_RandySpawner()
		{
			this.compClass = typeof(HediffComp_RandySpawner);
		}
	}
}
