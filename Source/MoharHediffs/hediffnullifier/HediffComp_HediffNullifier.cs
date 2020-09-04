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
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
    public class HediffComp_HediffNullifier : HediffComp
    {
        const int tickLimiterModulo = 60;
        int PawnHash;
        private int LimitedUsageNumber = 0;

        bool BlockPostTick = false;

        readonly bool myDebug = false;

        //Pawn myPawn => parent.pawn;
        public HediffCompProperties_HediffNullifier Props => (HediffCompProperties_HediffNullifier) props;

        public bool RequiresAtLeastOneBodyPart => !Props.RequiredBodyPart.NullOrEmpty();

        public bool HasHediffToNullify
        {
            get
            {
                return !Props.hediffToNullify.NullOrEmpty();
            }
        }
        public bool HasLimitedUsage
        {
            get
            {
                return (Props.limitedUsageNumber != -99);
            }
        }

        public void BlockAndDestroy()
        {
            Tools.DestroyParentHediff(parent, myDebug);
            BlockPostTick = true;
        }

        public void SetPawnHash()
        {
            PawnHash = Pawn.thingIDNumber % 30;
        }


        public override void CompPostMake()
        {
            //base.CompPostMake();

            Tools.Warn(">>>" + parent.def.defName + " - CompPostMake start", myDebug);

            SetPawnHash();

            if (!HasHediffToNullify)
            {
                Tools.Warn(parent.def.defName + " has no hediff to nullify, autokill", myDebug);
                BlockAndDestroy();
            }

            DestroyHediffIfMissingBP();

            if (HasLimitedUsage)
                LimitedUsageNumber = Props.limitedUsageNumber;
        }

        public void DestroyHediffIfMissingBP()
        {
            if (RequiresAtLeastOneBodyPart)
            {
                bool IsOk = false;
                foreach (BodyPartDef bodyPartDef in Props.RequiredBodyPart)
                {
                    if ((IsOk = Pawn.CheckIfExistingNaturalBP(bodyPartDef)) == true)
                        break;
                }
                if (!IsOk)
                {
                    Tools.Warn(Pawn.LabelShort + " does not have any required body part to have an active " + parent.def.defName + ", autokill", myDebug);
                    BlockAndDestroy();
                }
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref LimitedUsageNumber, "LimitedUsageNumber");
            Scribe_Values.Look(ref PawnHash, "PawnHash");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Find.TickManager.TicksGame % (tickLimiterModulo + PawnHash) != 0)
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

            foreach (Hediff curHediff in Pawn.health.hediffSet.hediffs)
            {
                Tools.Warn(Pawn.Label + " - " + curHediff.def.defName, myDebug);
                foreach (HediffDef curHediffToNullify in Props.hediffToNullify)
                {
                    if (curHediff.def == curHediffToNullify)
                    {

                        curHediff.Severity = 0;
                        Tools.Warn(curHediff.def.defName + " severity = 0", myDebug);

                        if (HasLimitedUsage)
                        {
                            LimitedUsageNumber--;
                            if (LimitedUsageNumber <= 0)
                            {
                                Tools.Warn(parent.def.defName + " has reached its limit usage, autokill", myDebug);
                                Tools.DestroyParentHediff(parent, myDebug);
                            }
                        }
                    }
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
