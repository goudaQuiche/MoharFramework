using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;
using Ubet;

namespace YAHA
{
    /*
     * Todo: 
     * 
     * method to check a triggered condition
     * wear/drop patch
     * draft/undraft patch
     * grace
     * grace per hediff ??
     * discard
     * discard per hediff ??
     */
    public class HediffComp_YetAnotherHediffApplier : HediffComp
    {
        Map myMap = null;
        public HediffCompProperties_YetAnotherHediffApplier Props => (HediffCompProperties_YetAnotherHediffApplier)props;

        public List<AssociatedHediffHistory> Registry = new List<AssociatedHediffHistory>();

        bool shouldSkip = false;
        bool MyDebug => Props.debug;

        bool TriggeredOnlyHediffs = false;

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
            if (debug) Log.Warning("0");
            if (Pawn.TrunkNodeComputation(CurHA.condition.trunk, debug))
                return true;

            if (debug) Log.Warning("1");
            if ( (CurHA.specifics != null) && (!CurHA.specifics.removeIfFalse))
                return false;

            if (debug) Log.Warning("2");
            if (CurAHH == null || CurAHH.appliedHediffs.NullOrEmpty())
                return false;

            if (debug) Log.Warning("3");
            //foreach(Hediff h in CurAHH.appliedHediffs)
            for (int j = CurAHH.appliedHediffs.Count - 1; j >= 0; j--)
            {
                Hediff h = CurAHH.appliedHediffs[j];
                // enough to remove the applied hediff ?? or need to retrieve it from pawn with bpr
                h.Severity = 0; h.PostRemoved();
                CurAHH.appliedHediffs.RemoveAt(j);
            }

            return false;

        }

        public void UpdateHediffDependingOnConditions(bool debug = false)
        {
            for (int i = 0; i < Props.associations.Count; i++)
            //foreach(HediffAssociation element in Props.associations)
            {
                HediffAssociation CurHA = Props.associations[i];
                AssociatedHediffHistory CurAHH = Registry[i];

                // association has already applied needed hediffs in the past, skipping
                if (CurAHH.done) continue;

                // triggered by harmony patch; no need to check it
                if (CurHA.specifics.triggered) continue;

                // pawn does not fulfil conditions : remove already applied hediffs and/or go for next hediff
                if (!RemoveHediffAndDeregister(CurHA, CurAHH, debug))
                    continue;

                List<BodyPartRecord> bodyPartRecords = null;
                // Hediff association has body parts specifications 
                if (CurHA.HasBodyPartToApplyHediff) {
                    bodyPartRecords = Pawn.GetBP(CurHA.bodyPart, debug);

                    //but pawn does not have any legit body part
                    if (bodyPartRecords.NullOrEmpty()) continue;
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
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (myMap == null)
            {
                Init();
                return;
            }

            if (shouldSkip)
                return;

            if (!Pawn.Spawned)
            {
                if (MyDebug) Log.Warning("pawn unspawned");
                return;
            }

            if (Props.PeriodicCheck && Pawn.IsHashIntervalTick(Props.checkFrequency))
                UpdateHediffDependingOnConditions(MyDebug);
        }

        public void Init()
        {
            if (MyDebug) Log.Warning("Entering Init");

            myMap = Pawn.Map;

            // pawn is not spawned ? caravaning ? from another faction ?
            if (!Pawn.Spawned)
            {
                if (MyDebug) Log.Warning("Null map");
                //parent.Severity = 0;
                shouldSkip = true;
                return;
            }

            if(Registry.NullOrEmpty())
            foreach(HediffAssociation ha in Props.associations)
            {
                if (MyDebug)
                    Log.Warning("Added 1 HediffAssociation");

                Registry.Add(new AssociatedHediffHistory());
            }

            TriggeredOnlyHediffs = Props.associations.All(a => a.specifics.triggered == true);
            if (MyDebug)
                Log.Warning("TriggeredOnlyHediffs:" + TriggeredOnlyHediffs);

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
                        Pawn.PawnResumeString() 
                        //+ "; hasAssociationHediffMaster: " + myPawn.Has_OHPLS()
                       ;
                }

                return result;
            }
        }

    }
}
