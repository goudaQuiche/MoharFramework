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
    public class HeDiffComp_HediffExclusive : HediffComp
    {
        const int tickLimiterModulo = 60;
        bool myDebug = false;

        public HeDiffCompProperties_HediffExclusive Props
        {
            get
            {
                return (HeDiffCompProperties_HediffExclusive)this.props;
            }
        }

        public override void CompPostMake()
        {
            //base.CompPostMake();
            myDebug = Props.debug;
        }

        public bool HasHediffToNullify
        {
            get
            {
                return (!Props.hediffToNullify.NullOrEmpty());
            }
        }
        public bool HasHediffPatternToNullify
        {
            get
            {
                return (!Props.hediffPatternToNullify.NullOrEmpty());
            }
        }
        public bool HasHediffToApply
        {
            get
            {
                return (!Props.hediffToApply.NullOrEmpty());
            }
        }

        private bool PatternMatch(string MyHediffDefname)
        {
            foreach(string cur in Props.hediffPatternToNullify)
            {
                if (MyHediffDefname.Contains(cur))
                    return true;
            }
            return false;
        }

        public void NullifyHediff(Pawn pawn)
        {
            int i = 0;
            if (HasHediffToNullify)
                foreach (Hediff curHediff in pawn.health.hediffSet.hediffs)
                {
                    Tools.Warn(pawn.Label + " hediff #" + i + ": " + curHediff.def.defName, myDebug);

                    int j = 0;
                    foreach (string curHediffToNullify in Props.hediffToNullify)
                    {
                        Tools.Warn(" Props.hediffToNullify #" + j + ": " + curHediffToNullify, myDebug);

                        if (curHediff.def.defName == curHediffToNullify)
                        {
                            curHediff.Severity = 0;
                            Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);
                        }
                        j++;
                    }
                    i++;
                }
        }

        public void PatternNullifyHediff(Pawn pawn)
        {
            int i = 0;
            if (HasHediffPatternToNullify)
                foreach (Hediff curHediff in pawn.health.hediffSet.hediffs)
                {
                    Tools.Warn(pawn.NameShortColored + " hediff #" + i + ": " + curHediff.def.defName, myDebug);

                    int j = 0;
                    foreach (string curHediffToNullify in Props.hediffPatternToNullify)
                    {
                        Tools.Warn(" Props.hediffPatternToNullify #" + j + ": " + curHediffToNullify, myDebug);

                        if (PatternMatch(curHediff.def.defName))
                        {
                            curHediff.Severity = 0;
                            Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);
                        }
                        j++;
                    }
                    i++;
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

            HediffDef hediff2use = HediffDef.Named(Props.hediffToApply);
            if (hediff2use == null)
            {
                Tools.Warn("cant find hediff called: " + Props.hediffToApply, true);
                return;
            }

            BodyPartDef myBPDef = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b.defName == Props.bodyPartName).RandomElement();
            if (myBPDef == null)
            {
                Tools.Warn("cant find body part def called: " + Props.bodyPartName, true);
                return;
            }
            
            BodyPartRecord myBP = pawn.RaceProps.body.GetPartsWithDef(myBPDef).RandomElement();
            if (myBP == null)
            {
                Tools.Warn("cant find body part record called: " + Props.bodyPartName, true);
                return;
            }

            Hediff hediff2apply = HediffMaker.MakeHediff(hediff2use, pawn, myBP);
            if (hediff2apply == null)
            {
                Tools.Warn("cant create hediff to apply: " + Props.bodyPartName, true);
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
