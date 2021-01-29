using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class RequirementSettings
    {
        public List<HediffRequirementSettings> hediff;
        public List<ThingRequirementSettings> thing;

        public bool HasHediffRequirement => !hediff.NullOrEmpty() && hediff.Any(h => h.HasHediffDef);
        public bool HasThingRequirement => !thing.NullOrEmpty() && thing.Any(t => t.HasThingDef);

        public bool HasAtLeastOneRequirementSetting => HasHediffRequirement || HasThingRequirement;

        public bool HasContainerSpawn => HasThingRequirement && thing.Any(t => t.HasContainerSpawn);
    }
}