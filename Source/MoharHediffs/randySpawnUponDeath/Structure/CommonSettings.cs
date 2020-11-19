using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class CommonSettings
    {
        public IntRange spawnCount = new IntRange(1, 1);
        public bool weightedSpawnCount = false;
        public ThingDef filthDef = null;
    }
}