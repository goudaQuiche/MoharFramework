using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class ThingRequirementSettings
    {
        // requires Building On map
        public ThingDef thingDef;
        public FloatRange distance = new FloatRange(0, 300);

        // spawn Method
        public bool spawnClose = false;
        // vanilla building only
        public bool spawnInside = false;

        // conditions
        // building or pawn
        public bool sameFaction = true;
        // building
        public bool needsFueled = false;
        public bool needsPowered = false;

        public bool HasThingDef => thingDef != null;
        public bool HasContainerSpawn => HasThingDef && spawnInside;
        public bool HasCustomSpawn => HasThingDef && (spawnClose || spawnInside);
    }
}