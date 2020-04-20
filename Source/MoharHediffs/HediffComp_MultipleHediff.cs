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
                parent.Severity = 0;
                blockAction = true;
            }
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();
            myDebug = Props.debug;
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

            foreach(HediffDef curH in Props.hediffToApply)
            {
                if (curH == null)
                {
                    Tools.Warn("cant find hediff called: " + Props.hediffToApply, true);
                    return;
                }
            }



            BodyPartDef myBPDef = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == Props.bodyPartDef).RandomElement();
            if (myBPDef == null)
            {
                Tools.Warn("cant find body part def called: " + Props.bodyPartDef.defName, true);
                return;
            }
            
            BodyPartRecord myBP = pawn.RaceProps.body.GetPartsWithDef(myBPDef).RandomElement();
            if (myBP == null)
            {
                Tools.Warn("cant find body part record called: " + Props.bodyPartDef.defName, true);
                return;
            }

            Hediff hediff2apply = HediffMaker.MakeHediff(hediff2use, pawn, myBP);
            if (hediff2apply == null)
            {
                Tools.Warn("cant create hediff "+ hediff2use.defName + " to apply on " + Props.bodyPartDef.defName, true);
                return;
            }

            pawn.health.AddHediff(hediff2apply, myBP, null);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
                return;

            NullifyHediff(pawn);
            PatternNullifyHediff(pawn);

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
