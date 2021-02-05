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
        private bool MyDebug = Prefs.DevMode && DebugSettings.godMode;
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
                if(MyDebug)Log.Warning(debugWarning + "Could not retrieve " + DataHediffDef.label);
                return false;
            }
            HediffComp_DataHediff hComp = DataHediff.TryGetComp<HediffComp_DataHediff>();
            if (DataHediff == null)
            {
                if (MyDebug) Log.Warning(debugWarning + "Could not retrieve HediffComp_DataHediff");
                return false;
            }

            MyDebug = hComp.Props.debug;

            if (!hComp.IsValid)
            {
                if (MyDebug) Log.Warning(debugWarning + "HediffComp_DataHediff is invalid, wont apply anything");
                return false;
            }

            if (!hComp.RetrieveItem(hediff.def, out ReplaceHediffItem RHI))
            {
                if (MyDebug) Log.Warning(debugWarning + "Could not find hediff.def " + hediff.def.label + " - this is not critical");
                return false;
            }

            // If we are there, it means we have found the hediff that has been added in the hediff data
            float OldHediffSeverity = hediff.Severity;
            BodyPartRecord BPR = hediff?.Part ?? null;

            if (RHI.HasConsiderableChances)
            {
                float Chances = RHI.chance.RandomInRange;
                if (!Rand.Chance(Chances))
                {
                    if (MyDebug) Log.Warning(debugWarning + "replacement dice roll failure for " + hediff.def.label +
                        " - chances were: " + Chances.ToStringPercent());
                    return false;
                }
                else
                {
                    if (MyDebug) Log.Warning(debugWarning + "replacement dice roll success for " + hediff.def.label +
                        " - chances were: " + Chances.ToStringPercent());
                }
                
            }

            pawn.health.RemoveHediff(hediff);
            if (MyDebug) Log.Warning(debugWarning + "hediff removed: " + hediff.Label);

            if (RHI.destroy)
                return true;


            HediffDef outputHediff = RHI.outputH;
            if (outputHediff != null)
            {
                Hediff newHediff = HediffMaker.MakeHediff(outputHediff, pawn, BPR);
                newHediff.Severity = OldHediffSeverity;

                pawn.health.AddHediff(newHediff);
                if (MyDebug) Log.Warning(debugWarning + "hediff " + newHediff.Label + " added to " + BPR?.Label);
            }
            else
            {
                return false;
            }

            return true;
        }
        
        
    }
}
