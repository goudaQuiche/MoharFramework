﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenParamsUtility
    {
        public static MyDefs.HealingTask InitHealingTask(this HediffComp_Regeneration RegenHComp, out Hediff hediffToTreat, out int InitTicks)
        {
            for (int i = 0; i < RegenHComp.regenerationPriority.DefaultPriority.Count; i++)
            {
                MyDefs.HealingTask curHealingTask = RegenHComp.regenerationPriority.DefaultPriority[i];

                // 00 bloodloss tending
                if(RegenHComp.Effect_TendBleeding && curHealingTask.IsBloodLossTending())
                {
                    if (RegenHComp.GetBleedingHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.BloodLossTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // 01 chronic disease tending
                else if (RegenHComp.Effect_TendChronicDisease && curHealingTask.IsChronicDiseaseTending())
                {
                    if (RegenHComp.Pawn.GetTendableChronicDisease(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.ChronicHediffTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }
                // 02 regular disease tending
                else if (RegenHComp.Effect_TendRegularDisease && curHealingTask.IsRegularDiseaseTending())
                {
                    if (RegenHComp.Pawn.GetTendableRegularDisease(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.RegularDiseaseTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }

                // 03 regular injury
                else if (RegenHComp.Effect_RegeneratePhysicalInjuries && curHealingTask.IsInjuryRegeneration())
                {
                    if (RegenHComp.GetPhysicalHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.RegularDiseaseTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // 04 regular disease
                else if(RegenHComp.Effect_HealDiseases && curHealingTask.IsDiseaseHealing())
                {
                    if (RegenHComp.GetDiseaseHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.DiseaseHediffRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }
                // 05 chemicals
                else if(RegenHComp.Effect_RemoveChemicals && curHealingTask.IsChemicalRemoval())
                {
                    if (RegenHComp.GetChemicalHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.ChemicalHediffRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // 06 permanent
                else if(RegenHComp.Effect_RemoveScares && curHealingTask.IsPermanentInjuryRegeneration())
                {
                    if (RegenHComp.Pawn.GetPermanentHediff(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.PermanentInjuryRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }

                // 07 Bodypart regen
                else if(RegenHComp.Effect_RegenerateBodyParts && curHealingTask.IsBodyPartRegeneration())
                {
                    if (RegenHComp.Pawn.GetMissingBodyPart(out hediffToTreat))
                    {
                        InitTicks = RegenHComp.Props.BodyPartRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }

            }

            InitTicks = 0;
            hediffToTreat = null;
            return MyDefs.HealingTask.None;
        }

        public static bool GetBleedingHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            Tools.Warn(RegenHComp.Pawn.LabelShort + " GetBleedingHediff", RegenHComp.MyDebug);

            /*
            if (!RegenHComp.Pawn.health.HasHediffsNeedingTend())
            {
                Tools.Warn(RegenHComp.Pawn.LabelShort + " GetBleedingHediff - HasHediffsNeedingTend == false ", RegenHComp.MyDebug);
                hediff = null;
                return false;
            }
            */

            IEnumerable<Hediff> hediffs =
            RegenHComp.Pawn.health.hediffSet.GetHediffs<Hediff>().Where(
                h => h.Bleeding &&
                h.TendableNow() &&
                !h.IsTended()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                Tools.Warn(RegenHComp.Pawn.LabelShort + " GetBleedingHediff - Found no bloodloss", RegenHComp.MyDebug);
                hediff = null;
                return false;
            }

            float maxSeverity = hediffs.Max(h => h.Severity);
            hediff = hediffs.First(h => h.Severity == maxSeverity);

            return true;
        }

        public static bool GetPhysicalHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(RegenHComp.Pawn, RegenHComp.Props.PhysicalInjuryRegenParams, out hediff);
        }

        public static bool GetChemicalHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(RegenHComp.Pawn, RegenHComp.Props.ChemicalHediffRegenParams, out hediff);
        }

        public static bool GetDiseaseHediff(this HediffComp_Regeneration RegenHComp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(RegenHComp.Pawn, RegenHComp.Props.DiseaseHediffRegenParams, out hediff);
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

        public static bool GetTendableRegularDisease(this Pawn p, out Hediff hediff)
        {
            if (!p.health.HasHediffsNeedingTend())
            {
                hediff = null;
                return false;
            }

            IEnumerable<Hediff> hediffs =
                p.health.hediffSet.GetHediffs<Hediff>().Where(
                h =>
                !h.def.chronic &&
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