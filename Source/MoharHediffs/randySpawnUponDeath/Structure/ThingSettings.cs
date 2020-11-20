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
        public CopyPawnSettings copyParent;
        public RedressSettings redressNewPawn;

        public IntRange biologicalAgeRange;
        public IntRange chronologicalAgeRange;

        public bool newBorn = false;

        public CommonSettings specificSettings;
        public float weight = 1f;

        public bool IsThingSpawner => thingToSpawn != null;

        public bool IsPawnSpawner => pawnKindToSpawn != null || (IsCopier && copyParent.pawnKind);
        public bool HasFactionParams => !factionPickerParameters.NullOrEmpty();

        public bool IsCopier => copyParent != null;
        public bool IsRedresser => redressNewPawn != null;

        public bool HasSpecificSettings => specificSettings != null;
        public bool HasFilthSettings => HasSpecificSettings && specificSettings.filth != null && specificSettings.filth.filthDef != null;
        public bool HasStackSettings => HasSpecificSettings && specificSettings.stack != null;

        public bool HasBiologicalAgeRange => biologicalAgeRange != null;
        public bool HasChronologicalAgeRange => chronologicalAgeRange != null;
        public bool HasAgeRange => HasBiologicalAgeRange || HasChronologicalAgeRange;
    }
}