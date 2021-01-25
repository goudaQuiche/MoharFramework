using Verse;
using Verse.AI;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharAiJob
{
    public class GraveDiggerDef : Def
    {
        public List<PawnKindDef> workerPawnKind;
        public List<GraveDig_JobParameters> jobParameters;
        public JobDef jobDef;
        public bool debug = false;

        public override string ToString() => defName;
        public GraveDiggerDef Named(string name) => DefDatabase<GraveDiggerDef>.GetNamed(name);
        public override int GetHashCode() => defName.GetHashCode();

        public bool IsEmpty => jobParameters.NullOrEmpty();
    }
    
    public class GraveDig_JobParameters
    {
        public WorkerRequirement workerRequirement;
        public GraveDigWorkFlow workFlow;
        public GraveSpecification target;

        private bool HasTarget => target != null;
        public bool HasTargetSpec => HasTarget && target.HasEligibleGraves;
        public bool HasWorkFlow => workFlow != null;

        public bool HasWorkerRequirement => workerRequirement != null;
    }

    public class GraveSpecification
    {
        public List<ThingDef> eligibleGraves;
        public float maxDistance = 10;

        public ReservationProcess reservation;

        public bool HasEligibleGraves => !eligibleGraves.NullOrEmpty();
        public bool HasReservation => reservation != null;
    }

    public class GraveDigWorkFlow
    {
        public int workAmount = 300;
        public bool workAmountDoorOpenSpeedWeighted = true;

        public int dustPeriod = 50;

        public SoundDef sustainSound = null;
        public bool HasCustomSustainSound => sustainSound != null;

        public List<ThinkNode_JobGiver> tryToChainJobGiver;

        public bool HasRelevantWorkAmount => workAmount > 0;
        public bool HasRelevantDustPeriod => dustPeriod> 0;

        public bool HasJobGiverToChain => !tryToChainJobGiver.NullOrEmpty();
    }
}
