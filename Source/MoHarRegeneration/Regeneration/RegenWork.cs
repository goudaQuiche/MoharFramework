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
            if(RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsBloodLossTending())
            {
                return false;
            }

            RegenHComp.currentHediff.Tended_NewTemp(RegenHComp.Props.BloodLossTendingParams.TendingQuality.RandomInRange, 2f);
            return true;
            //Tended(RegenHComp.Props.BleedingHediff.TendingQuality.RandomInRange);
        }

        public static bool TryTendRegularDisease(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsRegularDiseaseTending())
            {
                return false;
            }

            RegenHComp.currentHediff.Tended_NewTemp(RegenHComp.Props.RegularDiseaseTendingParams.TendingQuality.RandomInRange, 2f);
            return true;
            //Tended(RegenHComp.Props.BleedingHediff.TendingQuality.RandomInRange);
        }

        public static bool TryTendChronic(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsChronicDiseaseTending())
            {
                return false;
            }

            RegenHComp.currentHediff.Tended_NewTemp(RegenHComp.Props.ChronicHediffTendingParams.TendingQuality.RandomInRange, 2f);
            return true;
        }

        public static bool TryRegenInjury(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsInjuryRegeneration() || RegenHComp.currentHediff.Part == null)
            {
                DoneWithIt = false;
                return false;
            }

            if(RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.PhysicalInjuryRegenParams.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryCureDisease(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != MyDefs.HealingTask.DiseaseHealing)
            {
                DoneWithIt = false;
                return false;
            }

            if (RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.DiseaseHediffRegenParams.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryChemicalRemoval(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != MyDefs.HealingTask.ChemicalRemoval)
            {
                DoneWithIt = false;
                return false;
            }

            if (RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.ChemicalHediffRegenParams.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryRemovePermanentInjury(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != MyDefs.HealingTask.PermanentInjuryRegeneration)
            {
                DoneWithIt = false;
                return false;
            }

            if (RegenHComp.currentHediff.Severity > 0)
            {
                RegenHComp.currentHediff.Severity -= RegenHComp.Props.PermanentInjuryRegenParams.RegenerationBase.RandomInRange;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryBodyPartRegeneration(this HediffComp_Regeneration RegenHComp )
        {
            if (
                RegenHComp.currentHediff == null || 
                RegenHComp.currentHediff.def != HediffDefOf.MissingBodyPart ||
                RegenHComp.currentHediff.Part == null ||
                RegenHComp.currentHT != MyDefs.HealingTask.BodyPartRegeneration )
            {
                return false;
            }

            BodyPartRecord BPR = RegenHComp.currentHediff.Part;
            RegenHComp.Pawn.health.RemoveHediff(RegenHComp.currentHediff);
            Hediff BarelyAliveBP = HediffMaker.MakeHediff(HediffDefOf.SurgicalCut, RegenHComp.Pawn, BPR);
            RegenHComp.Pawn.health.AddHediff(BarelyAliveBP, BPR);
            //pawn.health.hediffSet.DirtyCache();
            return true;
        }
    }
}
