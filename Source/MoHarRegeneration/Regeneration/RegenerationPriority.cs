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

        public List<MyDefs.HealingTask> CustomPriority = new List<MyDefs.HealingTask>();

        public readonly List<MyDefs.HealingTask> DefaultPriority = new List<MyDefs.HealingTask>
        {
            MyDefs.HealingTask.BloodLossTending,
            MyDefs.HealingTask.ChronicDiseaseTending,
            MyDefs.HealingTask.RegularDiseaseTending,

            MyDefs.HealingTask.DiseaseHealing,
            MyDefs.HealingTask.InjuryRegeneration,
            MyDefs.HealingTask.ChemicalRemoval,

            MyDefs.HealingTask.PermanentInjuryRegeneration,
            MyDefs.HealingTask.BodyPartRegeneration
        };
        
        public string DumpDefaultPriority()
        {
            string answer = string.Empty;

            for(int i=0; i< DefaultPriority.Count(); i++)
            {
                answer += ' '+ i.ToString("00") + " - " + DefaultPriority[i].DescriptionAttr() +";";
            }

            return answer;
        }

        public void CreatePriorities()
        {
            int maxIndex = 0;

            if (parent.Effect_TendBleeding)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.BloodLossTending, parent.Props.BloodLossTendingParams.Priority, maxIndex);
            if (parent.Effect_TendChronicDisease)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.ChronicDiseaseTending, parent.Props.ChronicHediffTendingParams.Priority, maxIndex);
            if (parent.Effect_TendRegularDisease)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.RegularDiseaseTending, parent.Props.RegularDiseaseTendingParams.Priority, maxIndex);


            if (parent.Effect_RegeneratePhysicalInjuries)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.InjuryRegeneration, parent.Props.PhysicalInjuryRegenParams.Priority, maxIndex);
            if(parent.Effect_HealDiseases)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.DiseaseHealing , parent.Props.DiseaseHediffRegenParams.Priority, maxIndex);
            if(parent.Effect_RemoveChemicals)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.ChemicalRemoval , parent.Props.ChemicalHediffRegenParams.Priority, maxIndex);


            if(parent.Effect_RemoveScares)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.PermanentInjuryRegeneration, parent.Props.PermanentInjuryRegenParams.Priority, maxIndex);

            if (parent.Effect_RegenerateBodyParts)
                maxIndex = AffectPriority(CustomPriority, MyDefs.HealingTask.BodyPartRegeneration, parent.Props.BodyPartRegenParams.Priority, maxIndex);

        }
        public int AffectPriority(List<MyDefs.HealingTask> Array, MyDefs.HealingTask value, int priority, int maxIndex)
        {
            maxIndex = Math.Max(priority, maxIndex);
            Array[priority] = value;
            return maxIndex;

        }

    }
}
