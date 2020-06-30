using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace YORHG
{
    public class YourOwnRace_HediffGiver : HediffGiver
    {
        private readonly string ErrStr = "YourOwnRace_HediffGiver denied bc ";
        //private readonly bool myDebug = true;
        private bool myDebug = false;

        private bool RequiresRaceCheck(string race)
        {
            return !race.NullOrEmpty();
        }

        private bool RequiresBodyPartCheck(BodyPartDef BPD)
        {
            return BPD != null;
        }

        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            HediffDef hediffDef = this.hediff;

            if (!pawn.Spawned)
                return false;

            if (!hediffDef.HasModExtension<HediffDefModExtension>() && Prefs.DevMode)
                Log.Warning(ErrStr + pawn.LabelShort + " No Mod extension found in hediffDef => no debug, no race check, no conditionnal body part check");

            string race = hediffDef.GetModExtension<HediffDefModExtension>()?.race ?? null;
            myDebug = hediffDef.GetModExtension<HediffDefModExtension>()?.debug ?? false;
            BodyPartDef conditionnalBodyPart = hediffDef.GetModExtension<HediffDefModExtension>()?.conditionnalBodyPart ?? null;

            if (RequiresRaceCheck(race) && !pawn.IsRaceMember(race))
            {
                Tools.Warn(ErrStr + pawn.LabelShort + " is not from race: " + race, myDebug);
                return false;
            }

            if (RequiresBodyPartCheck(conditionnalBodyPart))
            {
                BodyPartRecord BPR = pawn.GetBPRecord(conditionnalBodyPart.defName) ?? null;
                bool missingBPR = pawn.health.hediffSet.PartIsMissing(BPR);
                bool artificialBPR = pawn.health.hediffSet.AncestorHasDirectlyAddedParts(BPR);

                if (BPR == null || missingBPR || artificialBPR)
                {
                    Hediff removeH = HediffMaker.MakeHediff(hediffDef, pawn, null);
                    if (removeH != null)
                    {
                        pawn.health.RemoveHediff(removeH);
                        Hediff oldMethod = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                        bool StillHasHediff =  oldMethod != null;
                        Tools.Warn(pawn.LabelShort + " still has " + hediffDef.defName + " while should not; trying oldMethod", myDebug);

                        oldMethod.Severity = 0;
                    }
                    else
                    {
                        Tools.Warn("Could not create removeH", myDebug);
                    }
                        
                    Tools.Warn(
                        pawn.LabelShort + " got hediff " + hediffDef.defName + " removed bc " + conditionnalBodyPart.defName +
                        "- null:" + (BPR == null) + "; missingBPR:" + missingBPR + "; artificialBPR:" + artificialBPR
                        , myDebug);

                    return false;
                }
            }

            bool appliedHediff = TryApply(pawn, null);

            if (appliedHediff)
            {
                Tools.Warn(pawn.LabelShort + "'s YourOwnRace_HediffGiver applied " + hediffDef.defName, myDebug);
                return true;
            }

            //Tools.Warn(pawn.LabelShort + "'s YourOwnRace_HediffGiver did not apply " + this.hediff.defName, myDebug);
            return false;
        }

    }
}
