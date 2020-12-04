using Verse;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class HediffComp_MultipleHediff : HediffComp
    {
        bool MyDebug => Props.debug;
        string DebugStr => MyDebug ? $"{Pawn.LabelShort} MultipleHediff {parent.def.defName} - " : "";

        bool blockAction = false;

        bool HasBodyRequirement => Props.bodyDef != null;

        public HediffCompProperties_MultipleHediff Props => (HediffCompProperties_MultipleHediff)this.props;
        
        public void CheckProps()
        {
            string fctN = DebugStr + "CheckProps - ";
            if(!HasHediffToApply)
            {
                Tools.Warn(fctN + "- empty hediffAndBodypart, destroying", MyDebug);
                Pawn.DestroyHediff(parent);
                blockAction = true;
            }

            if (HasBodyRequirement)
                if (Pawn.def.race.body != Props.bodyDef)
                {
                    Tools.Warn(fctN + " has not a bodyDef like required: " + Pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), true);
                    Pawn.DestroyHediff(parent);
                    blockAction = true;
                }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            Tools.Warn(DebugStr + "CompPostMake", MyDebug);

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
            string fctN = DebugStr + "CheckProps - ";

            for (int i = 0; i < Props.hediffAndBodypart.Count; i++)
            {
                HediffDef curHD = Props.hediffAndBodypart[i].hediff;
                BodyPartDef curBPD = Props.hediffAndBodypart[i].bodyPart;
                string curBPLabel = Props.hediffAndBodypart[i].bodyPartLabel;

                bool prioritizeMissing = Props.hediffAndBodypart[i].prioritizeMissing;
                bool allowMissing = Props.hediffAndBodypart[i].allowMissing;
                bool regenIfMissing = Props.hediffAndBodypart[i].regenIfMissing;
                bool forbidAddedPart = Props.hediffAndBodypart[i].forbidAddedPart;

                bool wholeBodyFallback = Props.hediffAndBodypart[i].wholeBodyFallback;

                if (curHD == null)
                {
                    Tools.Warn(fctN + "cant find hediff; i=" + i, true);
                    continue;
                }
                
                BodyPartRecord myBPR = null;
                if(curBPLabel != null)
                {
                    Tools.Warn(fctN + "Trying to retrieve BPR with label", MyDebug);
                    myBPR = pawn.GetBPRecordWithoutHediff(curBPLabel, curHD, allowMissing, prioritizeMissing, forbidAddedPart, MyDebug);
                    if (myBPR == null)
                    {
                        Tools.Warn(fctN + "Could not find a BPR to apply hediff, will pick whole body?" + wholeBodyFallback, MyDebug);
                        if(!wholeBodyFallback)
                            continue;
                    }
                }
                else if (curBPD != null)
                {
                    Tools.Warn(fctN + "Trying to retrieve BPR with def", MyDebug);
                    myBPR = pawn.GetBPRecordWithoutHediff(curBPD, curHD, allowMissing, prioritizeMissing, forbidAddedPart, MyDebug);
                    if (myBPR == null)
                    {
                        Tools.Warn(fctN + "Could not find a BPR to apply hediff, will pick whole body?" + wholeBodyFallback, MyDebug);
                        if (!wholeBodyFallback)
                            continue;
                    }
                }

                if (allowMissing && regenIfMissing)
                {
                    if (Pawn.IsMissingBPR(myBPR, out Hediff hediffMissing))
                    {
                        Tools.Warn(fctN + "regenerating " + myBPR.customLabel, MyDebug);
                        Pawn.health.RemoveHediff(hediffMissing);
                    }
                }

                Hediff hediff2apply = HediffMaker.MakeHediff(curHD, pawn, myBPR);
                if (hediff2apply == null)
                {
                    Tools.Warn(fctN + "cant create hediff " + curHD.defName + " to apply on " + curBPD.defName, true);
                    continue;
                }
                
                pawn.health.AddHediff(hediff2apply, myBPR, null);

                Tools.Warn(fctN + "Applied "+ curHD.defName, MyDebug);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligeable())
            {
                //Tools.DestroyParentHediff(parent, myDebug);
                return;
            }
                
            if (blockAction)
            {
                Tools.DestroyParentHediff(parent, MyDebug);
                return;
            }

            if (HasHediffToApply)
                ApplyHediff(Pawn);

            // this hediff self destruction
            Pawn.DestroyHediff(parent, MyDebug);
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
