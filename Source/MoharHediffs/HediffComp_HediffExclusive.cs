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

        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
                return;

            /*
            if (HasHediffToApply && HasHediffToNullify)
            {
                if (Props.hediffToNullify.Contains(Props.hediffToApply))
                {
                    Tools.Warn("Same hediff in both lists, hediff autokill", myDebug);
                    Tools.DestroyParentHediff(parent, myDebug);
                }
            }
            */


            //Tools.Warn("hediff Nullifier - Working", myDebug);

            int i = 0;
            if (HasHediffToNullify || HasHediffPatternToNullify)
                foreach (Hediff curHediff in pawn.health.hediffSet.hediffs)
                {
                    //Tools.Warn(pawn.NameShortColored + " hediff #" + i + ": " + curHediff.def.defName, myDebug);

                    int j = 0;
                    foreach (string curHediffToNullify in Props.hediffToNullify)
                    {
                        //Tools.Warn(" hediff to nullify #" + j + ": " + curHediffToNullify, myDebug);

                        if ( (curHediff.def.defName == curHediffToNullify) || PatternMatch(curHediff.def.defName))
                        {
                            //if((curHediff.Severity != 0) && (curHediff.ageTicks > 5))
                            curHediff.Severity = 0;
                            Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);

                        }
                        j++;
                    }
                    i++;
                }

            if (!HasHediffToApply)
                return;

            HediffDef hediff2use = HediffDef.Named(Props.hediffToApply);
            if (hediff2use == null)
            {
                Tools.Warn("cant find hediff called: "+ Props.hediffToApply, true);
                return;
            }
                
            //AbilityDef abilityDef = DefDatabase<AbilityDef>.AllDefs.Where((AbilityDef a) => a.level == abilityLevel).RandomElement();
            //BodyDef myBodyDef = DefDatabase<BodyDef>.AllDefs.Where((BodyDef Def.defName == Props.bodyPartRecord)
            //BodyPartDef myBPDef = body
            BodyPartDef myBPDef = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b.defName == Props.bodyPartName).RandomElement();
            if (myBPDef == null)
            {
                Tools.Warn("cant find body part def called: " + Props.bodyPartName, true);
                return;
            }

            BodyPartRecord myBP = pawn.RaceProps.body.GetPartsWithDef(myBPDef).RandomElement();
            if(myBP == null)
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

            Tools.DestroyParentHediff(parent, myDebug);
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                
                result += "This should dissapear very fast";
                
                return result;
            }
        }
    }
}
