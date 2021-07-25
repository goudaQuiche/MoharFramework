using RimWorld;
using System;
using System.Linq;
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
    public class HediffComp_OnTheCarpet : HediffComp
    {
        public bool HasItems => Props.HasHediffPool;
        public int ItemCount => Props.ItemCount;

        public bool MyDebug => Props.debug;
        bool blockAction = false;
        int graceTime = 999;

        public bool foundFault = false;

        public bool IsTimeToCheck => Pawn.IsHashIntervalTick(Props.checkPeriod);

        public HediffCompProperties_OnTheCarpet Props => (HediffCompProperties_OnTheCarpet)props;

        public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            Tools.Warn(ErrorLog, myDebug && !ErrorLog.NullOrEmpty());
            blockAction = true;
            Pawn.DestroyHediff(parent, MyDebug);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            string debugStr = MyDebug ? $"{Pawn.LabelShort} {parent.def.defName} CompPostMake - " : "";

            if (!HasItems)
            {
                Tools.Warn(debugStr + " found no item to work with, destroying ", MyDebug);
                Pawn.DestroyHediff(parent, MyDebug);
                blockAction = true;
                return;
            }
            Tools.Warn(debugStr + " found " + ItemCount + " items to work with", MyDebug);

            if (!Props.IsPawnNeedCompatible(Pawn))
            {
                Tools.Warn(debugStr + " is not compatible with this hediff, destroying ", MyDebug);

                Pawn.DestroyHediff(parent, MyDebug);
                blockAction = true;
                return;
            }

            SetGraceTime();

            Tools.Warn(debugStr + " found something to do", MyDebug);
        }

        public void SetGraceTime()
        {
            graceTime = Props.graceTimeBase;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref graceTime, "graceTime");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (Pawn.Negligible())
                return;

            if (blockAction)
            {
                //Pawn.DestroyHediff(parent, MyDebug);
                return;
            }

            if (graceTime-- > 0)
                return;

            bool SelfDestruction = false;

            if (IsTimeToCheck)
            {
                SelfDestruction = !this.TreatRelevantHediffsAndReportIfStillHediffsToCheck();
            }

            // this hediff self destruction
            if (SelfDestruction)
                Pawn.DestroyHediff(parent, MyDebug);

        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                
                result += "This should not disappear until an hediff is still there";
                
                return result;
            }
        }
    }
}
