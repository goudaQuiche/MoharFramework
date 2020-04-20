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
    public class HeDiffComp_MultipleHediff : HediffComp
    {
        const int tickLimiterModulo = 60;
        bool myDebug = false;
        bool blockAction = false;

        public HeDiffCompProperties_MultipleHediff Props
        {
            get
            {
                return (HeDiffCompProperties_MultipleHediff)this.props;
            }
        }
        public void CheckProps()
        {
            string fctN = "CheckProps";
            if(Props.hediffToApply.Count != Props.bodyPartDef.Count)
            {
                Tools.Warn(fctN + "- Props.hediffToApply.Count != Props.bodyPartDef.Count", myDebug);
                Tools.DestroyParentHediff(parent, myDebug);
                blockAction = true;
            }
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();
            myDebug = Props.debug;
            CheckProps();
        }

        public bool HasHediffToApply
        {
            get
            {
                return !Props.hediffToApply.NullOrEmpty();
            }
        }
        public void ApplyHediff(Pawn pawn)
        {
            if (Props.bodyDef != null)
                if (pawn.def.race.body != Props.bodyDef)
                {
                    Tools.Warn(pawn.Label + " has not a bodyDef like required: " + pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), true);
                    return;
                }

            for (int i = 0; i < Props.hediffToApply.Count; i++)
            {
                HediffDef curHD = Props.hediffToApply[i];
                BodyPartDef curBPD = Props.bodyPartDef[i];

                if (curHD == null)
                {
                    Tools.Warn("cant find hediff; i=" + i, true);
                    return;
                }
                if (curBPD == null)
                {
                    Tools.Warn("cant find body part def; i=" + i, true);
                    return;
                }

                BodyPartRecord myBPR = pawn.GetBPRecord(curBPD, myDebug);

                Hediff hediff2apply = HediffMaker.MakeHediff(curHD, pawn, myBPR);
                if (hediff2apply == null)
                {
                    Tools.Warn("cant create hediff " + curHD.defName + " to apply on " + curBPD.defName, true);
                    return;
                }

                pawn.health.AddHediff(hediff2apply, myBPR, null);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }
                
            if (blockAction)
            {
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            if (HasHediffToApply)
            {
                ApplyHediff(pawn);
            }

            // suicide
            Tools.DestroyParentHediff(parent, myDebug);
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                
                result += "This should disappear very fast";
                
                return result;
            }
        }
    }
}
