using Verse;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class HediffComp_MultipleHediff : HediffComp
    {
        bool myDebug = false;
        bool blockAction = false;

        bool HasBodyRequirement => Props.bodyDef != null;

        public HediffCompProperties_MultipleHediff Props => (HediffCompProperties_MultipleHediff)this.props;
        
        public void CheckProps()
        {
            string fctN = "CheckProps";
            if(!HasHediffToApply)
            {
                Tools.Warn(fctN + "- empty hediffAndBodypart, destroying", myDebug);
                Pawn.DestroyHediff(parent);
                blockAction = true;
            }

            if (HasBodyRequirement)
                if (Pawn.def.race.body != Props.bodyDef)
                {
                    Tools.Warn(Pawn.Label + " has not a bodyDef like required: " + Pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), true);
                    Pawn.DestroyHediff(parent);
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
                return !Props.hediffAndBodypart.NullOrEmpty();
            }
        }
        public void ApplyHediff(Pawn pawn)
        {
            for (int i = 0; i < Props.hediffAndBodypart.Count; i++)
            {
                HediffDef curHD = Props.hediffAndBodypart[i].hediff;
                BodyPartDef curBPD = Props.hediffAndBodypart[i].bodyPart;
                string curBPLabel = Props.hediffAndBodypart[i].bodyPartLabel;
                bool curAllowMissing = Props.hediffAndBodypart[i].allowMissing;
                bool regenIfMissing = Props.hediffAndBodypart[i].regenIfMissing;
                bool wholeBodyFallback = Props.hediffAndBodypart[i].wholeBodyFallback;

                if (curHD == null)
                {
                    Tools.Warn("cant find hediff; i=" + i, true);
                    continue;
                }
                
                BodyPartRecord myBPR = null;
                if(curBPLabel != null)
                {
                    myBPR = pawn.GetBPRecordWithoutHediff(curBPLabel, curHD, curAllowMissing, myDebug);
                    if (myBPR == null)
                    {
                        Tools.Warn("Could not find a BPR to apply hediff, will pick whole body?" + wholeBodyFallback, myDebug);
                        if(!wholeBodyFallback)
                            continue;
                    }
                }
                else if (curBPD != null)
                {
                    myBPR = pawn.GetBPRecordWithoutHediff(curBPD, curHD, curAllowMissing, myDebug);
                    if (myBPR == null)
                    {
                        Tools.Warn("Could not find a BPR to apply hediff, will pick whole body?" + wholeBodyFallback, myDebug);
                        if (!wholeBodyFallback)
                            continue;
                    }
                }

                if (curAllowMissing)
                {
                    if (Pawn.IsMissingBPR(myBPR, out Hediff hediffMissing))
                    {
                        if (regenIfMissing)
                        {
                            Tools.Warn("regenerating " + myBPR.customLabel, myDebug);
                            Pawn.health.RemoveHediff(hediffMissing);
                        }
                            
                    }
                }

                Hediff hediff2apply = HediffMaker.MakeHediff(curHD, pawn, myBPR);
                if (hediff2apply == null)
                {
                    Tools.Warn("cant create hediff " + curHD.defName + " to apply on " + curBPD.defName, true);
                    continue;
                }
                
                pawn.health.AddHediff(hediff2apply, myBPR, null);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!Tools.OkPawn(Pawn))
            {
                //Tools.DestroyParentHediff(parent, myDebug);
                return;
            }
                
            if (blockAction)
            {
                Tools.DestroyParentHediff(parent, myDebug);
                return;
            }

            if (HasHediffToApply)
                ApplyHediff(Pawn);

            // this hediff self destruction
            Pawn.DestroyHediff(parent);
            //Tools.DestroyParentHediff(parent, myDebug);
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
