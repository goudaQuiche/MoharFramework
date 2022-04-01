using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class HealingWithMaxParams : HealingParams
    {
        public float BPMaxHealth = .35f;
        public bool RegenBodyPartChildrenAtOnce = false;

        public float parentMinHealthRequirement = 0;
        public HediffDef regrownHediff;

        public HediffDef prostheticHediff;
        public float prostheticMaxHealth = 1f;

        public string techHediffTag = string.Empty;

        public bool NeededParentHealthCheck => parentMinHealthRequirement > 0;
    }
}
