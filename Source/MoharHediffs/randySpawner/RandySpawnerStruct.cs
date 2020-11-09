using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class ItemParameter
    {
        //Item - thing/pawnKind
        public ThingDef thingToSpawn = null; 
        public PawnKindDef pawnKindToSpawn = null;
        public bool NewBorn = false;

        public IntRange spawnCount = new IntRange(1, 1);

        public ThingDef filthDef = null;

        // Item faction
        public List<RandomFactionParameter> randomFactionParameters;

        //when
        public FloatRange daysB4Next = new FloatRange(1f, 2f);

        public float graceChance = 0f;
        public FloatRange graceDays = new FloatRange(1f, 2f);

        //random
        public float weight = 0f;

        //log
        public string spawnVerb = "delivery";

        public bool ThingSpawner => thingToSpawn != null && pawnKindToSpawn == null;
        public bool PawnSpawner => thingToSpawn == null && pawnKindToSpawn != null;
        public bool HasFactionParams => !randomFactionParameters.NullOrEmpty();
        public bool HasGraceChance => graceChance != 0;
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

        public void ComputeRandomParameters(out int calculatedTickUntilSpawn, out int calculatedGraceTicks, out int calculatedSpawnCount)
        {
            calculatedTickUntilSpawn = (int)(daysB4Next.RandomInRange * 60000);

            calculatedSpawnCount = spawnCount.RandomInRange;

            calculatedGraceTicks = 0;
            if (HasGraceChance && Rand.Chance(graceChance))
                calculatedGraceTicks = (int)(graceDays.RandomInRange * 60000);
        }

        
    }
}