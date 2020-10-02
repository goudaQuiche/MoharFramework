using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class RegenParams
    {
        public IntRange PeriodBase = new IntRange(600, 1200);
        public FloatRange RegenerationBase = new FloatRange(.05f, .15f);
        public List<HediffDef> HediffDefs;

        public float HungerCostPerRegen = 1;
        public float RestCostPerRegen = 1;

        public byte Priority = 0;
    }
}
