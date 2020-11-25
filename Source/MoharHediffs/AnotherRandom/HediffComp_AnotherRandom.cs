using RimWorld;
using System;
using System.Linq;
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
    public class HediffComp_AnotherRandom : HediffComp
    {
        bool blockAction = false;

        public bool HasItems => Props.HasHediffPool;
        public int ItemNum => Props.hediffPool.Count;

        public bool MyDebug => Props.debug;
        public bool LowVerbosity => Props.debug && Props.verbosity >= 1;
        public bool MediumVerbosity => Props.debug && Props.verbosity >= 2;
        public bool HighVerbosity => Props.debug && Props.verbosity >= 3;

        public HediffCompProperties_AnotherRandom Props
        {
            get
            {
                return (HediffCompProperties_AnotherRandom)this.props;
            }
        }

        public void DumpProps()
        {
            string fctN = "CheckProps";
            if (!HasItems)
            {
                Tools.Warn(fctN + "- HediffComp_AnotherRandom; no item found", MyDebug);
            }
            else
                Tools.Warn(fctN + "- HediffComp_AnotherRandom; found " + ItemNum + " items", MyDebug);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            string debugStr = MyDebug ? Pawn.LabelShort + " CompPostMake - " : "";

            if (MyDebug)
                DumpProps();

            if (!HasItems)
            {
                Tools.Warn(debugStr + " found no item to work with, destroying ", MyDebug);
                //Tools.DestroyParentHediff(parent, MyDebug);
                Pawn.DestroyHediff(parent, MyDebug);
                blockAction = true;
                return;
            }

            Tools.Warn(debugStr + " found " + ItemNum + " items to work with", MyDebug);

            if (Props.HasConditionsToApplyHediffs)
            {
                if (
                    !Props.conditionsToApplyHediffs.pawn.ValidateCompatibilityOfHediffWithPawn(Pawn, MyDebug) ||
                    !Props.conditionsToApplyHediffs.bodyPart.GetBPRFromHediffCondition(Pawn, out _, MyDebug)
                    )
                {
                    Tools.Warn(debugStr + " is not compatible with this hediff, destroying ", MyDebug);
                    //Tools.DestroyParentHediff(parent, MyDebug);
                    Pawn.DestroyHediff(parent, MyDebug);
                    blockAction = true;
                    return;
                }
            } else
                Tools.Warn(debugStr + " skipped HasConditionsToApplyHediffs", MyDebug);

            Tools.Warn(debugStr + " checking if at least 1 hediff from " + ItemNum + " is appliable", MyDebug);

            if (!this.InitialHediffConditionCheck(MyDebug)) {
                Tools.Warn(debugStr + " has found no appliable item, destroying ", MyDebug);
                //Tools.DestroyParentHediff(parent, MyDebug);
                Pawn.DestroyHediff(parent, MyDebug);
                blockAction = true;
                return;
            }
            else
                Tools.Warn(debugStr + " found at least 1 appliable hediff", MyDebug);
        }

        public void ApplyHediff(Pawn pawn)
        {
            string debugStr = MyDebug ? Pawn.LabelShort + " - " + parent.def.defName + " - ApplyHediff" : "";

            List<HediffItem> AlreadyPickedItems = new List<HediffItem>();
            int iterationNum = Props.hediffToApplyNumRange.RandomInRange;

            List<HediffItem> compatibleItems = this.GetCompatibleItems();
            if (compatibleItems.NullOrEmpty())
                return;

            Tools.Warn(debugStr + "Trying to apply " + iterationNum + " hediffs among " + compatibleItems.Count + " options pool", MyDebug);

            for (int i = 0; i < iterationNum; i++)
            {
                string debugIteration = MyDebug ? "[" + i + "/" + iterationNum + "]" : "";

                if (!AlreadyPickedItems.NullOrEmpty())
                {
                    compatibleItems = compatibleItems.GetRemainingItems(AlreadyPickedItems);
                    if (compatibleItems.NullOrEmpty())
                        return;
                }
                Tools.Warn(debugStr + debugIteration + " " + compatibleItems.Count + " options remaining ", MyDebug);

                HediffItem hediffItem = compatibleItems.PickRandomWeightedItem();
                if (hediffItem == null)
                {
                    Tools.Warn(debugStr + debugIteration + " null hediffItem, giving up ", MyDebug);
                    return;
                }

                Tools.Warn(debugStr + debugIteration + " found a hediffItem:" + hediffItem?.hediffDef + ", going on ", MyDebug);

                float randomChanceToApply = hediffItem.applyChance.RandomInRange;
                if (!Rand.Chance(randomChanceToApply))
                {
                    Tools.Warn(debugStr + debugIteration + " rand(" + randomChanceToApply + ") == false, nothing is applied", MyDebug);

                    if (Props.excludePickedItems && Props.excludeRandomlyNotApplied)
                        AlreadyPickedItems.Add(hediffItem);
                    continue;
                }
                Tools.Warn(debugStr + debugIteration + " rand(" + randomChanceToApply + ") == true, hediff:"+ hediffItem?.hediffDef+" will be applied", MyDebug);

                HediffDef curHD = hediffItem.hediffDef;
                if (curHD == null)
                {
                    Tools.Warn(debugStr + debugIteration + "cant find hediff, giving up", MyDebug);
                    return;
                }

                HediffCondition calculatedHC = 
                    ConditionBuilder.GetDefaultPlusSpecificHediffCondition(Props?.defaultCondition ?? null, hediffItem?.specificCondition ?? null, HighVerbosity);

                if (!calculatedHC.bodyPart.GetBPRFromHediffCondition(Pawn, out BodyPartRecord curBPR, MyDebug))
                {
                    Tools.Warn(debugStr + debugIteration + " could not find anything suitable, giving up", MyDebug);
                    return;
                }

                Hediff hediff2apply = HediffMaker.MakeHediff(curHD, pawn, curBPR);
                if (hediff2apply == null)
                {
                    Tools.Warn(debugStr + debugIteration + "cant create hediff", MyDebug);
                    return;
                }

                hediff2apply.Severity = hediffItem.severity.RandomInRange;

                Tools.Warn(
                    debugStr + debugIteration + " Applying " +
                    "hediff:" + curHD.defName +
                    "; bpr:" + (curBPR == null ? "body" : curBPR.def.defName) +
                    "; severity:" + hediff2apply.Severity
                    , MyDebug
                );

                pawn.health.AddHediff(hediff2apply, curBPR, null);

                if(Props.excludePickedItems)
                    AlreadyPickedItems.Add(hediffItem);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
            {
                //Tools.DestroyParentHediff(parent, MyDebug);
                Pawn.DestroyHediff(parent, MyDebug);
                return;
            }
                
            if (blockAction)
            {
                //Tools.DestroyParentHediff(parent, MyDebug);
                Pawn.DestroyHediff(parent, MyDebug);
                return;
            }

            if (HasItems)
            {
                ApplyHediff(pawn);
            }

            // this hediff self destruction
            Pawn.DestroyHediff(parent, MyDebug);
            //Tools.DestroyParentHediff(parent, MyDebug);
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
