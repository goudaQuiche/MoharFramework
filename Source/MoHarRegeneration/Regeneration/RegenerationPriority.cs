using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class RegenerationPriority
    {
        /*
        enum CurrentHealingTask
        {
            [Description("None")]
            None = 0,
            [Description("InjuryRegeneration")]
            InjuryRegeneration = 1,
            [Description("ChemicalRemoval")]
            ChemicalRemoval = 2,
            [Description("DiseaseHealing")]
            DiseaseHealing = 3,
            [Description("PermanentInjuryRegeneration")]
            PermanentInjuryRegeneration = 4,
            [Description("BodyPartRegeneration")]
            BodyPartRegeneration = 5
        }
        */
        public readonly List<RegenerationUtility.HealingTask> DefaultPriority = new List<RegenerationUtility.HealingTask>
        {
            RegenerationUtility.HealingTask.BleedingTending,
            RegenerationUtility.HealingTask.InjuryRegeneration,
            RegenerationUtility.HealingTask.DiseaseHealing,
            RegenerationUtility.HealingTask.ChemicalRemoval,
            RegenerationUtility.HealingTask.PermanentInjuryRegeneration,
            RegenerationUtility.HealingTask.BodyPartRegeneration
        };
         
    }
}
