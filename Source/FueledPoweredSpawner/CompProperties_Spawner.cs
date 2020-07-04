using Verse;

namespace FuPoSpa
{
    public class CompProperties_FuelPoweredSpawner : CompProperties
    {
        public ThingDef thingToSpawn;
        public int spawnCount = 1;

        public IntRange spawnIntervalRange = new IntRange(100, 100);
        public int spawnMaxAdjacent = -1;
        public bool spawnForbidden;

        public bool requiresPower;
        public bool requiresFuel;

        public bool writeTimeLeftToSpawn;
        public bool showMessageIfOwned;
        public string saveKeysPrefix;
        public bool inheritFaction;

        public bool debug = false;

        public CompProperties_FuelPoweredSpawner()
        {
            compClass = typeof(CompFuelPoweredSpawner);
        }
    }
}
