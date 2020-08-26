/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
    public class HediffComp_HediffNullifier : HediffComp
    {
        const int tickLimiterModulo = 60;
        private int LimitedUsageNumber = 0;

        bool BlockPostTick = false;

        readonly bool myDebug = false;

        public HediffCompProperties_HediffNullifier Props
        {
            get
            {
                return (HediffCompProperties_HediffNullifier)this.props;
            }
        }

        public bool HasHediffToNullify
        {
            get
            {
                return (!Props.hediffToNullify.NullOrEmpty());
            }
        }
        public bool HasLimitedUsage
        {
            get
            {
                return (Props.limitedUsageNumber != -99);
            }
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();

            Tools.Warn(">>>" + parent.def.defName + " - CompPostMake start", myDebug);

            if (!HasHediffToNullify)
            {
                Tools.Warn(parent.def.defName + " has no hediff to nullify, autokill", myDebug);
                Tools.DestroyParentHediff(parent, myDebug);
                BlockPostTick = true;
            }
                
            if (HasLimitedUsage)
                LimitedUsageNumber = Props.limitedUsageNumber;
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref LimitedUsageNumber, "LimitedUsageNumber");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Find.TickManager.TicksGame % tickLimiterModulo != 0)
            {
                //Tools.Warn("hediff Nullifier - Chilling", myDebug);
                return;
            }

            if (BlockPostTick)
                return;

            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
                return;

            //Tools.Warn("hediff Nullifier - Working", myDebug);

            foreach (Hediff curHediff in pawn.health.hediffSet.hediffs)
            {
                Tools.Warn(pawn.Label + " - " + curHediff.def.defName, myDebug);
                foreach (HediffDef curHediffToNullify in Props.hediffToNullify)
                {
                    if (curHediff.def == curHediffToNullify)
                    {

                        curHediff.Severity = 0;
                        Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);

                        if (HasLimitedUsage)
                        {
                            LimitedUsageNumber--;
                            if (LimitedUsageNumber <= 0)
                            {
                                Tools.Warn(parent.def.defName + " has reached its limit usage, autokill", myDebug);
                                Tools.DestroyParentHediff(parent, myDebug);
                            }
                        }
                    }
                }
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                if (!HasHediffToNullify)
                    return result;
                result += "Immune to: ";
                foreach (HediffDef hediffName in Props.hediffToNullify)
                {
                    result += hediffName.label + "; ";
                }

                if (!HasLimitedUsage)
                    result += (" for ever");
                else
                    result += (" " + LimitedUsageNumber + " left");

                return result;
            }
        }
    }
}
