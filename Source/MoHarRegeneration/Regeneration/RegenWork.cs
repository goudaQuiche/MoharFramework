using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenWork
    {
        public static bool TryTendBleeding(this HediffComp_Regeneration RegenHComp)
        {
            if(RegenHComp.currentHediff == null || RegenHComp.currentHT != RegenParamsUtility.HealingTask.BleedingTending)
            {
                return false;
            }

            RegenHComp.currentHediff.Tended_NewTemp(RegenHComp.Props.BleedingHediff.TendingQuality.RandomInRange, 2f);
            return true;
            //Tended(RegenHComp.Props.BleedingHediff.TendingQuality.RandomInRange);
        }

        public static bool TryTendChronic(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != RegenParamsUtility.HealingTask.ChronicDisease)
            {
                return false;
            }

            RegenHComp.currentHediff.Tended_NewTemp(RegenHComp.Props.ChronicHediff.TendingQuality.RandomInRange, 2f);
            return true;
        }

        public static bool TryRegenInjury(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != RegenParamsUtility.HealingTask.InjuryRegeneration || RegenHComp.currentHediff.Part == null)
            {
                DoneWithIt = false;
                return false;
            }

            if(RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.PhysicalHediff.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryCureDisease(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != RegenParamsUtility.HealingTask.DiseaseHealing)
            {
                DoneWithIt = false;
                return false;
            }

            if (RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.DiseaseHediff.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryChemicalRemoval(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != RegenParamsUtility.HealingTask.ChemicalRemoval)
            {
                DoneWithIt = false;
                return false;
            }

            if (RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.ChemicalHediff.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
            }

            DoneWithIt = true;
            return true;
        }
    }
}
