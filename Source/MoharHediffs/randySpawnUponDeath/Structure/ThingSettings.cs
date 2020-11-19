using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class ThingSettings
    {
        public ThingDef thingToSpawn = null;
        public PawnKindDef pawnKindToSpawn = null;

        // Item faction
        public List<FactionPickerParameters> factionPickerParameters;
        //Misc
        public bool copyParentName = false;
        public bool copyParentPawnKind = false;

        public bool copyParentAge = false;
        public IntRange ageRange;// = new IntRange(16, 60);
        public bool newBorn = false;

        public CommonSettings specificSettings;
        public float weight = 1f;

        public bool IsThingSpawner => thingToSpawn != null;
        public bool IsPawnSpawner => pawnKindToSpawn != null || copyParentPawnKind;

        public bool HasFactionParams => !factionPickerParameters.NullOrEmpty();
        public bool HasSpecificSettings => specificSettings != null;
    }
}