using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenParamsUtility
    {
        public enum HealingTask
        {
            [Description("None")]
            None = 0,
            [Description("Bleeding tending")]
            BleedingTending = 1,
            [Description("Injury regeneration")]
            InjuryRegeneration = 2,
            [Description("Chemical removal")]
            ChemicalRemoval = 3,
            [Description("Disease healing")]
            DiseaseHealing = 4,
            [Description("Permanent injury regeneration")]
            PermanentInjuryRegeneration = 5,
            [Description("Chronic disease")]
            ChronicDisease = 6,
            [Description("Body part regeneration")]
            BodyPartRegeneration = 7
        }

        public static HealingTask InitHealingTask(this HediffComp_Regeneration RegenHComp, out Hediff hediffToTreat, out int InitTicks)
        {
            for (int i = 0; i < RegenHComp.regenerationPriority.DefaultPriority.Count; i++)
            {
                HealingTask curHealingTask = RegenHComp.regenerationPriority.DefaultPriority[i];

                //bleeding
                if(RegenHComp.Effect_TendBleeding && curHealingTask == HealingTask.BleedingTending)
                {
                    if (RegenHComp.Pawn.GetBleedingHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.BleedingHediff.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                //regular injury
                else if (RegenHComp.Effect_RegeneratePhysicalInjuries && curHealingTask == HealingTask.InjuryRegeneration)
                {
                    if (RegenHComp.GetPhysicalHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.PhysicalHediff.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // regular disease
                else if(RegenHComp.Effect_HealDiseases && curHealingTask == HealingTask.DiseaseHealing)
                {
                    if (RegenHComp.GetDiseaseHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.DiseaseHediff.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // chemicals
                else if(RegenHComp.Effect_RemoveChemicals && curHealingTask == HealingTask.ChemicalRemoval)
                {
                    if (RegenHComp.GetChemicalHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.ChemicalHediff.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // permanent
                else if(RegenHComp.Effect_RemoveScares && curHealingTask == HealingTask.PermanentInjuryRegeneration)
                {
                    if (RegenHComp.Pawn.GetPermanentHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.PermanentInjury.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                else if(RegenHComp.Effect_TendChronicDisease && curHealingTask == HealingTask.ChronicDisease)
                {
                    if (RegenHComp.Pawn.GetTendableChronicDisease (out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.ChronicHediff.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }
                // Bodypart regen
                else if(RegenHComp.Effect_RegenerateBodyParts && curHealingTask == HealingTask.BodyPartRegeneration)
                {
                    if (RegenHComp.Pawn.GetMissingBodyPart(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.BodyPartRegeneration.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }

            }

            InitTicks = 0;
            hediffToTreat = null;
            return HealingTask.None;
        }

        public static bool GetBleedingHediff(this Pawn p, out Hediff hediff)
        {
            if (!p.health.HasHediffsNeedingTend())
            {
                hediff = null;
                return false;
            }

            IEnumerable<Hediff> hediffs =
            p.health.hediffSet.GetHediffs<Hediff>().Where(
                h => h.def == HediffDefOf.BloodLoss &&
                h.TendableNow() &&
                !h.IsTended()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            float maxSeverity = hediffs.Max(h => h.Severity);
            hediff = hediffs.First(h => h.Severity == maxSeverity);

            return true;
        }

        public static bool GetPhysicalHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(RegenHComp.Pawn, RegenHComp.Props.PhysicalHediff, out hediff);
        }

        public static bool GetChemicalHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(RegenHComp.Pawn, RegenHComp.Props.ChemicalHediff, out hediff);
        }

        public static bool GetDiseaseHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(RegenHComp.Pawn, RegenHComp.Props.DiseaseHediff, out hediff);
        }

        public static bool GetHediffFromRegenParamsHediffArray(this Pawn p, RegenParams RP, out Hediff hediff)
        {
            IEnumerable<Hediff> hediffs =
                p.health.hediffSet.GetHediffs<Hediff>().Where(
                h => 
                RP.HediffDefs.Contains(h.def) &&
                !h.IsPermanent()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            float maxSeverity = hediffs.Max(h => h.Severity);
            hediff = hediffs.First(h => h.Severity == maxSeverity);

            return true;
        }

        public static bool GetPermanentHediff(this Pawn p, out Hediff hediff)
        {
            IEnumerable<Hediff> hediffs =
                p.health.hediffSet.GetHediffs<Hediff>().Where(
                h =>
                h.def != HediffDefOf.MissingBodyPart && 
                h.IsPermanent()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            float maxSeverity = hediffs.Max(h => h.Severity);
            hediff = hediffs.First(h => h.Severity == maxSeverity);

            return true;
        }

        public static bool GetMissingBodyPart(this Pawn p, out Hediff hediff)
        {
            List<Hediff_MissingPart> mpca = p.health.hediffSet.GetMissingPartsCommonAncestors();
            if (mpca.NullOrEmpty())
            {
                hediff = null;
                return false;
            }

            float maxHP = mpca.Max(h => h.Part.def.GetMaxHealth(p));
            hediff = mpca.First(h => h.Severity == maxHP);
            return true;
        }

        public static bool GetTendableChronicDisease(this Pawn p, out Hediff hediff)
        {
            if (!p.health.HasHediffsNeedingTend())
            {
                hediff = null;
                return false;
            }

            IEnumerable<Hediff> hediffs =
                p.health.hediffSet.GetHediffs<Hediff>().Where(
                h =>
                h.def.chronic &&
                h.TendableNow() && 
                !h.IsTended()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            float maxSeverity = hediffs.Max(h => h.Severity);
            hediff = hediffs.First(h => h.Severity == maxSeverity);

            return true;
        }
    }
}
