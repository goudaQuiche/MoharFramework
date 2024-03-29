﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
//using AlienRace;

namespace MoHarRegeneration
{
    public static class MyDefs
    {
        public enum HealingTask
        {
            [Description("None")]
            None = 0,

            [Description("BloodLoss tending")]
            BloodLossTending = 1,
            [Description("Chronic disease tending")]
            ChronicDiseaseTending = 2,
            [Description("Regular disease tending")]
            RegularDiseaseTending = 3,

            [Description("Disease curing")]
            DiseaseHealing = 4,
            [Description("Chemical removal")]
            ChemicalRemoval = 5,
            [Description("Injury regeneration")]
            InjuryRegeneration = 6,
            
            [Description("Permanent injury regeneration")]
            PermanentInjuryRegeneration = 7,
            [Description("Body part regeneration")]
            BodyPartRegeneration = 8
        }

        public static bool IsBloodLossTending(this HealingTask HT)
        {
            return HT == HealingTask.BloodLossTending;
        }
        public static bool IsRegularDiseaseTending(this HealingTask HT)
        {
            return HT == HealingTask.RegularDiseaseTending;
        }
        public static bool IsChronicDiseaseTending(this HealingTask HT)
        {
            return HT == HealingTask.ChronicDiseaseTending;
        }

        public static bool IsInjuryRegeneration(this HealingTask HT)
        {
            return HT == HealingTask.InjuryRegeneration;
        }
        public static bool IsChemicalRemoval(this HealingTask HT)
        {
            return HT == HealingTask.ChemicalRemoval;
        }
        public static bool IsDiseaseHealing(this HealingTask HT)
        {
            return HT == HealingTask.DiseaseHealing;
        }

        public static bool IsPermanentInjuryRegeneration(this HealingTask HT)
        {
            return HT == HealingTask.PermanentInjuryRegeneration;
        }
        public static bool IsBodyPartRegeneration(this HealingTask HT)
        {
            return HT == HealingTask.BodyPartRegeneration;
        }

        public static readonly List<HealingTask> DefaultPriority = new List<HealingTask>
        {
            HealingTask.BloodLossTending,
            HealingTask.ChronicDiseaseTending,
            HealingTask.RegularDiseaseTending,

            HealingTask.DiseaseHealing,
            HealingTask.InjuryRegeneration,
            HealingTask.ChemicalRemoval,

            HealingTask.PermanentInjuryRegeneration,
            HealingTask.BodyPartRegeneration
        };

        public static string DumpDefaultPriority()
        {
            string answer = string.Empty;

            for (int i = 0; i < DefaultPriority.Count(); i++)
            {
                answer += ' ' + i.ToString("00") + " - " + DefaultPriority[i].DescriptionAttr() + ";";
            }

            return answer;
        }

        public static HealingParams GetParams(this HediffComp_Regeneration comp)
        {
            HealingTask curHT = comp.currentHT;
            if (comp.Effect_TendBleeding && curHT.IsBloodLossTending())
            {
                return comp.Props.BloodLossTendingParams;
            }
            // 01 chronic disease tending
            else if (comp.Effect_TendChronicDisease && curHT.IsChronicDiseaseTending())
            {
                return comp.Props.ChronicHediffTendingParams;
            }
            // 02 regular disease tending
            else if (comp.Effect_TendRegularDisease && curHT.IsRegularDiseaseTending())
            {
                return comp.Props.RegularDiseaseTendingParams;
            }
            // 03 regular injury
            else if (comp.Effect_RegeneratePhysicalInjuries && curHT.IsInjuryRegeneration())
            {
                return comp.Props.PhysicalInjuryRegenParams;
            }
            // 04 regular disease
            else if (comp.Effect_HealDiseases && curHT.IsDiseaseHealing())
            {
                return comp.Props.DiseaseHediffRegenParams;
            }
            // 05 chemicals
            else if (comp.Effect_RemoveChemicals && curHT.IsChemicalRemoval())
            {
                return comp.Props.ChemicalHediffRegenParams;
            }
            // 06 permanent
            else if (comp.Effect_RemoveScares && curHT.IsPermanentInjuryRegeneration())
            {
                return comp.Props.PermanentInjuryRegenParams;
            }
            // 07 Bodypart regen
            else if (comp.Effect_RegenerateBodyParts && curHT.IsBodyPartRegeneration())
            {
                return comp.Props.BodyPartRegenParams;
            }

            return null;
        }
    }
}
