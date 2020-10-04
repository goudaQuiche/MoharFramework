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
        public static bool TryTendBleeding(this HediffComp_Regeneration RegenHComp, out bool Impossible)
        {
            if( RegenHComp.currentHediff == null ||
                !RegenHComp.currentHT.IsBloodLossTending() || 
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.IsTended()
                )
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryTendBleeding calling HungerAndRestTransaction", RegenHComp.MyDebug);

            float TendingQuality = RegenHComp.Props.BloodLossTendingParams.HealingQuality.RandomInRange;
            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.BloodLossTendingParams.HungerCost, 
                    RegenHComp.Props.BloodLossTendingParams.RestCost, 
                    TendingQuality/2,
                    RegenHComp.MyDebug)
                )
            {
                return false;
            }

            Tools.Warn("TryTendBleeding OK", RegenHComp.MyDebug);
            RegenHComp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            return true;
            //Tended(RegenHComp.Props.BleedingHediff.TendingQuality.RandomInRange);
        }

        public static bool TryTendRegularDisease(this HediffComp_Regeneration RegenHComp, out bool Impossible)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsRegularDiseaseTending() || 
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.IsTended())
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryTendRegularDisease calling HungerAndRestTransaction", RegenHComp.MyDebug);

            float TendingQuality = RegenHComp.Props.RegularDiseaseTendingParams.HealingQuality.RandomInRange;
            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.RegularDiseaseTendingParams.HungerCost, 
                    RegenHComp.Props.RegularDiseaseTendingParams.RestCost, 
                    TendingQuality/2,
                    RegenHComp.MyDebug)
                )
            {
                return false;
            }
                

            Tools.Warn("TryTendRegularDisease OK", RegenHComp.MyDebug);
            RegenHComp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            return true;
        }

        public static bool TryTendChronic(this HediffComp_Regeneration RegenHComp, out bool Impossible)
        {
            if (
                RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsChronicDiseaseTending() ||
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.IsTended()
                )
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryTendChronic calling HungerAndRestTransaction", RegenHComp.MyDebug);
            float TendingQuality = RegenHComp.Props.ChronicHediffTendingParams.HealingQuality.RandomInRange;
            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.ChronicHediffTendingParams.HungerCost,
                    RegenHComp.Props.ChronicHediffTendingParams.RestCost,
                    TendingQuality/2,
                    RegenHComp.MyDebug)
                )
            {
                return false;
            }

            Tools.Warn("TryTendChronic OK", RegenHComp.MyDebug);
            RegenHComp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            return true;
        }

        public static bool TryRegenInjury(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt, out bool Impossible)
        {
            if (RegenHComp.currentHediff == null || !RegenHComp.currentHT.IsInjuryRegeneration() || RegenHComp.currentHediff.Part == null ||
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (RegenHComp.currentHediff.Severity > 0)
            {
                Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryRegenInjury calling HungerAndRestTransaction", RegenHComp.MyDebug);

                float BPRMaxHealth = RegenHComp.currentHediff.Part.def.GetMaxHealth(RegenHComp.Pawn);
                float RegenQuantity = RegenHComp.Props.PhysicalInjuryRegenParams.HealingQuality.RandomInRange * BPRMaxHealth;
                float PawnBodyPartRatio = RegenQuantity / (float)RegenHComp.Pawn.MaxHitPoints;

                Tools.Warn(
                    RegenHComp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                    "BPRMaxHealth: " + BPRMaxHealth + "; " +
                    "RegenQuantity: " + RegenQuantity + "; " +
                    "PawnBodyPartRatio: " + PawnBodyPartRatio + "; "
                    , RegenHComp.MyDebug
                );

                if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.PhysicalInjuryRegenParams.HungerCost,
                    RegenHComp.Props.PhysicalInjuryRegenParams.RestCost,
                    PawnBodyPartRatio,
                    RegenHComp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                Tools.Warn("TryRegenInjury OK", RegenHComp.MyDebug);
                RegenHComp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryCureDisease(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt, out bool Impossible)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != MyDefs.HealingTask.DiseaseHealing ||
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (RegenHComp.currentHediff.Severity > 0)
            {
                Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryCureDisease calling HungerAndRestTransaction", RegenHComp.MyDebug);

                float RegenQuantity = RegenHComp.Props.DiseaseHediffRegenParams.HealingQuality.RandomInRange;
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

                Tools.Warn("TryCureDisease OK", RegenHComp.MyDebug);
                RegenHComp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryChemicalRemoval(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt, out bool Impossible)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != MyDefs.HealingTask.ChemicalRemoval ||
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (RegenHComp.currentHediff.Severity > 0)
            {
                Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryChemicalRemoval calling HungerAndRestTransaction", RegenHComp.MyDebug);

                float RegenQuantity = RegenHComp.Props.ChemicalHediffRegenParams.HealingQuality.RandomInRange;
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

                Tools.Warn("TryChemicalRemoval OK", RegenHComp.MyDebug);
                RegenHComp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryRemovePermanentInjury(this HediffComp_Regeneration RegenHComp, out bool DoneWithIt, out bool Impossible)
        {
            if (RegenHComp.currentHediff == null || RegenHComp.currentHT != MyDefs.HealingTask.PermanentInjuryRegeneration ||
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) ||
                RegenHComp.currentHediff.Severity == 0)
            {
                Impossible = true;
                DoneWithIt = false;
                return false;
            }

            Impossible = false;
            if (RegenHComp.currentHediff.Severity > 0)
            {
                Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryRemovePermanentInjury calling HungerAndRestTransaction", RegenHComp.MyDebug);

                float BPRMaxHealth = RegenHComp.currentHediff.Part.def.GetMaxHealth(RegenHComp.Pawn);

                float RegenQuantity = RegenHComp.Props.PermanentInjuryRegenParams.HealingQuality.RandomInRange * BPRMaxHealth;
                RegenQuantity = Math.Min(RegenQuantity, RegenHComp.currentHediff.Severity);
                float PawnBodyPartRatio = RegenQuantity / (float)RegenHComp.Pawn.MaxHitPoints;

                Tools.Warn(
                RegenHComp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                    "BPRMaxHealth: " + BPRMaxHealth + "; " +
                    "Pawn maxHP: " + RegenHComp.Pawn.MaxHitPoints +"; "+
                    "RegenQuantity: " + RegenQuantity + "; " +
                    "PawnBodyPartRatio: " + PawnBodyPartRatio + "; "
                    , RegenHComp.MyDebug
                );

                if (!RegenHComp.Pawn.HungerAndRestTransaction(
                    RegenHComp.Props.PermanentInjuryRegenParams.HungerCost,
                    RegenHComp.Props.PermanentInjuryRegenParams.RestCost,
                    PawnBodyPartRatio,
                    RegenHComp.MyDebug)
                )
                {
                    DoneWithIt = false;
                    return false;
                }

                Tools.Warn("TryRemovePermanentInjury OK", RegenHComp.MyDebug);
                RegenHComp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (RegenHComp.currentHediff.Severity <= 0) ? true : false;
                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool TryBodyPartFullRegeneration(this HediffComp_Regeneration comp, out bool Impossible)
        {
            Pawn p = comp.Pawn;
            BodyPartRecord BPR = comp.currentHediff.Part;

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
            float BPRMaxHealth = comp.Pawn.GetAllChildrenHealth(BPR);
            float TheoricSeverity = BPRMaxHealth * (1 - comp.Props.BodyPartRegenParams.BPMaxHealth);
            float PawnBodyPartRatio = BPRMaxHealth * comp.Props.BodyPartRegenParams.BPMaxHealth / comp.BodyPartsHealthSum;

            Tools.Warn(
                comp.Pawn.LabelShort + " - TryBodyPartFullRegeneration " +
                "PawnBPHealthSum: " + comp.BodyPartsHealthSum + "; " +
                "BPRMH: " + BPRMaxHealth + "; " +
                "TheoricSeverity: " + TheoricSeverity + "; " +
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

            MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(p, BPR, p.Position, p.Map);

            //p.RecursiveHediffRemoval(BPR, HediffDefOf.MissingBodyPart, comp.MyDebug);
            p.health.hediffSet.DirtyCache();
            /*
            if (RegenHComp.Effect_PartialHealthUponRegrow)
                p.RecursiveHediff(BPR, TheoricSeverity, HediffDefOf.SurgicalCut);
            */


            return true;
        }

        public static bool TryBodyPartRegeneration(this HediffComp_Regeneration RegenHComp, out bool Impossible)
        {
            if (
                RegenHComp.currentHediff == null || 
                RegenHComp.currentHediff.def != HediffDefOf.MissingBodyPart ||
                RegenHComp.currentHediff.Part == null ||
                RegenHComp.currentHT != MyDefs.HealingTask.BodyPartRegeneration ||
                !RegenHComp.Pawn.health.hediffSet.HasHediff(RegenHComp.currentHediff.def, RegenHComp.currentHediff.Part) 
                )
            {
                Impossible = true;
                return false;
            }

            Impossible = false;
            Tools.Warn(RegenHComp.Pawn.LabelShort + " - TryBodyPartRegeneration calling HungerAndRestTransaction", RegenHComp.MyDebug);

            BodyPartRecord BPR = RegenHComp.currentHediff.Part;
            float BPRMaxHealth = BPR.def.GetMaxHealth(RegenHComp.Pawn);
            float TheoricSeverity = BPRMaxHealth * (1 - RegenHComp.Props.BodyPartRegenParams.BPMaxHealth);

            float PawnBodyPartRatio = BPRMaxHealth * RegenHComp.Props.BodyPartRegenParams.BPMaxHealth / RegenHComp.BodyPartsHealthSum;

            Tools.Warn(
                RegenHComp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                "BPRMH: " + BPRMaxHealth + "+; " +
                "TheoricSeverity: " + TheoricSeverity + "+; " +
                "PawnBodyPartRatio: " + PawnBodyPartRatio + "+; "
                , RegenHComp.MyDebug
            );

            if (!RegenHComp.Pawn.HungerAndRestTransaction(
                RegenHComp.Props.BodyPartRegenParams.HungerCost,
                RegenHComp.Props.BodyPartRegenParams.RestCost,
                PawnBodyPartRatio,
                RegenHComp.MyDebug)
            )
            {
                return false;
            }

            Tools.Warn("TryBodyPartRegeneration OK", RegenHComp.MyDebug);
            RegenHComp.Pawn.health.RemoveHediff(RegenHComp.currentHediff);

            if (RegenHComp.Effect_PartialHealthUponRegrow)
            {
                Hediff BarelyAliveBP = HediffMaker.MakeHediff(HediffDefOf.SurgicalCut, RegenHComp.Pawn, BPR);
                BarelyAliveBP.Severity = TheoricSeverity;

                RegenHComp.Pawn.health.AddHediff(BarelyAliveBP, BPR);
            }

            //pawn.health.hediffSet.DirtyCache();
            return true;
        }
    }
}
