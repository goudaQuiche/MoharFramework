using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class HealingDispatcher
    {
        public static void Dispatcher(this HediffComp_Regeneration comp)
        {
            MyDefs.HealingTask curHT = comp.currentHT;
            HealingParams HP = comp.GetParams();

            Pawn p = comp.Pawn;
            bool MyDebug = comp.MyDebug;

            bool DidIt = false;
            bool DoneWithIt = false;
            bool Impossible = false;
            bool NextHediffIfDidIt = false;
            bool NextHediffIfDoneWithIt = false;
            bool ResetHealingTick = true;
            FleckDef MyFleckDef = HP.FleckDef ?? null;

            // 00 Tending - Blood loss
            if (curHT.IsBloodLossTending())
            {
                NextHediffIfDidIt = true;
                DidIt = comp.TryTendBleeding(out Impossible);
            }
            // 01 Tending - Chronic disease
            else if (curHT.IsChronicDiseaseTending())
            {
                NextHediffIfDidIt = true;
                DidIt = comp.TryTendChronic(out Impossible);
            }
            // 02 Tending - Regular disease
            else if (curHT.IsRegularDiseaseTending())
            {
                NextHediffIfDidIt = true;
                DidIt = comp.TryTendRegularDisease(out Impossible);
            }
            // 03 Regeneration - Injury 
            else if (curHT.IsDiseaseHealing())
            {
                NextHediffIfDoneWithIt = true;
                DidIt = comp.TryCureDisease(out DoneWithIt, out Impossible);
            }
            // 04 Regeneration - Injury 
            else if (curHT.IsInjuryRegeneration())
            {
                NextHediffIfDoneWithIt = true;
                DidIt = comp.TryRegenInjury(out DoneWithIt, out Impossible);
            }
            // 05 Regeneration - Chemical 
            else if (curHT.IsChemicalRemoval())
            {
                NextHediffIfDoneWithIt = true;
                DidIt = comp.TryChemicalRemoval(out DoneWithIt, out Impossible);
            }
            // 06 Regeneration - Permanent injury
            else if (curHT.IsPermanentInjuryRegeneration())
            {
                NextHediffIfDoneWithIt = true;
                DidIt = comp.TryRemovePermanentInjury(out DoneWithIt, out Impossible);
            }
            // 07 Regeneration -Bodypart
            else if (curHT.IsBodyPartRegeneration())
            {
                NextHediffIfDidIt = true;
                bool AppliedProsthetic = false;

                if (comp.Effect_GrowProsthetic)
                {
                    HediffDef ProstheticHediff = comp.TryFindBodyPartProsthetic();
                    if (ProstheticHediff != null)
                        DidIt = AppliedProsthetic = comp.TryRegrowProsthetic(ProstheticHediff);
                }

                if (!AppliedProsthetic)
                {
                    if (comp.Effect_RegenBodyPartChildrenAtOnce)
                    {
                        //DidIt = this.TryBodyPartFullRegeneration(out Impossible);
                        DidIt = comp.TryBodyPartRegeneration(out Impossible);
                        ResetHealingTick = false;
                    }
                    else
                        DidIt = comp.TryBodyPartRegeneration(out Impossible);
                }
            }

            if (DidIt)
                Tools.Warn(p.LabelShort + " had " + curHT.DescriptionAttr() + " performed", MyDebug);
            if (DoneWithIt)
                Tools.Warn(p.LabelShort + " had " + curHT.DescriptionAttr() + " fully cured/healed/regen", MyDebug);

            if (NextHediffIfDidIt && DidIt || NextHediffIfDoneWithIt && DoneWithIt)
            {
                if (MyFleckDef != null)
                    FleckMaker.ThrowMetaIcon(p.Position, p.Map, MyFleckDef);
                //MoteMaker.ThrowMetaIcon(p.Position, p.Map, MyMoteDef);

                comp.RemoveProgressHediff();
                comp.ApplyCompleteHediff();

                if (ResetHealingTick)
                    comp.NextHediff();
                else
                    comp.NextHediffWithoutTickReset();

                Tools.Warn(p.LabelShort +
                    " ResetHealingTick:" + ResetHealingTick + "; HealingTicks:" + comp.HealingTickCounter +
                    "; new HT: " + curHT.DescriptionAttr() +
                    "; BP:" + comp.currentHediff?.Part?.def?.defName, MyDebug);
            }
            else if (Impossible)
            {
                comp.NextHediff();
                Tools.Warn(p.LabelShort + " Impossible to heal hediff found - new HT: " + curHT.DescriptionAttr(), MyDebug);
            }

            if (comp.HasNoPendingTreatment)
            {
                Tools.Warn(
                    p.LabelShort +
                    "no pending treatment, InitCheckCounter", MyDebug);
                comp.InitCheckCounter();
            }
        }
    }
}
