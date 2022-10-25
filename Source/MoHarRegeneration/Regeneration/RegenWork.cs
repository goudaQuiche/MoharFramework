using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using Verse;
//using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenWork
    {
        public static bool TryTendBleeding(this HediffComp_Regeneration comp, out bool Impossible)
        {
            if (comp.currentHediff == null ||
                !comp.currentHT.IsBloodLossTending() ||
                !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part) ||
                comp.currentHediff.IsTended()
                )
            {
                Impossible = true;
                return false;
            }
            Impossible = false;
            if (comp.MyDebug)
                Log.Warning(comp.Pawn.LabelShort + " - TryTendBleeding calling HungerAndRestTransaction");

            float TendingQuality = comp.Props.BloodLossTendingParams.HealingQuality.RandomInRange;
            if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.BloodLossTendingParams.HungerCost,
                    comp.Props.BloodLossTendingParams.RestCost,
                    TendingQuality / 2,
                    comp.MyDebug)
                )
            {
                return false;
            }

            if (comp.MyDebug)
                Log.Warning("TryTendBleeding OK");
            //comp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            comp.currentHediff.Tended(TendingQuality, 2f);

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
            if (comp.MyDebug)
                Log.Warning(comp.Pawn.LabelShort + " - TryTendRegularDisease calling HungerAndRestTransaction");

            float TendingQuality = comp.Props.RegularDiseaseTendingParams.HealingQuality.RandomInRange;
            if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.RegularDiseaseTendingParams.HungerCost,
                    comp.Props.RegularDiseaseTendingParams.RestCost,
                    TendingQuality / 2,
                    comp.MyDebug)
                )
            {
                return false;
            }

            if (comp.MyDebug)
                Log.Warning("TryTendRegularDisease OK");
            //comp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            comp.currentHediff.Tended(TendingQuality, 2f);

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
            if (comp.MyDebug)
                Log.Warning(comp.Pawn.LabelShort + " - TryTendChronic calling HungerAndRestTransaction");
            float TendingQuality = comp.Props.ChronicHediffTendingParams.HealingQuality.RandomInRange;
            if (!comp.Pawn.HungerAndRestTransaction(
                    comp.Props.ChronicHediffTendingParams.HungerCost,
                    comp.Props.ChronicHediffTendingParams.RestCost,
                    TendingQuality / 2,
                    comp.MyDebug)
                )
            {
                return false;
            }

            if (comp.MyDebug)
                Log.Warning("TryTendChronic OK");
            //comp.currentHediff.Tended_NewTemp(TendingQuality, 2f);
            comp.currentHediff.Tended(TendingQuality, 2f);

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
                if (comp.MyDebug)
                    Log.Warning(comp.Pawn.LabelShort + " - TryRegenInjury calling HungerAndRestTransaction");

                float BPRMaxHealth = comp.currentHediff.Part.def.GetMaxHealth(comp.Pawn);
                float RegenQuantity = comp.Props.PhysicalInjuryRegenParams.HealingQuality.RandomInRange * BPRMaxHealth;
                float PawnBodyPartRatio = RegenQuantity / (float)comp.Pawn.MaxHitPoints;

                if (comp.MyDebug)
                    Log.Warning(
                        comp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                    "BPRMaxHealth: " + BPRMaxHealth + "; " +
                    "RegenQuantity: " + RegenQuantity + "; " +
                    "PawnBodyPartRatio: " + PawnBodyPartRatio + "; "
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

                if (comp.MyDebug)
                    Log.Warning("TryRegenInjury OK");
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
                if (comp.MyDebug)
                    Log.Warning(comp.Pawn.LabelShort + " - TryCureDisease calling HungerAndRestTransaction");

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
                if(comp.MyDebug)
                    Tools.Warn("TryCureDisease OK");

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
                if (comp.MyDebug)
                    Log.Warning(comp.Pawn.LabelShort + " - TryChemicalRemoval calling HungerAndRestTransaction");

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

                if (comp.MyDebug)
                    Log.Warning("TryChemicalRemoval OK");

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
                if (comp.MyDebug)
                    Log.Warning(comp.Pawn.LabelShort + " - TryRemovePermanentInjury calling HungerAndRestTransaction");

                float BPRMaxHealth = comp.currentHediff.Part.def.GetMaxHealth(comp.Pawn);

                float RegenQuantity = comp.Props.PermanentInjuryRegenParams.HealingQuality.RandomInRange * BPRMaxHealth;
                RegenQuantity = Math.Min(RegenQuantity, comp.currentHediff.Severity);
                float PawnBodyPartRatio = RegenQuantity / (float)comp.Pawn.MaxHitPoints;

                if (comp.MyDebug)
                    Log.Warning(
                comp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                    "BPRMaxHealth: " + BPRMaxHealth + "; " +
                    "Pawn maxHP: " + comp.Pawn.MaxHitPoints + "; " +
                    "RegenQuantity: " + RegenQuantity + "; " +
                    "PawnBodyPartRatio: " + PawnBodyPartRatio + "; "
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

                if (comp.MyDebug)
                    Log.Warning("TryRemovePermanentInjury OK");

                comp.currentHediff.Severity -= RegenQuantity;
                DoneWithIt = (comp.currentHediff.Severity <= 0) ? true : false;

                if (comp.HasLimits)
                    comp.TreatmentPerformedQuality += RegenQuantity;

                return true;
            }

            DoneWithIt = true;
            return true;
        }

        public static bool ParentBPHealthNeededCheck(this HediffComp_Regeneration comp)
        {
            return comp.Props.BodyPartRegenParams.NeededParentHealthCheck; 
        }

        public static bool ParentBPHealthCheck(this HediffComp_Regeneration comp)
        {
            if (comp.currentHediff.Part.parent == null)
                return false;

            float maxH = comp.currentHediff.Part.parent.def.GetMaxHealth(comp.Pawn);
            float curH = comp.Pawn.health.hediffSet.GetPartHealth(comp.currentHediff.Part);

            if (maxH == 0)
                return false;

            return (curH / maxH) > comp.Props.BodyPartRegenParams.parentMinHealthRequirement;
        }

        public static bool TryBodyPartRegeneration(this HediffComp_Regeneration comp, out bool Impossible)
        {
            if (
                comp.currentHediff == null
                || comp.currentHediff.def != HediffDefOf.MissingBodyPart
                || comp.currentHediff.Part == null
                || comp.currentHT != MyDefs.HealingTask.BodyPartRegeneration
                || !comp.Pawn.health.hediffSet.HasHediff(comp.currentHediff.def, comp.currentHediff.Part)
                || comp.ParentBPHealthNeededCheck() && comp.ParentBPHealthCheck()
                )
            {
                Impossible = true;
                return false;
            }

            Impossible = false;
            if (comp.MyDebug)
                Log.Warning(comp.Pawn.LabelShort + " - TryBodyPartRegeneration calling HungerAndRestTransaction");

            BodyPartRecord BPR = comp.currentHediff.Part;
            float BPRMaxHealth = BPR.def.GetMaxHealth(comp.Pawn);
            float TheoricSeverity = BPRMaxHealth * (1 - comp.Props.BodyPartRegenParams.BPMaxHealth);

            float PawnBodyPartRatio = BPRMaxHealth * comp.Props.BodyPartRegenParams.BPMaxHealth / comp.BodyPartsHealthSum;

            if (comp.MyDebug)
                Log.Warning(
                comp.Pawn.LabelShort + " - TryBodyPartRegeneration " +
                "BPRMH: " + BPRMaxHealth + "+; " +
                "TheoricSeverity: " + TheoricSeverity + "+; " +
                "PawnBodyPartRatio: " + PawnBodyPartRatio + "+; "
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

            if (comp.MyDebug)
                Log.Warning("TryBodyPartRegeneration OK");

            comp.Pawn.health.RemoveHediff(comp.currentHediff);

            // artificial, needs parameter ?
            if (comp.HasLimits)
                comp.TreatmentPerformedQuality += PawnBodyPartRatio * 10;

            if (comp.Effect_PartialHealthUponRegrow && !comp.Effect_RegenBodyPartChildrenAtOnce)
            {

                //Hediff BarelyAliveBP = HediffMaker.MakeHediff(HediffDefOf.SurgicalCut, comp.Pawn, BPR);
                HediffDef HD = comp.Props.BodyPartRegenParams.regrownHediff == null ? HediffDefOf.SurgicalCut : comp.Props.BodyPartRegenParams.regrownHediff;
                Hediff BarelyAliveBP = HediffMaker.MakeHediff(HD, comp.Pawn, BPR);
                BarelyAliveBP.Severity = TheoricSeverity;
                if (BarelyAliveBP.def.tendable)
                {
                    BarelyAliveBP.Tended(new FloatRange(.2f, 1f).RandomInRange, 1f);
                }

                comp.Pawn.health.AddHediff(BarelyAliveBP, BPR);
            }

            //pawn.health.hediffSet.DirtyCache();
            return true;
        }
    }
}
