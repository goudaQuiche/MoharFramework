using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;
using Ubet;

namespace YAHA
{
    /*
     * Todo: 
     * takeDamage trigger ; override postpostdamage
     */
    public class HediffComp_YetAnotherHediffApplier : HediffComp
    {
        Map myMap = null;
        public HediffCompProperties_YetAnotherHediffApplier Props => (HediffCompProperties_YetAnotherHediffApplier)props;

        public List<AssociatedHediffHistory> Registry = new List<AssociatedHediffHistory>();

        bool ShouldSkip = false;
        bool MyDebug => Props.debug;

        bool TriggeredOnlyHediffs = false;
        int UnspawnedGrace = 0;

        int UpdateNumthisTick = 0;

        public override void CompPostMake()
        {
            Init();
        }

        public void ApplyHediffAndRegisterSingleBodyPart(HediffItem hi, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, BodyPartRecord BPR = null, bool debug = false)
        {
            //Severity
            Hediff h = HediffMaker.MakeHediff(hi.hediff, Pawn, BPR);
            if (hi.HasSeverity)
                h.Severity = hi.severity.RandomInRange;

            //Applying hediff to full body if BPR is null
            Pawn.health.AddHediff(h, BPR, null);

            //Recording hediff applied in registry
            CurAHH.appliedHediffs.Add(h);

            if (CurHA.specifics.HasLimit)
                CurAHH.appliedNum++;

            // destroy parent hediff if discard upon remove setting and random is satisfied
            if (CurHA.specifics.HasDiscard && CurHA.specifics.discard.HasUponApplyDiscard && Rand.Chance(CurHA.specifics.discard.uponApply.chance.RandomInRange))
            {
                Pawn.health.RemoveHediff(parent);
            }
            // add grace destroy if grace setting upon remove and random is satisfied
            if (CurHA.specifics.HasGrace && CurHA.specifics.grace.HasUponApplyGrace && Rand.Chance(CurHA.specifics.grace.uponApply.chance.RandomInRange))
            {
                CurAHH.grace += CurHA.specifics.grace.uponApply.tickAmount.RandomInRange;
            }

            CurAHH.DidSomethingThisTick = true;
        }

        public void ApplyHediffAndRegisterWithBodyPartList(HediffItem hi, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, List<BodyPartRecord> BPRL, bool debug = false)
        {
            foreach(BodyPartRecord BPR in BPRL)
            {
                ApplyHediffAndRegisterSingleBodyPart(hi, CurHA, CurAHH, BPR, debug);
            }
        }

        public bool RemoveHediffAndDeregister(HediffAssociation CurHA, AssociatedHediffHistory CurAHH, bool debug = false)
        {
            if (debug) Log.Warning("Entering RemoveHediffAndDeregister");

            if (Pawn.TrunkNodeComputation(CurHA.condition.trunk, debug))
            {
                if (debug) Log.Warning("Exiting RemoveHediffAndDeregister : condition was true");
                return true;
            }

            if ( (CurHA.specifics != null) && (!CurHA.specifics.removeIfFalse))
                return false;

            if (CurAHH == null || CurAHH.appliedHediffs.NullOrEmpty())
                return false;

            //foreach(Hediff h in CurAHH.appliedHediffs)
            for (int j = CurAHH.appliedHediffs.Count - 1; j >= 0; j--)
            {
                Hediff h = CurAHH.appliedHediffs[j];
                // enough to remove the applied hediff ?? or need to retrieve it from pawn with bpr
                //Pawn.health.RemoveHediff(h);
                h.Severity = 0; //h.PostRemoved();
                
                // destroy parent hediff if discard upon remove setting and random is satisfied
                if(CurHA.specifics.HasDiscard && CurHA.specifics.discard.HasUponRemoveDiscard && Rand.Chance(CurHA.specifics.discard.uponRemove.chance.RandomInRange))
                {
                    // will it blend ?
                    Pawn.health.RemoveHediff(parent);
                    
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

            return false;

        }

        public IEnumerable<int> GetTriggeredHediffAssociationIndex(TriggerEvent te, bool debug = false)
        {
            for (int i = 0; i < Props.associations.Count; i++)
            {
                if (debug)
                    Log.Warning("i:" + i);

                if (Props.associations[i].specifics.IsTriggered && Props.associations[i].specifics.triggerEvent.Contains(te))
                    yield return i;
            }
            yield break;
        }

        public void UpdateHediffDependingOnConditionsItem(HediffAssociation CurHA, AssociatedHediffHistory CurAHH, bool ContinueIfTriggered = true, bool debug = false)
        {
            if (++UpdateNumthisTick > Props.UpdateNumthisTickLimit)
            {
                Log.Warning("Yaha has tried to update " + UpdateNumthisTick + " times during this tick. Limit is : "+ Props.UpdateNumthisTickLimit + ". Is there a recursion problem ?");
                //return;
            }

            if (CurAHH.DidSomethingThisTick)
            {
                if(debug)
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

            // pawn does not fulfil conditions : remove already applied hediffs and/or go for next hediff
            if (!RemoveHediffAndDeregister(CurHA, CurAHH, debug))
            {
                return;
            }

            List<BodyPartRecord> bodyPartRecords = null;
            // Hediff association has body parts specifications 
            if (CurHA.HasBodyPartToApplyHediff)
            {
                bodyPartRecords = Pawn.GetBP(CurHA.bodyPart, debug);

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
                        ApplyHediffAndRegisterSingleBodyPart(hi, CurHA, CurAHH, null, debug);
                    }
                }
                else if (CurHA.HasRandomHediffPool)
                {
                    //Multiple random hediffs ????
                    ApplyHediffAndRegisterSingleBodyPart(CurHA.randomHediffPool.PickRandomWeightedItem(), CurHA, CurAHH, null, debug);
                }
            }
            // target has multiple bodypart records as target
            else
            {
                if (CurHA.HasHediffPool)
                {
                    foreach (HediffItem hi in CurHA.hediffPool)
                    {
                        ApplyHediffAndRegisterWithBodyPartList(hi, CurHA, CurAHH, bodyPartRecords, debug);
                    }
                }
                else if (CurHA.HasRandomHediffPool)
                {
                    ApplyHediffAndRegisterWithBodyPartList(CurHA.randomHediffPool.PickRandomWeightedItem(debug), CurHA, CurAHH, bodyPartRecords, debug);
                }
            }
            if (CurHA.specifics.HasLimit && CurAHH.appliedNum > CurHA.specifics.applyNumLimit)
                CurAHH.done = true;
        }
        

        public void UpdateHediffDependingOnConditions(bool debug = false)
        {
            for (int i = 0; i < Props.associations.Count; i++)
            //foreach(HediffAssociation element in Props.associations)
            {
                HediffAssociation CurHA = Props.associations[i];
                AssociatedHediffHistory CurAHH = Registry[i];

                // To make a difference when update is called with either the frequency loop or the triggered event
                //CurAHH.DidSomethingThisTick = false;

                UpdateHediffDependingOnConditionsItem(CurHA, CurAHH, true, debug);
            }
        }
        /*
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
        }
        */

        public override void CompPostTick(ref float severityAdjustment)
        {
            UpdateNumthisTick = 0;

            if (UnspawnedGrace > 0)
            {
                UnspawnedGrace--;
                if (UnspawnedGrace == 0)
                    ShouldSkip = false;
            }

            if (ShouldSkip)
                return;

            if (Pawn.Spawned)
            {
                    if (myMap == null)
                    {
                        Init();
                        return;
                    }
            }
            else
            {
                if (MyDebug) Log.Warning(Pawn.Name + " : pawn unspawned - Entering grace");
                UnspawnedGrace = Props.UnspawnedGrace;
                ShouldSkip = true;
                return;
            }

            DidNothing();

            // no need to periodic check, since associations are event triggered
            if (TriggeredOnlyHediffs)
                return;

            if (Props.PeriodicCheck && Pawn.IsHashIntervalTick(Props.checkFrequency))
                UpdateHediffDependingOnConditions(MyDebug);
        }
        public void DidNothing()
        {
            foreach (AssociatedHediffHistory curAHH in Registry)
            {
                curAHH.DidSomethingThisTick = false;
            }
        }

        public void Init()
        {
            if (MyDebug) Log.Warning("Entering Init");

            myMap = Pawn.Map;

            // pawn is not spawned ? caravaning ? from another faction ?
            if (!Pawn.Spawned)
            {
                if (MyDebug) Log.Warning("Null map");
                //shouldSkip = true;
                return;
            }

            if(Registry.NullOrEmpty())
            foreach(HediffAssociation ha in Props.associations)
            {
                if (MyDebug)
                    Log.Warning("Added 1 HediffAssociation");

                Registry.Add(new AssociatedHediffHistory());
            }

            TriggeredOnlyHediffs = Props.associations.All(a => a.specifics.IsTriggered);
            if (MyDebug)
                Log.Warning("TriggeredOnlyHediffs:" + TriggeredOnlyHediffs);

            /*
             * if (TriggeredOnlyHediffs)
                return;
                */

            UpdateHediffDependingOnConditions(MyDebug);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref TriggeredOnlyHediffs, "TriggeredOnlyHediffs");

            Scribe_Collections.Look(ref Registry, "Registry");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && Registry == null)
            {
                Registry = new List<AssociatedHediffHistory>();
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (Props.debug)
                {
                    result +=
                        Pawn.PawnResumeString() +
                        (Props.associations.NullOrEmpty() ? "empty association" : Props.associations.Count + " hediff associations") +
                        (Registry.NullOrEmpty()?"empty registry":Registry.Count + " registered hediff associations")
                        //+ "; hasAssociationHediffMaster: " + myPawn.Has_OHPLS()
                       ;
                }

                return result;
            }
        }

    }
}
