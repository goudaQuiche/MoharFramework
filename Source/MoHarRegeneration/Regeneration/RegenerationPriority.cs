using RimWorld;
using System.Collections.Generic;
using System;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class RegenerationPriority
    {
        HediffComp_Regeneration parent;
        bool MyDebug => parent.Props.debug;

        public RegenerationPriority(HediffComp_Regeneration RegenHComp)
        {
            parent = RegenHComp;

            //CreatePriorities();
        }

        public List<RegenParamsUtility.HealingTask> CustomPriority = new List<RegenParamsUtility.HealingTask>();

        public readonly List<RegenParamsUtility.HealingTask> DefaultPriority = new List<RegenParamsUtility.HealingTask>
        {
            RegenParamsUtility.HealingTask.BleedingTending,
            RegenParamsUtility.HealingTask.ChronicDisease,

            RegenParamsUtility.HealingTask.InjuryRegeneration,
            RegenParamsUtility.HealingTask.DiseaseHealing,

            RegenParamsUtility.HealingTask.ChemicalRemoval,

            RegenParamsUtility.HealingTask.PermanentInjuryRegeneration,
            RegenParamsUtility.HealingTask.BodyPartRegeneration
        };
        
        public void CreatePriorities()
        {
            int maxIndex = 0;

            if (parent.Effect_TendBleeding)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.BleedingTending, parent.Props.BleedingHediff.Priority, maxIndex);

            if (parent.Effect_RegeneratePhysicalInjuries)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.InjuryRegeneration, parent.Props.PhysicalHediff.Priority, maxIndex);

            if(parent.Effect_HealDiseases)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.DiseaseHealing , parent.Props.DiseaseHediff.Priority, maxIndex);
            
            if(parent.Effect_RemoveChemicals)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.ChemicalRemoval , parent.Props.ChemicalHediff.Priority, maxIndex);

            if(parent.Effect_RemoveScares)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.PermanentInjuryRegeneration, parent.Props.PermanentInjury.Priority, maxIndex);

            if (parent.Effect_TendChronicDisease)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.ChronicDisease, parent.Props.ChronicHediff.Priority, maxIndex);

            if (parent.Effect_RegenerateBodyParts)
                maxIndex = AffectPriority(CustomPriority, RegenParamsUtility.HealingTask.BodyPartRegeneration, parent.Props.BodyPartRegeneration.Priority, maxIndex);

        }
        public int AffectPriority(List<RegenParamsUtility.HealingTask> Array, RegenParamsUtility.HealingTask value, int priority, int maxIndex)
        {
            maxIndex = Math.Max(priority, maxIndex);
            Array[priority] = value;
            return maxIndex;

        }

    }
}
