using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace HEREHEGI
{
    public class HediffReplacer_HediffGiver : HediffGiver
    {
        private bool MyDebug = false;
        private string debugWarning = "HEREHEGI - OnHediffAdded - ";

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            //HediffDef hediffDef = this.hediff;
            HealthUtility.AdjustSeverity(pawn, hediff, .1f);
        }
        
        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            if (pawn == null || pawn.Map == null || !pawn.Spawned)
                return false;

            HediffDef DataHediffDef = this.hediff;

            Hediff DataHediff = pawn.health.hediffSet.GetFirstHediffOfDef(DataHediffDef);
            if (DataHediff == null)
            {
                Tools.Warn(debugWarning + "Could not retrieve " + DataHediffDef.label, Prefs.DevMode && DebugSettings.godMode);
                return false;
            }
            HediffComp_DataHediff hComp = DataHediff.TryGetComp<HediffComp_DataHediff>();
            if (DataHediff == null)
            {
                Tools.Warn(debugWarning + "Could not retrieve HediffComp_DataHediff", Prefs.DevMode && DebugSettings.godMode);
                return false;
            }

            MyDebug = hComp.Props.debug;

            if (!hComp.IsValid)
            {
                Tools.Warn(debugWarning + "HediffComp_DataHediff is invalid, wont apply anything", MyDebug);
                return false;
            }

            if (!hComp.RetrieveHediffIndex(hediff.def, out int hediffIndex))
            {
                Tools.Warn(debugWarning + "Could not find hediff.def " + hediff.def.label + " - this is not critical", MyDebug);
                return false;
            }

            // If we are there, it means we have found the hediff that has been added in the hediff data
            float OldHediffSeverity = hediff.Severity;
            BodyPartRecord BPR = hediff?.Part ?? null;

            if (hComp.HasChances)
            {
                float Chances = hComp.Props.HediffReplacementChance[hediffIndex];
                if (!Rand.Chance(Chances))
                {
                    Tools.Warn(debugWarning + "replacement dice roll failure for " + hediff.def.label +
                        " - chances were: " + Chances.ToStringPercent(), MyDebug);
                    return false;
                }
                else
                {
                    Tools.Warn(debugWarning + "replacement dice roll success for " + hediff.def.label +
                        " - chances were: " + Chances.ToStringPercent(), MyDebug);
                }
                
            }

            pawn.health.RemoveHediff(hediff);
            Tools.Warn(debugWarning + "hediff removed: " + hediff.Label, MyDebug);

            HediffDef outputHediff = hComp.Props.OutputHediffPool[hediffIndex];
            if (!outputHediff.IsNullHediff())
            {
                Hediff newHediff = HediffMaker.MakeHediff(outputHediff, pawn, BPR);
                newHediff.Severity = OldHediffSeverity;

                pawn.health.AddHediff(newHediff);
                Tools.Warn(debugWarning + "hediff " + newHediff.Label + " added to " + BPR.Label, MyDebug);
            }

            return true;
        }
        
        
    }
}
