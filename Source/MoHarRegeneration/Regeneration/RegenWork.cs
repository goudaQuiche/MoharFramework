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

            float TendingQuality = RegenHComp.Props.BloodLossTendingParams.TendingQuality.RandomInRange;
            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.BloodLossTendingParams.HungerCost, 
                    RegenHComp.Props.BloodLossTendingParams.RestCost, 
                    TendingQuality,
                    RegenHComp.MyDebug)
                )
            {
                return false;
            }

            RegenHComp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            return true;
            //Tended(RegenHComp.Props.BleedingHediff.TendingQuality.RandomInRange);
        }

        public static bool TryTendRegularDisease(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsRegularDiseaseTending())
            {
                return false;
            }

            float TendingQuality = RegenHComp.Props.RegularDiseaseTendingParams.TendingQuality.RandomInRange;
            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.RegularDiseaseTendingParams.HungerCost, 
                    RegenHComp.Props.RegularDiseaseTendingParams.RestCost, 
                    TendingQuality,
                    RegenHComp.MyDebug)
                )
                return false;

            RegenHComp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            return true;
        }

        public static bool TryTendChronic(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsChronicDiseaseTending())
            {
                return false;
            }

            float TendingQuality = RegenHComp.Props.ChronicHediffTendingParams.TendingQuality.RandomInRange;
            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.ChronicHediffTendingParams.HungerCost,
                    RegenHComp.Props.ChronicHediffTendingParams.RestCost,
                    TendingQuality,
                    RegenHComp.MyDebug)
                )
                return false;

            RegenHComp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
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
                float BPRMaxHealth = RegenHComp.currentHediff.Part.def.GetMaxHealth(RegenHComp.Pawn);

                float RegenQuantity = RegenHComp.Props.PhysicalInjuryRegenParams.RegenerationBase.RandomInRange * BPRMaxHealth;
                if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.PhysicalInjuryRegenParams.HungerCost,
                    RegenHComp.Props.PhysicalInjuryRegenParams.RestCost,
                    RegenQuantity,
                    RegenHComp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                RegenHComp.currentHediff.Severity -= RegenQuantity;
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
                float RegenQuantity = RegenHComp.Props.DiseaseHediffRegenParams.RegenerationBase.RandomInRange;
                if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.DiseaseHediffRegenParams.HungerCost,
                    RegenHComp.Props.DiseaseHediffRegenParams.RestCost,
                    RegenQuantity,
                    RegenHComp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }
                RegenHComp.currentHediff.Severity -= RegenQuantity;
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
                float RegenQuantity = RegenHComp.Props.ChemicalHediffRegenParams.RegenerationBase.RandomInRange;
                if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.ChemicalHediffRegenParams.HungerCost,
                    RegenHComp.Props.ChemicalHediffRegenParams.RestCost,
                    RegenQuantity,
                    RegenHComp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                RegenHComp.currentHediff.Severity -= RegenQuantity;
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
                float BPRMaxHealth = RegenHComp.currentHediff.Part.def.GetMaxHealth(RegenHComp.Pawn);

                float RegenQuantity = RegenHComp.Props.PermanentInjuryRegenParams.RegenerationBase.RandomInRange * BPRMaxHealth;
                if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.PermanentInjuryRegenParams.HungerCost,
                    RegenHComp.Props.PermanentInjuryRegenParams.RestCost,
                    RegenQuantity,
                    RegenHComp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                RegenHComp.currentHediff.Severity -= RegenQuantity;
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
            float BPRMaxHealth = BPR.def.GetMaxHealth(RegenHComp.Pawn);
            float TheoricSeverity = BPRMaxHealth * (1 - RegenHComp.Props.BodyPartRegenParams.BPMaxHealth);
            float PawnBodyPartRatio = BPRMaxHealth * RegenHComp.Props.BodyPartRegenParams.BPMaxHealth / (float)RegenHComp.Pawn.MaxHitPoints;

            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                RegenHComp.Props.BodyPartRegenParams.HungerCost,
                RegenHComp.Props.BodyPartRegenParams.RestCost,
                PawnBodyPartRatio,
                RegenHComp.MyDebug)
            )
            {
                return false;
            }

            RegenHComp.Pawn.health.RemoveHediff(RegenHComp.currentHediff);
            
            Hediff BarelyAliveBP = HediffMaker.MakeHediff(HediffDefOf.SurgicalCut, RegenHComp.Pawn, BPR);
            BarelyAliveBP.Severity = TheoricSeverity;

            RegenHComp.Pawn.health.AddHediff(BarelyAliveBP, BPR);
            //pawn.health.hediffSet.DirtyCache();
            return true;
        }
    }
}
