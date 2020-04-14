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
                return (Props.limitedUsage);
            }
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();

            Tools.Warn(">>>" + parent.def.defName + " - CompPostMake start", myDebug);

            if(HasLimitedUsage)
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

            if (!HasHediffToNullify)
                return;

            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
                return;

            //Tools.Warn("hediff Nullifier - Working", myDebug);

            int i = 0;
            foreach (Hediff curHediff in pawn.health.hediffSet.hediffs)
            {
                //Tools.Warn(pawn.NameShortColored + " hediff #" + i + ": " + curHediff.def.defName, myDebug);

                int j = 0;
                foreach (string curHediffToNullify in Props.hediffToNullify)
                {
                    //Tools.Warn(" hediff to nullify #" + j + ": " + curHediffToNullify, myDebug);
                    
                    if (curHediff.def.defName == curHediffToNullify)
                    {
                        
                        curHediff.Severity = 0;
                        Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);

                        if (HasLimitedUsage)
                            LimitedUsageNumber--;

                        if (LimitedUsageNumber <= 0)
                            Tools.DestroyParentHediff(parent, myDebug);
                    }
                    j++;
                }
                i++;
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
                foreach (string hediffName in Props.hediffToNullify)
                {
                    result += hediffName + "; ";
                }

                if (!HasLimitedUsage)
                    result += (" for ever");
                else
                    result += (" - " + LimitedUsageNumber + " left");

                return result;
            }
        }
    }
}
