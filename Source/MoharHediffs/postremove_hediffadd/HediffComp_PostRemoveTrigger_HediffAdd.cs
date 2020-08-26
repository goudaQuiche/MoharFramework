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
using System.Linq;
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
    public class HediffComp_PostRemoveTrigger_HediffAdd : HediffComp
    {
        //bool myDebug = false;
        bool blockAction = false;

        public HediffCompProperties_PostRemoveTrigger_HediffAdd Props
        {
            get
            {
                return (HediffCompProperties_PostRemoveTrigger_HediffAdd)this.props;
            }
        }

        public void CheckProps()
        {
            if (!HasHediffToApply)
            {
                blockAction = true;
                Tools.DestroyParentHediff(parent, Props.debug);
            }
        }

        public bool HasHediffToApply
        {
            get
            {
                return !Props.triggeredHediff.NullOrEmpty();
            }
        }
        public void ApplyHediff(Pawn pawn)
        {
            for (int i = 0; i < Props.triggeredHediff.Count; i++)
            {
                HediffDef curHD = Props.triggeredHediff[i];

                if (curHD == null)
                {
                    Tools.Warn("cant find hediff; i=" + i, true);
                    return;
                }
                Hediff hediff2apply = HediffMaker.MakeHediff(curHD, pawn, null);
                if (hediff2apply == null)
                {
                    Tools.Warn("cant create hediff " + curHD.defName, true);
                    return;
                }

                Tools.Warn("Adding " + curHD.defName + "for science", Props.debug);
                pawn.health.AddHediff(hediff2apply, null, null);
            }
        }

        public override void CompPostPostRemoved()
        {
            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                Tools.DestroyParentHediff(parent, Props.debug);
                return;
            }
            if (blockAction) return;

            Tools.Warn(parent.def.defName + " is no more, applying hediff", Props.debug);
            if (HasHediffToApply)
            {
                ApplyHediff(pawn);
            }

        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                if(Props.debug)
                    result += parent.def.defName+" is still alive, aperture science we do what we must";
                
                return result;
            }
        }
    }
}
