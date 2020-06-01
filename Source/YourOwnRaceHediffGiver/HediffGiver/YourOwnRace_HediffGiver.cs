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
        private readonly string ErrStr = "HediffGiver_YourOwnRace denied bc ";
        //private readonly bool myDebug = true;
        private bool myDebug = false;

        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            HediffDef hediffDef = this.hediff;

            if (!pawn.Spawned)
                return false;

            /*
            if(hediff.def != hediffDef)
            {
                if(Prefs.DevMode)
                    Log.Warning(ErrStr + pawn.LabelShort + " hediff is : " + hediff.def.defName + "; we are looking for: "+ hediffDef.defName);
                return false;
            }

            if (hediff == null)
            {
                if (Prefs.DevMode)
                    Log.Warning(ErrStr + pawn.LabelShort + " hediff is null");
                return false;
            }
            */
            if (!hediffDef.HasModExtension<HediffDefModExtension>())
            {
                if (Prefs.DevMode)
                    Log.Warning(ErrStr + pawn.LabelShort + " No Mod extension found in hediffDef");
                return false;
            }

            string race = hediffDef.GetModExtension<HediffDefModExtension>()?.race ?? "";
            myDebug = hediffDef.GetModExtension<HediffDefModExtension>()?.debug ?? false;
            bool keepLowSeverity = hediffDef.GetModExtension<HediffDefModExtension>()?.keepLowSeverity ?? false;
            BodyPartDef partToAffect = hediffDef.GetModExtension<HediffDefModExtension>()?.partToAffect ?? null;

            if (!pawn.IsRaceMember(race))
            {
                Tools.Warn(ErrStr + pawn.LabelShort + " is not from race: " + race, myDebug);
                return false;
            }

            BodyPartRecord BPR = null;
            if (partToAffect != null)
            {
                BPR = pawn.GetBPRecord(partToAffect.defName);
                /*
                if (BPR != null && hediff.Part != BPR)
                {
                    if (pawn.Spawned)
                        Tools.Warn(pawn.LabelShort + "'s" + ErrStr + "hediff.Part(" + hediff?.Part?.def?.defName + ") != pawn.GetBPRecord()", myDebug);
                    return false;
                }
                */
            }
            bool appliedHediff = TryApply(pawn, null);

            /*
            bool hasHediff = false;
            if (BPR != null)
                hasHediff = pawn.HasHediff(hediffDef, BPR);

            if (hasHediff && keepLowSeverity)
            {
                Hediff myH = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);

                if (hediffDef.initialSeverity != 0)
                {
                    Tools.Warn("Adjusting "+myH.def.defName+" severity from: " + myH.Severity + " to " + hediffDef.initialSeverity+"("+hediffDef.defName+" initialseverity)", myDebug);
                    myH.Severity = hediffDef.initialSeverity;
                }
            }
            */

            if (appliedHediff)
            {
                if (pawn.Spawned)
                    Tools.Warn(pawn.LabelShort + "'s YourOwnRace_HediffGiver applied " + this.hediff.defName, myDebug);
                return true;
            }

            //Tools.Warn(pawn.LabelShort + "'s YourOwnRace_HediffGiver did not apply " + this.hediff.defName, myDebug);
            return false;
        }

    }
}
