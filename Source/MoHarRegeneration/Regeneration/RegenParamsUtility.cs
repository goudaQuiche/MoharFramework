using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenParamsUtility
    {
        public static int ResetHealingTicks(this HediffComp_Regeneration comp)
        {
            Tools.Warn(comp.Pawn.LabelShort + " - Entering ResetHealingTicks - cutHT=" + comp.currentHT.DescriptionAttr(), comp.MyDebug);

            MyDefs.HealingTask curHT = comp.currentHT;
            HealingParams HP = comp.GetParams();
            if (HP == null)
            {
                Tools.Warn(comp.Pawn.LabelShort + " - ResetHealingTicks - Found not params for task=" + curHT.DescriptionAttr(), comp.MyDebug);
                return 600;
            }

            return HP.PeriodBase.RandomInRange;
        }

        public static MyDefs.HealingTask InitHealingTask(this HediffComp_Regeneration comp, out Hediff hediffToTreat, out int InitTicks)
        {
            //Tools.Warn(comp.Pawn.LabelShort + " - Entering InitHealingTask", comp.MyDebug);
            //for (int i = 0; i < comp.regenerationPriority.DefaultPriority.Count; i++)
            for (int i = 0; i < MyDefs.DefaultPriority.Count; i++)
            {
                //MyDefs.HealingTask curHealingTask = comp.regenerationPriority.DefaultPriority[i];
                
                MyDefs.HealingTask curHealingTask = MyDefs.DefaultPriority[i];

                //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask task[" + i + "]: " + curHealingTask.DescriptionAttr(), comp.MyDebug);

                // 00 bloodloss tending
                if (comp.Effect_TendBleeding && curHealingTask.IsBloodLossTending())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_TendBleeding", comp.MyDebug);
                    if (comp.GetBleedingHediff(out hediffToTreat))
                    {
                        InitTicks = comp.Props.BloodLossTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // 01 chronic disease tending
                else if (comp.Effect_TendChronicDisease && curHealingTask.IsChronicDiseaseTending())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_TendChronicDisease", comp.MyDebug);

                    if (comp.Pawn.GetTendableChronicDisease(out hediffToTreat))
                    {
                        InitTicks = comp.Props.ChronicHediffTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }
                // 02 regular disease tending
                else if (comp.Effect_TendRegularDisease && curHealingTask.IsRegularDiseaseTending())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_TendRegularDisease", comp.MyDebug);

                    if (comp.HasTendRegularDiseaseTargets)
                    {
                        if (comp.Pawn.GetTendableRegularDisease(out hediffToTreat, comp.Props.RegularDiseaseTendingParams.TargetedHediffDefs, comp.MyDebug))
                        {
                            InitTicks = comp.Props.RegularDiseaseTendingParams.PeriodBase.RandomInRange;
                            return curHealingTask;
                        }
                    }
                    else if (comp.Pawn.GetTendableRegularDisease(out hediffToTreat, null, comp.MyDebug))
                    {
                        InitTicks = comp.Props.RegularDiseaseTendingParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }

                // 03 regular injury
                else if (comp.Effect_RegeneratePhysicalInjuries && curHealingTask.IsInjuryRegeneration())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_RegeneratePhysicalInjuries", comp.MyDebug);

                    if (comp.GetPhysicalHediff(out hediffToTreat))
                    {
                        InitTicks = comp.Props.PhysicalInjuryRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // 04 regular disease
                else if(comp.Effect_HealDiseases && curHealingTask.IsDiseaseHealing())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_HealDiseases", comp.MyDebug);

                    if (comp.GetDiseaseHediff(out hediffToTreat))
                    {
                        InitTicks = comp.Props.DiseaseHediffRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                }
                // 05 chemicals
                else if(comp.Effect_RemoveChemicals && curHealingTask.IsChemicalRemoval())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_RemoveChemicals", comp.MyDebug);

                    if (comp.GetChemicalHediff(out hediffToTreat))
                    {
                        InitTicks = comp.Props.ChemicalHediffRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
                // 06 permanent
                else if(comp.Effect_RemoveScares && curHealingTask.IsPermanentInjuryRegeneration())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_RemoveScares", comp.MyDebug);
                    if (comp.Pawn.GetPermanentHediff(out hediffToTreat))
                    {
                        InitTicks = comp.Props.PermanentInjuryRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }

                // 07 Bodypart regen
                else if(comp.Effect_RegenerateBodyParts && curHealingTask.IsBodyPartRegeneration())
                {
                    //Tools.Warn(comp.Pawn.LabelShort + " InitHealingTask Effect_RegenerateBodyParts", comp.MyDebug);
                    if (comp.Pawn.GetMissingBodyPart(out hediffToTreat))
                    {
                        InitTicks = comp.Props.BodyPartRegenParams.PeriodBase.RandomInRange;
                        return curHealingTask;
                    }
                        
                }
            }

            //Tools.Warn(comp.Pawn.LabelShort + " - Exiting InitHealingTask: found nothing to do", comp.MyDebug);

            InitTicks = 0;
            hediffToTreat = null;
            return MyDefs.HealingTask.None;
        }

        public static bool GetBleedingHediff(this HediffComp_Regeneration comp, out Hediff hediff)
        {
            //Tools.Warn(comp.Pawn.LabelShort + " GetBleedingHediff", comp.MyDebug);

            /*
            if (!comp.Pawn.health.HasHediffsNeedingTend())
            {
                Tools.Warn(comp.Pawn.LabelShort + " GetBleedingHediff - HasHediffsNeedingTend == false ", comp.MyDebug);
                hediff = null;
                return false;
            }
            */

            IEnumerable<Hediff> hediffs =
            comp.Pawn.health.hediffSet.GetHediffs<Hediff>().Where(
                h => h.Bleeding &&
                h.TendableNow() &&
                !h.IsTended()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                //Tools.Warn(comp.Pawn.LabelShort + " GetBleedingHediff - Found no bloodloss", comp.MyDebug);
                hediff = null;
                return false;
            }

            hediff = hediffs.MostSeverityHediff();

            return true;
        }

        public static bool GetPhysicalHediff(this HediffComp_Regeneration comp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(comp.Pawn, comp.Props.PhysicalInjuryRegenParams, out hediff);
        }

        public static bool GetChemicalHediff(this HediffComp_Regeneration comp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(comp.Pawn, comp.Props.ChemicalHediffRegenParams, out hediff);
        }

        public static bool GetDiseaseHediff(this HediffComp_Regeneration comp, out Hediff hediff)
        {
            return GetHediffFromRegenParamsHediffArray(comp.Pawn, comp.Props.DiseaseHediffRegenParams, out hediff);
        }

        public static Hediff MostSeverityHediff(this IEnumerable<Hediff> IEH)
        {
            float maxSeverity = IEH.Max(h => h.Severity);
            return IEH.First(h => h.Severity == maxSeverity);
        }

        public static bool GetHediffFromRegenParamsHediffArray(this Pawn p, HealingWithHediffListParams HP, out Hediff hediff)
        {
            IEnumerable<Hediff> hediffs =
                p.health.hediffSet.GetHediffs<Hediff>().Where(
                h => 
                HP.TargetedHediffDefs.Contains(h.def) &&
                !h.IsPermanent()
            );

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            hediff = hediffs.MostSeverityHediff();

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

            hediff = hediffs.MostSeverityHediff();

            return true;
        }

        public static bool GetMissingBodyPart(this Pawn p, out Hediff hediff, bool myDebug=false)
        {
            List<Hediff_MissingPart> mpca = p.health.hediffSet.GetMissingPartsCommonAncestors();
            if (mpca.NullOrEmpty())
            {
                hediff = null;
                return false;
            }

            float maxHP = mpca.Max(h => h.Part.def.GetMaxHealth(p));
            hediff = mpca.First(h => h.Part.def.GetMaxHealth(p) == maxHP);

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

            hediff = hediffs.MostSeverityHediff();

            return true;
        }

        public static bool GetTendableRegularDisease(this Pawn p, out Hediff hediff, List<HediffDef> TargetedHediffs = null, bool MyDebug = false)
        {
            //Tools.Warn(p.LabelShort + " Entering GetTendableRegularDisease", MyDebug);

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
            /*
            if (MyDebug)
            {
                int i = 0;
                foreach(Hediff h in hediffs)
                {
                    Tools.Warn(p.LabelShort + " GetTendableRegularDisease hediff[" + i + "]:" + h.def.defName, MyDebug);
                    i++;
                }
            }
            */

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            if (!TargetedHediffs.NullOrEmpty())
            {
                hediffs = hediffs.Where(
                    h =>
                    TargetedHediffs.Contains(h.def)
                );
                /*
                if (MyDebug)
                {
                    int i = 0;
                    foreach (Hediff h in hediffs)
                    {
                        Tools.Warn(p.LabelShort + " GetTendableRegularDisease hediff[" + i + "]:" + h.def.defName, MyDebug);
                        i++;
                    }
                }
                */
            }

            if (hediffs.EnumerableNullOrEmpty())
            {
                hediff = null;
                return false;
            }

            hediff = hediffs.MostSeverityHediff();

            return true;
        }

        public static string GetTreatmentLabel(this HediffComp_Regeneration comp)
        {
            //Tools.Warn(comp.Pawn.LabelShort + " - Entering GetTreatmentLabel - cutHT=" + comp.currentHT.DescriptionAttr(), comp.MyDebug);

            MyDefs.HealingTask curHT = comp.currentHT;
            HealingParams HP = comp.GetParams();
            if(HP == null || HP.TreatmentLabel.NullOrEmpty())
                return curHT.DescriptionAttr();

            return HP.TreatmentLabel;
        }
    }
}
