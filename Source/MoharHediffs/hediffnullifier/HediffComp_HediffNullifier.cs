using Verse;
using RimWorld;
using System.Linq;

namespace MoharHediffs
{
    public class HediffComp_HediffNullifier : HediffComp
    {
        private int LimitedUsageNumber = 0;

        bool BlockPostTick = false;

        readonly bool myDebug = false;

        //Pawn myPawn => parent.pawn;
        public HediffCompProperties_HediffNullifier Props => (HediffCompProperties_HediffNullifier) props;

        public bool RequiresAtLeastOneBodyPart => !Props.RequiredBodyPart.NullOrEmpty();
        public bool HasMessageToDisplay => Props.showMessage && !Props.nullifyKey.NullOrEmpty();
        public bool DisplayLimitedUsageLeft => HasMessageToDisplay && Props.concatUsageLimit && !Props.limitedKey.NullOrEmpty();

        public bool HasHediffToNullify => !Props.hediffToNullify.NullOrEmpty();
        public bool HasLimitedUsage => (Props.limitedUsageNumber != -99);


        public void BlockAndDestroy()
        {
            Tools.DestroyParentHediff(parent, myDebug);
            BlockPostTick = true;
        }
               
        public override void CompPostMake()
        {
            if (!StaticCheck.IsOk)
                BlockAndDestroy();

            //base.CompPostMake();
            if (myDebug)Log.Warning(">>>" + parent.def.defName + " - CompPostMake start");

            if (!HasHediffToNullify)
            {
                if (myDebug) Log.Warning(parent.def.defName + " has no hediff to nullify, autokill");
                BlockAndDestroy();
            }

            DestroyHediffIfMissingBP();

            if (HasLimitedUsage)
                LimitedUsageNumber = Props.limitedUsageNumber;
        }

        public void DestroyHediffIfMissingBP()
        {
            if (!RequiresAtLeastOneBodyPart)
                return;

            bool IsOk = false;
            foreach (BodyPartDef bodyPartDef in Props.RequiredBodyPart)
            {
                if ((IsOk = Pawn.CheckIfExistingNaturalBP(bodyPartDef)) == true)
                    break;
            }
            if (!IsOk)
            {
                if (myDebug) Log.Warning(Pawn.LabelShort + " does not have any required body part to have an active " + parent.def.defName + ", autokill");
                BlockAndDestroy();
            }

        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref LimitedUsageNumber, "LimitedUsageNumber");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            //if (Find.TickManager.TicksGame % (tickLimiterModulo + PawnHash) != 0)
            if (!Pawn.IsHashIntervalTick(Props.checkPeriod))
            {
                //Tools.Warn("hediff Nullifier - Chilling", myDebug);
                return;
            }

            if (!Tools.OkPawn(Pawn))
                return;

            DestroyHediffIfMissingBP();

            if (BlockPostTick)
            {
                return;
            }

            //Tools.Warn("hediff Nullifier - Working", myDebug);

            foreach (Hediff curHediff in Pawn.health.hediffSet.hediffs.Where(h => Props.hediffToNullify.Contains(h.def)))
            {
                if (myDebug) Log.Warning(Pawn.Label + " - " + curHediff.def.defName, myDebug);

                curHediff.Severity = 0;
                //Pawn.DestroyHediff(curHediff);
                if (myDebug) Log.Warning(curHediff.def.defName + " severity = 0");

                if (HasLimitedUsage)
                {
                    LimitedUsageNumber--;
                    if (LimitedUsageNumber <= 0)
                    {
                        if (myDebug) Log.Warning(parent.def.defName + " has reached its limit usage, autokill");
                        Tools.DestroyParentHediff(parent, myDebug);
                    }
                }

                if (HasMessageToDisplay)
                {
                    string myMessage;
                    myMessage = Props.nullifyKey.Translate(Pawn.LabelShort, curHediff.def.label, Pawn.gender.GetPronoun(), Pawn.kindDef.race.label);
                    if (DisplayLimitedUsageLeft)
                        myMessage += Props.limitedKey.Translate(LimitedUsageNumber);
                    myMessage += ".";

                    Messages.Message(myMessage, MessageTypeDefOf.NeutralEvent);
                }
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
                foreach (HediffDef hediffName in Props.hediffToNullify)
                {
                    result += hediffName.label + "; ";
                }

                if (!HasLimitedUsage)
                    result += (" for ever");
                else
                    result += (" " + LimitedUsageNumber + " left");

                return result;
            }
        }
    }
}
