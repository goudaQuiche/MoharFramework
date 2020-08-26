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

        // ModExtensions
        private bool myDebug = false;
        string race = string.Empty;
        BodyPartDef conditionnalBodyPart = null;
        bool TriggeredOnHediffAdded = false;
        bool TriggeredOnIntervalPassed = false;

        private bool RequiresRaceCheck(string race)
        {
            return !race.NullOrEmpty();
        }

        private bool RequiresBodyPartCheck(BodyPartDef BPD)
        {
            return BPD != null;
        }

        private bool SetupModExtensions(HediffDef hediffDef, Pawn pawn)
        {
            if (!hediffDef.HasModExtension<HediffDefModExtension>() && Prefs.DevMode)
            {
                Log.Warning(ErrStr + pawn.LabelShort + " No Mod extension found in hediffDef => Impossible to get needed values");
                return false;
            }

            race = hediffDef.GetModExtension<HediffDefModExtension>()?.race ?? null;
            myDebug = hediffDef.GetModExtension<HediffDefModExtension>()?.debug ?? false;
            conditionnalBodyPart = hediffDef.GetModExtension<HediffDefModExtension>()?.conditionalBodyPart ?? null;

            TriggeredOnHediffAdded = hediffDef.GetModExtension<HediffDefModExtension>()?.TriggeredOnHediffAdded ?? false;
            TriggeredOnIntervalPassed = hediffDef.GetModExtension<HediffDefModExtension>()?.TriggeredOnIntervalPassed ?? false;

            return true;
        }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            HediffDef hediffDef = this.hediff;

            if (pawn == null || pawn.Map == null || !pawn.Spawned)
                return;

            SetupModExtensions(hediffDef, pawn);

            if (!TriggeredOnIntervalPassed)
                return;

            if (RequiresRaceCheck(race) && !pawn.IsRaceMember(race))
            {
                Tools.Warn(ErrStr + pawn.LabelShort + " is not from race: " + race, myDebug);
                return;
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
                        Hediff AlreadyHediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                        AlreadyHediff.Severity = 0;
                    }
                    else
                    {
                        Tools.Warn("Could not create removeH", myDebug);
                    }

                    Tools.Warn(
                        pawn.LabelShort + " got hediff " + hediffDef.defName + " removed bc " + conditionnalBodyPart.defName +
                        "- null:" + (BPR == null) + "; missingBPR:" + missingBPR + "; artificialBPR:" + artificialBPR
                        , myDebug);

                    return;
                }
            }

            HealthUtility.AdjustSeverity(pawn, hediff, .1f);
        }
        
        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            HediffDef hediffDef = this.hediff;

            if (pawn == null || pawn.Map == null || !pawn.Spawned)
                return false;

            SetupModExtensions(hediffDef, pawn);

            if (!TriggeredOnHediffAdded)
                return false;

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
