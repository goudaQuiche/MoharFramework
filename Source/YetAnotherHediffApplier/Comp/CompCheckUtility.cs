using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;
using Ubet;

namespace YAHA
{
    public static class CompCheckUtility
    {
        public static void CheckAllHediffAssociations(this HediffComp_YetAnotherHediffApplier c)
        {
            for (int i = 0; i < c.Props.associations.Count; i++)
            //foreach(HediffAssociation element in Props.associations)
            {
                HediffAssociation CurHA = c.Props.associations[i];
                AssociatedHediffHistory CurAHH = c.Registry[i];

                c.CheckSingleHediffAssociation(CurHA, CurAHH, true);
            }
        }


        public static void CheckSingleHediffAssociation(this HediffComp_YetAnotherHediffApplier c, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, bool ContinueIfTriggered = true)
        {
            if (++c.UpdateNumthisTick > c.Props.UpdateNumthisTickLimit)
            {
                if (c.MyDebug)
                    Log.Warning("Yaha has tried to update " + c.UpdateNumthisTick + " times during this tick. Limit is : " + c.Props.UpdateNumthisTickLimit + ". Is there a recursion problem ?");
                //return;
            }

            if (CurAHH.DidSomethingThisTick)
            {
                if (c.MyDebug)
                    Log.Warning("Yaha registry says something has already been done this tick; exiting");
                return;
            }

            // association has already applied needed hediffs in the past, skipping
            if (CurAHH.done) return;

            // triggered by harmony patch; no need to check it
            if (ContinueIfTriggered && CurHA.specifics.IsTriggered) return;

            if (CurAHH.HasGraceTime)
            {
                CurAHH.grace--;
                return;
            }

            bool conditionCheck = c.Pawn.TrunkNodeComputation(CurHA.condition.trunk, c.MyDebug);

            // pawn does not fulfil conditions
            if (!conditionCheck)
            {
                // remove already applied hediffs and/or go for next hediff
                c.RemoveHediffAndDeregister(CurHA, CurAHH);
                return;
            }

            List<BodyPartRecord> bodyPartRecords = null;
            // Hediff association has body parts specifications 
            if (CurHA.HasBodyPartToApplyHediff)
            {
                bodyPartRecords = c.Pawn.GetBP(CurHA.bodyPart, c.MyDebug);

                //but pawn does not have any legit body part
                if (bodyPartRecords.NullOrEmpty()) return;
            }

            // target is full body
            if (bodyPartRecords.NullOrEmpty())
            {
                if (CurHA.HasHediffPool)
                {
                    foreach (HediffItem hi in CurHA.hediffPool)
                    {
                        c.ApplyHediffAndRegisterSingleBodyPart(hi, CurHA, CurAHH, null);
                    }
                }
                else if (CurHA.HasRandomHediffPool)
                {
                    //Multiple random hediffs ????
                    c.ApplyHediffAndRegisterSingleBodyPart(CurHA.randomHediffPool.PickRandomWeightedItem(), CurHA, CurAHH, null);
                }
            }
            // target has multiple bodypart records as target
            else
            {
                if (CurHA.HasHediffPool)
                {
                    foreach (HediffItem hi in CurHA.hediffPool)
                    {
                        c.ApplyHediffAndRegisterWithBodyPartList(hi, CurHA, CurAHH, bodyPartRecords);
                    }
                }
                else if (CurHA.HasRandomHediffPool)
                {
                    c.ApplyHediffAndRegisterWithBodyPartList(CurHA.randomHediffPool.PickRandomWeightedItem(c.MyDebug), CurHA, CurAHH, bodyPartRecords);
                }
            }
            if (CurHA.specifics.HasLimit && CurAHH.appliedNum > CurHA.specifics.applyNumLimit)
                CurAHH.done = true;
        }

        public static void ApplyHediffAndRegisterSingleBodyPart(this HediffComp_YetAnotherHediffApplier c, HediffItem hi, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, BodyPartRecord BPR = null)
        {
            //Severity
            Hediff h = HediffMaker.MakeHediff(hi.hediff, c.Pawn, BPR);
            if (hi.HasSeverity)
                h.Severity = hi.severity.RandomInRange;

            //Applying hediff to full body if BPR is null
            c.Pawn.health.AddHediff(h, BPR, null);

            //Recording hediff applied in registry
            CurAHH.appliedHediffs.Add(h);

            if (CurHA.specifics.HasLimit)
                CurAHH.appliedNum++;

            // destroy parent hediff if discard upon remove setting and random is satisfied
            if (CurHA.specifics.HasDiscard && CurHA.specifics.discard.HasUponApplyDiscard && Rand.Chance(CurHA.specifics.discard.uponApply.chance.RandomInRange))
            {
                c.Pawn.health.RemoveHediff(c.parent);
            }
            // add grace destroy if grace setting upon remove and random is satisfied
            if (CurHA.specifics.HasGrace && CurHA.specifics.grace.HasUponApplyGrace && Rand.Chance(CurHA.specifics.grace.uponApply.chance.RandomInRange))
            {
                CurAHH.grace += CurHA.specifics.grace.uponApply.tickAmount.RandomInRange;
            }

            CurAHH.DidSomethingThisTick = true;
        }

        public static void ApplyHediffAndRegisterWithBodyPartList(this HediffComp_YetAnotherHediffApplier c, HediffItem hi, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, List<BodyPartRecord> BPRL)
        {
            foreach (BodyPartRecord BPR in BPRL)
            {
                c.ApplyHediffAndRegisterSingleBodyPart(hi, CurHA, CurAHH, BPR);
            }
        }

        public static bool RemoveHediffAndDeregister(this HediffComp_YetAnotherHediffApplier c, HediffAssociation CurHA, AssociatedHediffHistory CurAHH)
        {
            if (c.MyDebug) Log.Warning("Entering RemoveHediffAndDeregister");

            // no hediff removed upon check fail, exit
            if (CurHA.HasSpecifics && (!CurHA.specifics.removeIfFalse))
                return false;

            // no applied hediff before, exit
            if (CurAHH == null || !CurAHH.HasAppliedHediffs)
                return false;

            //foreach(Hediff h in CurAHH.appliedHediffs)
            for (int j = CurAHH.appliedHediffs.Count - 1; j >= 0; j--)
            {
                Hediff h = CurAHH.appliedHediffs[j];
                // enough to remove the applied hediff ?? or need to retrieve it from pawn with bpr
                //Pawn.health.RemoveHediff(h);
                h.Severity = 0; //h.PostRemoved();

                // destroy parent hediff if discard upon remove setting and random is satisfied
                if (CurHA.specifics.HasDiscard && CurHA.specifics.discard.HasUponRemoveDiscard && Rand.Chance(CurHA.specifics.discard.uponRemove.chance.RandomInRange))
                {
                    // will it blend ?
                    c.Pawn.health.RemoveHediff(c.parent);

                }
                // add grace destroy if grace setting upon remove and random is satisfied
                if (CurHA.specifics.HasGrace && CurHA.specifics.grace.HasUponRemoveGrace && Rand.Chance(CurHA.specifics.grace.uponRemove.chance.RandomInRange))
                {
                    CurAHH.grace += CurHA.specifics.grace.uponRemove.tickAmount.RandomInRange;
                }

                // Deregister
                //h.Severity = 0; h.PostRemoved();
                CurAHH.appliedHediffs.RemoveAt(j);

                CurAHH.DidSomethingThisTick = true;

            }

            return true;
        }

        public static IEnumerable<int> GetTriggeredHediffAssociationIndex(this HediffComp_YetAnotherHediffApplier c, TriggerEvent te, bool debug = false)
        {
            for (int i = 0; i < c.Props.associations.Count; i++)
            {
                if (debug)
                    Log.Warning("i:" + i);

                if (c.Props.associations[i].specifics.IsTriggered && c.Props.associations[i].specifics.triggerEvent.Contains(te))
                    yield return i;
            }
            yield break;
        }
    }
}
