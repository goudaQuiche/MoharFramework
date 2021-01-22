using Verse;
using RimWorld;
using System.Collections.Generic;

namespace OHFP
{
    public class CompProperties_OHFP_Hatcher : CompProperties
    {
        public float hatcherDaystoHatch = 1f;

        public List<PawnKindDef> hatcherPawnList;
        //public PawnKindDef hatcherPawn;

        public List<RandomAdoption> randomAdoption;

        public FactionDef forcedFaction = null;

        public bool findRandomMotherIfNull = false;
        public bool findRandomFatherIfNull = false;

        public float manhunterChance = 0f;
        public float newBornChance = 0f;
        public bool debug = false;

        public bool HasForcedFaction => forcedFaction != null;
        public bool IsRandomlyAdopted => !randomAdoption.NullOrEmpty();

        public CompProperties_OHFP_Hatcher()
        {
            compClass = typeof(Comp_OHFP_Hatcher);
        }
    }

    public class RandomAdoption
    {
        public AdoptionType factionType;
        public float weight;
    }
    public enum AdoptionType
    {
        player,
        neutral,
        enemy
    }
}