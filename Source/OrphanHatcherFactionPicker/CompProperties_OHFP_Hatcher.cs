using Verse;
using System.Collections.Generic;

namespace OHFP
{
    public class CompProperties_OHFP_Hatcher : CompProperties
    {
        public float hatcherDaystoHatch = 1f;

        public List<PawnKindDef> hatcherPawnList;
        //public PawnKindDef hatcherPawn;

        public float colonyAdoptedChance = .5f;
        public float neutralAdoptedChance = .3f;
        public float enemyAdoptedChance = .2f;

        public float manhunterChance = .25f;
        public float newBornChance = 0f;
        public bool debug = false;

        public CompProperties_OHFP_Hatcher()
        {
            compClass = typeof(Comp_OHFP_Hatcher);
        }

    }
}