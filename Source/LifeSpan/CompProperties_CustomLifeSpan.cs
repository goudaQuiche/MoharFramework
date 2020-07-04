using Verse;
using RimWorld;

namespace CustomLifeSpan
{
	public class CompProperties_Lifespan : CompProperties
	{
		public int lifeSpanTicks = 300;

        public bool spawnMeat = true;
        public bool relativeMeatAmount = true;
        public ThingDef meatToSpawn;
        public int fixedMeatAmount = 50;
        public IntRange meatPilesIntRange = new IntRange(3, 5);
        public float meatRadius = 3;

        public bool spawnThing = true;
        public ThingDef thingToSpawn;
        public bool relativeThingAmount = true;
        public IntRange thingPilesIntRange = new IntRange(3, 5);
        public int thingNumIfFullBody = 75;

        public bool tryToUnstack = true;
        public IntRange spawnIntervalRange = new IntRange(100, 100);
        public int spawnMaxAdjacent = -1;
        public bool spawnForbidden = true;
        public bool showMessageIfOwned = false;
        public bool inheritFaction = false;

        public bool spawnFilth = true;
        public ThingDef filthDef;
        public IntRange filthIntRange = new IntRange(3, 5);
        public float filthRadius = 3f;

        public bool spawnMote = true;
        //public ThingDef moteDef = ThingDef.Named("Mote_Smoke");
        public IntRange moteNumRange = new IntRange(3, 5);
        public float moteRadius = 3;
        public FloatRange moteScale = new FloatRange(1, 2.5f);

        public ThingDef moteDef = null;

        public bool debug = true;

        public CompProperties_Lifespan()
		{
			compClass = typeof(CompLifespan);
		}
	}
}