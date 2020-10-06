using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class AdditionnalHediff
    {
        public static bool RequiresProgressHediff(this HediffComp_Regeneration comp, out HediffDef hediffToApply)
        {
            MyDefs.HealingTask curHT = comp.currentHT;
            HealingParams HP = comp.GetParams();

            hediffToApply = null;
            if (HP == null || HP.HediffToApplyDuringProgress == null)
                return false;

            hediffToApply = HP.HediffToApplyDuringProgress;
            return true;
        }

        public static bool RequiresProgressHediffRemoval(this HediffComp_Regeneration comp, out HediffDef hediffToRemove)
        {
            MyDefs.HealingTask curHT = comp.currentHT;
            HealingParams HP = comp.GetParams();

            hediffToRemove = null;
            if (HP == null || HP.HediffToApplyDuringProgress == null)
                return false;

            hediffToRemove = HP.HediffToApplyDuringProgress;
            return HP.RemoveHediffWhenProgressOver;
        }

        public static bool RequiresCompleteHediff(this HediffComp_Regeneration comp, out HediffDef hediffToApply)
        {
            MyDefs.HealingTask curHT = comp.currentHT;
            HealingParams HP = comp.GetParams();

            hediffToApply = null;
            if (HP == null || HP.HediffToApplyWhenComplete == null)
                return false;

            hediffToApply = HP.HediffToApplyWhenComplete;
            return true;
        }

        // Applied when nexthediff
        public static void ApplyProgressHediff(this HediffComp_Regeneration comp, Hediff OldHediff)
        {
            if (comp.HasPendingTreatment && comp.HealingTickCounter != 0 && OldHediff != comp.currentHediff && comp.RequiresProgressHediff(out HediffDef ProgressHediff))
            {
                if (ProgressHediff == null)
                    return;

                if (comp.Pawn.health.hediffSet.HasHediff(ProgressHediff))
                    return;

                Hediff NewHediff = HediffMaker.MakeHediff(ProgressHediff, comp.Pawn);
                comp.Pawn.health.AddHediff(NewHediff);
            }
        }

        // Applied when doneWithIt or DidIt in dispatcher
        public static void RemoveProgressHediff(this HediffComp_Regeneration comp)
        {
            if (comp.RequiresProgressHediffRemoval(out HediffDef ProgressHediff))
            {
                if (ProgressHediff == null)
                    return;

                if (!comp.Pawn.health.hediffSet.HasHediff(ProgressHediff))
                    return;

                Hediff hediff = comp.Pawn.health.hediffSet.GetFirstHediffOfDef(ProgressHediff);
                comp.Pawn.health.RemoveHediff(hediff);
            }
        }

        // Applied when doneWithIt or DidIt in dispatcher
        public static void ApplyCompleteHediff(this HediffComp_Regeneration comp)
        {
            if (comp.RequiresCompleteHediff(out HediffDef CompleteHediff))
            {
                if (CompleteHediff == null)
                    return;

                if (comp.Pawn.health.hediffSet.HasHediff(CompleteHediff))
                    return;

                Hediff NewHediff = HediffMaker.MakeHediff(CompleteHediff, comp.Pawn);
                comp.Pawn.health.AddHediff(NewHediff);
            }
        }

    }
}
