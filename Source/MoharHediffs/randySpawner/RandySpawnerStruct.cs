using Verse;
using RimWorld;

namespace MoharHediffs
{
    public class ItemParameter
    {
        //Item - thing/pawnKind
        public ThingDef thingToSpawn = null; 
        public PawnKindDef pawnKindToSpawn = null;

        public IntRange spawnCount = new IntRange(1, 1);

        // Item faction
        public bool inheritedFaction = false;
        public FactionDef forcedFaction = null;

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
        public bool IsFactionForced => forcedFaction != null;

        public void LogParams(bool myDebug=false)
        {
            Tools.Warn(
                "ThingSpawner:" + ThingSpawner + "; " + (ThingSpawner ? thingToSpawn.defName : "") +
                "PawnSpawner:" + PawnSpawner + "; " + (PawnSpawner ? pawnKindToSpawn.defName : "") +

                "spawnCount:" + spawnCount + "; " +
                "inheritedFaction:" + inheritedFaction + "; " + (IsFactionForced ? pawnKindToSpawn.defName : "") +

                "weight:" + weight + "; "
                , myDebug
            );
        }

        public void ComputeRandomParameters(out int calculatedTickUntilSpawn, out int calculatedGraceTicks, out int calculatedSpawnCount)
        {
            calculatedTickUntilSpawn = (int)(daysB4Next.RandomInRange * 60000);

            calculatedSpawnCount = spawnCount.RandomInRange;

            calculatedGraceTicks = 0;
            if (Rand.Chance(graceChance))
                calculatedGraceTicks = (int)(daysB4Next.RandomInRange * 60000);
        }
    }
}