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
        public List<FactionPickerParameters> faction;
        //Misc
        public CopyPawnSettings copyParent;
        public RedressSettings redressNewPawn;

        //age
        public IntRange biologicalAgeRange = new IntRange(18, 80);
        public IntRange chronologicalAgeRange = new IntRange(0, 500);
        public bool newBorn = false;

        public ChannelColorCondition colorCondition;

        public CommonSettings specificSettings;
        public float weight = 1f;


        public bool IsThingSpawner => thingToSpawn != null;
        public bool IsParentCopier => IsCopier && copyParent.pawnKind;
        public bool IsPawnSpawner => pawnKindToSpawn != null || IsParentCopier;

        public string ItemDump => IsThingSpawner ? "thing:" + thingToSpawn : (IsPawnSpawner ? (IsCopier ? "parentCopier" : "pawn:" + pawnKindToSpawn) : "nothing?!");

        public bool HasFactionParams => !faction.NullOrEmpty();

        public bool IsCopier => copyParent != null;
        public bool IsRedresser => redressNewPawn != null;

        public bool HasSpecificSettings => specificSettings != null;
        public bool HasFilthSettings => HasSpecificSettings && specificSettings.filth != null && specificSettings.filth.filthDef != null;
        public bool HasStackSettings => HasSpecificSettings && specificSettings.stack != null;

        public bool HasColorCondition => colorCondition != null && !colorCondition.channelName.NullOrEmpty();
        /*
        public bool HasBiologicalAgeRange => biologicalAgeRange != null;
        public bool HasChronologicalAgeRange => chronologicalAgeRange != null;
        public bool HasAgeRange => HasBiologicalAgeRange || HasChronologicalAgeRange;
        */
    }
}