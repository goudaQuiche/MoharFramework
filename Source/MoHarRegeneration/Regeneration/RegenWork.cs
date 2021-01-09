using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenWork
    {
        public static bool TryTendBleeding(this HediffComp_Regeneration comp, out bool Impossible)
        {
            if( comp.currentHediff == null ||
                !comp.currentHT.IsBloodLossTending() || 
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.IsTended()
                )
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            Tools.Warn(comp.Pawn.LabelShort + " - TryTendBleeding calling HungerAndRestTransaction", comp.MyDebug);

            float TendingQuality = comp.Props.BloodLossTendingParams.HealingQuality.RandomInRange;
            if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.BloodLossTendingParams.HungerCost, 
                    comp.Props.BloodLossTendingParams.RestCost, 
                    TendingQuality/2,
                    comp.MyDebug)
                )
            {
                return false;
            }

            Tools.Warn("TryTendBleeding OK", comp.MyDebug);
            comp.currentHediff.Tended_NewTemp(TendingQuality, 2f);

            if (comp.HasLimits)
                comp.TreatmentPerformedQuality += TendingQuality;

            return true;
            //Tended(comp.Props.BleedingHediff.TendingQuality.RandomInRange);
        }

        public static bool TryTendRegularDisease(this HediffComp_Regeneration comp, out bool Impossible)
        {
            if (comp.currentHediff == null || !comp.currentHT.IsRegularDiseaseTending() || 
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.IsTended())
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            Tools.Warn(comp.Pawn.LabelShort + " - TryTendRegularDisease calling HungerAndRestTransaction", comp.MyDebug);

            float TendingQuality = comp.Props.RegularDiseaseTendingParams.HealingQuality.RandomInRange;
            if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.RegularDiseaseTendingParams.HungerCost, 
                    comp.Props.RegularDiseaseTendingParams.RestCost, 
                    TendingQuality/2,
                    comp.MyDebug)
                )
            {
                return false;
            }
                
            Tools.Warn("TryTendRegularDisease OK", comp.MyDebug);
            comp.currentHediff.Tended_NewTemp(TendingQuality, 2f);

            if (comp.HasLimits)
                comp.TreatmentPerformedQuality += TendingQuality;

            return true;
        }

        public static bool TryTendChronic(this HediffComp_Regeneration comp, out bool Impossible)
        {
            if (
                comp.currentHediff == null || !comp.currentHT.IsChronicDiseaseTending() ||
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.IsTended()
                )
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            Tools.Warn(comp.Pawn.LabelShort + " - TryTendChronic calling HungerAndRestTransaction", comp.MyDebug);
            float TendingQuality = comp.Props.ChronicHediffTendingParams.HealingQuality.RandomInRange;
            if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.ChronicHediffTendingParams.HungerCost,
                    comp.Props.ChronicHediffTendingParams.RestCost,
                    TendingQuality/2,
                    comp.MyDebug)
                )
            {
                return false;
            }

            Tools.Warn("TryTendChronic OK", comp.MyDebug);
            comp.currentHediff.Tended_NewTemp(TendingQuality, 2f);

            if (comp.HasLimits)
                comp.TreatmentPerformedQuality += TendingQuality;

            return true;
        }

        public static bool TryRegenInjury(this HediffComp_Regeneration comp, out bool DoneWithIt, out bool Impossible)
        {
            if (comp.currentHediff == null || !comp.currentHT.IsInjuryRegeneration() || comp.currentHediff.Part == null ||
            !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
            comp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (comp.currentHediff.Severity > 0)
            {
                Tools.Warn(comp.Pawn.LabelShort + " - TryRegenInjury calling HungerAndRestTransaction", comp.MyDebug);

                float BPRMaxHealth = comp.currentHediff.Part.def.GetMaxHealth(comp.Pawn);
                float RegenQuantity = comp.Props.PhysicalInjuryRegenParams.HealingQuality.RandomInRange * BPRMaxHealth;
                float PawnBodyPartRatio = RegenQuantity / (float)comp.Pawn.MaxHitPoints;

                Tools.Warn(
                    comp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                    "BPRMaxHealth: " + BPRMaxHealth + "; " +
                    "RegenQuantity: " + RegenQuantity + "; " +
                    "PawnBodyPartRatio: " + PawnBodyPartRatio + "; "
                    , comp.MyDebug
                );

                if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.PhysicalInjuryRegenParams.HungerCost,
                    comp.Props.PhysicalInjuryRegenParams.RestCost,
                    PawnBodyPartRatio,
                    comp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                Tools.Warn("TryRegenInjury OK", comp.MyDebug);
                comp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (comp.currentHediff.Severity <= 0) ? true : false;

                if (comp.HasLimits)
                    comp.TreatmentPerformedQuality += RegenQuantity;

                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryCureDisease(this HediffComp_Regeneration comp, out bool DoneWithIt, out bool Impossible)
        {
            if (comp.currentHediff == null || comp.currentHT != MyDefs.HealingTask.DiseaseHealing ||
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (comp.currentHediff.Severity > 0)
            {
                Tools.Warn(comp.Pawn.LabelShort + " - TryCureDisease calling HungerAndRestTransaction", comp.MyDebug);

                float RegenQuantity = comp.Props.DiseaseHediffRegenParams.HealingQuality.RandomInRange;
                if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.DiseaseHediffRegenParams.HungerCost,
                    comp.Props.DiseaseHediffRegenParams.RestCost,
                    RegenQuantity,
                    comp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                Tools.Warn("TryCureDisease OK", comp.MyDebug);
                comp.currentHediff.Severity -= RegenQuantity;
                // Immunity ?
                DoneWithIt = (comp.currentHediff.Severity <= 0) ? true : false;

                if (comp.HasLimits)
                    comp.TreatmentPerformedQuality += RegenQuantity;

                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryChemicalRemoval(this HediffComp_Regeneration comp, out bool DoneWithIt, out bool Impossible)
        {
            if (comp.currentHediff == null || comp.currentHT != MyDefs.HealingTask.ChemicalRemoval ||
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (comp.currentHediff.Severity > 0)
            {
                Tools.Warn(comp.Pawn.LabelShort + " - TryChemicalRemoval calling HungerAndRestTransaction", comp.MyDebug);

                float RegenQuantity = comp.Props.ChemicalHediffRegenParams.HealingQuality.RandomInRange;
                if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.ChemicalHediffRegenParams.HungerCost,
                    comp.Props.ChemicalHediffRegenParams.RestCost,
                    RegenQuantity,
                    comp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                Tools.Warn("TryChemicalRemoval OK", comp.MyDebug);
                comp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (comp.currentHediff.Severity <= 0) ? true : false;

                if (comp.HasLimits)
                    comp.TreatmentPerformedQuality += RegenQuantity;

                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryRemovePermanentInjury(this HediffComp_Regeneration comp, out bool DoneWithIt, out bool Impossible)
        {
            if (comp.currentHediff == null || comp.currentHT != MyDefs.HealingTask.PermanentInjuryRegeneration ||
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (comp.currentHediff.Severity > 0)
            {
                Tools.Warn(comp.Pawn.LabelShort + " - TryRemovePermanentInjury calling HungerAndRestTransaction", comp.MyDebug);

                float BPRMaxHealth = comp.currentHediff.Part.def.GetMaxHealth(comp.Pawn);

                float RegenQuantity = comp.Props.PermanentInjuryRegenParams.HealingQuality.RandomInRange * BPRMaxHealth;
                RegenQuantity = Math.Min(RegenQuantity, comp.currentHediff.Severity);
                float PawnBodyPartRatio = RegenQuantity / (float)comp.Pawn.MaxHitPoints;

                Tools.Warn(
                comp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                    "BPRMaxHealth: " + BPRMaxHealth + "; " +
                    "Pawn maxHP: " + comp.Pawn.MaxHitPoints +"; "+
                    "RegenQuantity: " + RegenQuantity + "; " +
                    "PawnBodyPartRatio: " + PawnBodyPartRatio + "; "
                    , comp.MyDebug
                );

                if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.PermanentInjuryRegenParams.HungerCost,
                    comp.Props.PermanentInjuryRegenParams.RestCost,
                    PawnBodyPartRatio,
                    comp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                Tools.Warn("TryRemovePermanentInjury OK", comp.MyDebug);
                comp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (comp.currentHediff.Severity <= 0) ? true : false;

                if (comp.HasLimits)
                    comp.TreatmentPerformedQuality += RegenQuantity;

                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryBodyPartRegeneration(this HediffComp_Regeneration comp, out bool Impossible)
        {
            if (
                comp.currentHediff == null || 
                comp.currentHediff.def != HediffDefOf.MissingBodyPart ||
                comp.currentHediff.Part == null ||
                comp.currentHT != MyDefs.HealingTask.BodyPartRegeneration ||
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) 
                )
            {
                Impossible = true;
                return false;
            }

            Impossible = false;
            Tools.Warn(comp.Pawn.LabelShort + " - TryBodyPartRegeneration calling HungerAndRestTransaction", comp.MyDebug);

            BodyPartRecord BPR = comp.currentHediff.Part;
            float BPRMaxHealth = BPR.def.GetMaxHealth(comp.Pawn);
            float TheoricSeverity = BPRMaxHealth * (1 - comp.Props.BodyPartRegenParams.BPMaxHealth);

            float PawnBodyPartRatio = BPRMaxHealth * comp.Props.BodyPartRegenParams.BPMaxHealth / comp.BodyPartsHealthSum;

            Tools.Warn(
                comp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                "BPRMH: " + BPRMaxHealth + "+; " +
                "TheoricSeverity: " + TheoricSeverity + "+; " +
                "PawnBodyPartRatio: " + PawnBodyPartRatio + "+; "
                , comp.MyDebug
            );

            if (!comp.Pawn.HungerAndRestTransaction(
                comp.Props.BodyPartRegenParams.HungerCost,
                comp.Props.BodyPartRegenParams.RestCost,
                PawnBodyPartRatio,
                comp.MyDebug)
            )
            {
                return false;
            }

            Tools.Warn("TryBodyPartRegeneration OK", comp.MyDebug);
            comp.Pawn.health.RemoveHediff(comp.currentHediff);

            // artificial, needs parameter ?
            if(comp.HasLimits)
                comp.TreatmentPerformedQuality += PawnBodyPartRatio * 10;

            if (comp.Effect_PartialHealthUponRegrow && !comp.Effect_RegenBodyPartChildrenAtOnce)
            {
                Hediff BarelyAliveBP = HediffMaker.MakeHediff(HediffDefOf.SurgicalCut, comp.Pawn, BPR);
                BarelyAliveBP.Severity = TheoricSeverity;

                comp.Pawn.health.AddHediff(BarelyAliveBP, BPR);
            }

            //pawn.health.hediffSet.DirtyCache();
            return true;
        }
    }
}
