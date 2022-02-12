using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;

namespace YAHA
{
    public class HediffComp_YetAnotherHediffApplier : HediffComp
    {
        Map myMap = null;
        public HediffCompProperties_YetAnotherHediffApplier Props => (HediffCompProperties_YetAnotherHediffApplier)props;

        public List<AssociatedHediffHistory> Registry = new List<AssociatedHediffHistory>();
        //public JobDef CurrentJob;

        bool shouldSkip = false;
        bool MyDebug => Props.debug;
         
        public override void CompPostMake()
        {
            Init();
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

                // pawn does not fulfil conditions : go for next hediff
                if (!Pawn.CheckAllConditions(CurHA))
                {
                    if (!CurHA.specifics.removeIfFalse)
                        continue;

                    //foreach(Hediff h in CurAHH.appliedHediffs)
                    for (int j = CurAHH.appliedHediffs.Count - 1; j >= 0; j--)
                    {
                        Hediff h = CurAHH.appliedHediffs[j];
                        // enough to remove the applied hediff ?? or need to retrieve it from pawn with bpr
                        h.Severity = 0; h.PostRemoved();
                        CurAHH.appliedHediffs.RemoveAt(j);
                    }
                }

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
                        foreach (HediffDef hd in CurHA.hediffPool)
                        {
                            //Applying hediff to full body
                            Hediff h = HediffMaker.MakeHediff(hd, Pawn, null);
                            if (CurHA.specifics.HasSeverity)
                                h.Severity = CurHA.specifics.severity.RandomInRange;
                            Pawn.health.AddHediff(h, null, null);

                            //Recording hediff applied in registry
                            CurAHH.appliedHediffs.Add(h);

                            if (CurHA.specifics.HasLimit)
                                CurAHH.appliedNum++;
                        }
                    }
                }
                // target has multiple bodypart records as target
                else
                {
                    foreach (BodyPartRecord bpr in bodyPartRecords)
                    {
                        if (CurHA.HasHediffPool)
                        {
                            foreach (HediffDef hd in CurHA.hediffPool)
                            {
                                //Applying hediff to full body
                                Hediff h = HediffMaker.MakeHediff(hd, Pawn, bpr);
                                if (CurHA.specifics.HasSeverity)
                                    h.Severity = CurHA.specifics.severity.RandomInRange;

                                Pawn.health.AddHediff(h, bpr, null);

                                //Recording hediff applied in registry
                                CurAHH.appliedHediffs.Add(h);

                                if (CurHA.specifics.HasLimit)
                                    CurAHH.appliedNum++;
                            }
                        }
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
                UpdateHediffDependingOnConditions();
        }

        public void Init()
        {
            if (MyDebug) Log.Warning("Entering Init");

            myMap = Pawn.Map;
            if (Pawn == null || myMap == null)
            {
                if (MyDebug) Log.Warning("Null pawn or map");
                parent.Severity = 0;
                shouldSkip = true;
                return;
            }

            foreach(HediffAssociation ha in Props.associations)
            {
                Registry.Add(new AssociatedHediffHistory());
            }
            
            UpdateHediffDependingOnConditions();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

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
