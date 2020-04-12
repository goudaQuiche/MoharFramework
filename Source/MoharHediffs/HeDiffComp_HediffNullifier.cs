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
        bool myDebug = false;

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
                        //if((curHediff.Severity != 0) && (curHediff.ageTicks > 5))
                        curHediff.Severity = 0;
                        Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);
                        
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

                return result;
            }
        }
    }
}
