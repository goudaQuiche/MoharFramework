using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class PawnOrThingParameter
    {
        //Item - thing/pawnKind
        public ThingDef thingToSpawn = null; 
        public PawnKindDef pawnKindToSpawn = null;

        public IntRange spawnCount = new IntRange(1, 1);

        public ThingDef filthDef = null;

        // Item faction
        public List<FactionPickerParameters> factionPickerParameters;

        //random
        public float weight = 0f;



        public bool ThingSpawner => thingToSpawn != null && pawnKindToSpawn == null;
        public bool PawnSpawner => thingToSpawn == null && pawnKindToSpawn != null;
        public bool HasFactionParams => !factionPickerParameters.NullOrEmpty();
        public bool HasFilth => filthDef != null;

        public void LogParams(bool myDebug = false)
        {
            Tools.Warn(
                "ThingSpawner:" + ThingSpawner + "; " + (ThingSpawner ? thingToSpawn.defName : "") +
                "PawnSpawner:" + PawnSpawner + "; " + (PawnSpawner ? pawnKindToSpawn.defName : "") +

                "spawnCount:" + spawnCount + "; " +

                "weight:" + weight + "; "
                , myDebug
            );
        }

        public void ComputeRandomParameters(out int calculatedSpawnCount)
        {
            calculatedSpawnCount = spawnCount.RandomInRange;
        }

        
    }
}