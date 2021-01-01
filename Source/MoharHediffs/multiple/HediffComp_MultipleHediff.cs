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

        bool HasSingleBodyRequirement => Props.bodyDef != null;
        bool HasWhiteList => !Props.bodyDefWhiteList.NullOrEmpty();
        bool HasBlackList => !Props.bodyDefBlackList.NullOrEmpty();

        bool WhiteListCompliant => HasWhiteList ? Props.bodyDefWhiteList.Contains(Pawn.def.race.body) : true;
        bool BlackListCompliant => HasBlackList ? !Props.bodyDefBlackList.Contains(Pawn.def.race.body) : true;
        bool HasAccessList => HasWhiteList || HasBlackList;

        public HediffCompProperties_MultipleHediff Props => (HediffCompProperties_MultipleHediff)this.props;
        public bool HasHediffToApply => !Props.hediffAndBodypart.NullOrEmpty();

        public void CheckProps()
        {
            string fctN = DebugStr + "CheckProps - ";
            if (!HasHediffToApply)
            {
                Tools.Warn(fctN + "- empty hediffAndBodypart, destroying", MyDebug);
                Pawn.DestroyHediff(parent);
                blockAction = true;
            }

            if (HasSingleBodyRequirement && (Pawn.def.race.body != Props.bodyDef))
            {
                Tools.Warn(fctN + " has not a bodyDef like required: " + Pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), MyDebug);
                Pawn.DestroyHediff(parent);
                blockAction = true;
            }

            if (HasAccessList)
            {
                bool BlackIsOk = BlackListCompliant;
                bool WhiteIsOk = WhiteListCompliant;
                if (!BlackIsOk || !WhiteIsOk)
                {
                    if (MyDebug)
                    {
                        Log.Warning(
                            fctN +
                            (HasWhiteList ? $"Props.BodyDefWhiteList contains {Props.bodyDefWhiteList.Count} elements" : "No whitelist") + ", compliant: " + WhiteIsOk +
                            "; " + (HasBlackList ? $"Props.BodyDefBlackList contains {Props.bodyDefBlackList.Count} elements" : "No blacklist") + ", compliant:" + BlackIsOk
                        );
                    }
                    Pawn.DestroyHediff(parent);
                    blockAction = true;
                }
                else
                {
                    if (MyDebug)
                        Log.Warning(fctN + " AccessList compliant ok");
                }
            }

            if (Props.hediffAndBodypart.Any(habp => habp.bodyPart != null && habp.bodyPartLabel != null))
            {
                Tools.Warn(fctN + "at least one item has both a bodypart def and a bodypart label, label will be prioritized", MyDebug);
            }

            if (Props.hediffAndBodypart.Any(habp => habp.hediff == null))
            {
                Tools.Warn(fctN + "at least one item has no hediff defined. What will happen ?", MyDebug);
            }
        }

        public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            Tools.Warn(ErrorLog, myDebug && !ErrorLog.NullOrEmpty());
            blockAction = true;
            Tools.DestroyParentHediff(parent, myDebug);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            Tools.Warn(DebugStr + "CompPostMake", MyDebug);

            if (ModCompatibilityCheck.MoharCheckAndDisplay() == false)
                BlockAndDestroy();

            CheckProps();
        }

        public void ApplyHediff(Pawn pawn)
        {
            string fctN = DebugStr + "ApplyHediff - ";

            for (int i = 0; i < Props.hediffAndBodypart.Count; i++)
            {
                HediffDef curHD = Props.hediffAndBodypart[i].hediff;

                BodyPartDef curBPD = Props.hediffAndBodypart[i].bodyPart;
                string curBPLabel = Props.hediffAndBodypart[i].bodyPartLabel;

                bool prioritizeMissing = Props.hediffAndBodypart[i].prioritizeMissing;
                bool allowMissing = Props.hediffAndBodypart[i].allowMissing;
                bool regenIfMissing = Props.hediffAndBodypart[i].regenIfMissing;
                bool allowAddedPart = Props.hediffAndBodypart[i].allowAddedPart;

                bool wholeBodyFallback = Props.hediffAndBodypart[i].wholeBodyFallback;

                if (curHD == null)
                {
                    Tools.Warn(fctN + "cant find hediff; i=" + i, true);
                    continue;
                }

                BodyPartRecord myBPR = null;
                if (curBPLabel != null || curBPD != null)
                {
                    Tools.Warn(fctN + "Trying to retrieve BPR with [BP label]:" + curBPLabel + " [BP def]:" + curBPD?.defName, MyDebug);
                    myBPR = pawn.GetBPRecordWithoutHediff(curBPLabel, curBPD, curHD, allowMissing, prioritizeMissing, allowAddedPart, MyDebug);

                }

                if (myBPR == null)
                {
                    Tools.Warn(fctN + "Could not find a BPR to apply hediff, will pick whole body?" + wholeBodyFallback, MyDebug);
                    if (!wholeBodyFallback)
                        continue;
                }

                if (allowMissing && regenIfMissing && myBPR!=null)
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

                pawn.health.AddHediff(hediff2apply, myBPR);

                Tools.Warn(fctN + "Applied " + curHD.defName, MyDebug);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligible())
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
