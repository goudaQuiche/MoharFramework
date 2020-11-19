using Verse;
using System.Collections.Generic;

namespace MoharHediffs
{
	public class HediffCompProperties_RandySpawnUponDeath : HediffCompProperties
	{
        public float requiredMinSeverity = 0f;

        public GeneralSettings settings;

        public bool destroyBodyUponDeath = false;
        public bool StripBeforeDeath = false;
        //public bool destroyWeaponUponDeath = false;

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
