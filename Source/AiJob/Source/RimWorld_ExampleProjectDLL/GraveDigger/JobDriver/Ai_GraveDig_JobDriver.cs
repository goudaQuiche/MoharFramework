using Verse;
using Verse.AI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace MoharAiJob
{
    public class Ai_GraveDig_JobDriver : JobDriver
    {
        bool MyDebug = false;
        public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;

        private const TargetIndex GraveInd = TargetIndex.A;
        private const TargetIndex CorpseInd = TargetIndex.B;
        private const TargetIndex CellInd = TargetIndex.C;

        //private static List<IntVec3> tmpCells = new List<IntVec3>();

        private Building GraveBuilding => (Building)job.GetTarget(TargetIndex.A).Thing;
        private Corpse CorpseThing => (Corpse)job.GetTarget(TargetIndex.B).Thing;

        private Thing Target => GraveBuilding;

        private const int DefaultWorkAmount = 300;
        private const int DefaultDustPeriod = 50;

        public GraveDigWorkFlow workFlow = null;
        public bool HasWorkFlow => workFlow != null;

        public float SpeedFactor => (GraveBuilding.Stuff == null) ? 1 : GraveBuilding.Stuff.GetStatValueAbstract(StatDefOf.DoorOpenSpeed);

        public int WorkAmount
        {
            get
            {
                if (HasWorkFlow && workFlow.HasRelevantWorkAmount)
                    return workFlow.workAmountDoorOpenSpeedWeighted ? (int)(workFlow.workAmount * SpeedFactor) : workFlow.workAmount;

                return DefaultWorkAmount;
            }
        }

        public SoundDef MySustainSound => HasWorkFlow && workFlow.HasCustomSustainSound ? workFlow.sustainSound : GraveDigDefaultDefOf.Tunnel;
        public int MyDustPeriod => HasWorkFlow && workFlow.HasRelevantDustPeriod ? workFlow.dustPeriod : DefaultDustPeriod;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            CheckAndFillWorkFlow();
            pawn.CurJob.count = 1;
            return true;
        }

        private bool CheckAndFillWorkFlow()
        {
            if (!HasWorkFlow)
            {
                if (PreRetrieveDebug || MyDebug) Log.Warning("Ai_GraveDig_JobDriver cant work without workflow, Loading it");

                if (pawn.RetrieveGDD(out MyDebug, PreRetrieveDebug) is GraveDiggerDef DefToUse && pawn.RetrieveGDJP(DefToUse, MyDebug) is GraveDig_JobParameters GDJP)
                {
                    workFlow = GDJP.workFlow;
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (!CheckAndFillWorkFlow())
            {
                if (PreRetrieveDebug) Log.Warning("Ai_GraveDig_JobDriver - Failed to initialize settings");
                yield break;
            }

            this.FailOnDestroyedOrNull(GraveInd);
            Toil gotoGrave = Toils_Goto.GotoThing(GraveInd, PathEndMode.Touch).FailOnDespawnedOrNull(GraveInd);
            yield return gotoGrave;
            this.FailOnDestroyedOrNull(GraveInd);

            Toil toil =
                Toils_General.Wait(WorkAmount)
                .FailOnDespawnedOrNull(GraveInd).FailOnCannotTouch(GraveInd, PathEndMode.Touch)
                .PlaySustainerOrSound(MySustainSound);

            toil.tickAction = delegate
            {
                if ( pawn.IsHashIntervalTick(MyDustPeriod))
                {
                    if(MyDebug)Log.Warning("Ai_GraveDig_JobDriver - time to puff");
                    ThrowDigMote(pawn.Position.BetweenTouchingCells(Target.Position), pawn.Map);
                }
            };
            yield return toil;

            yield return Toils_General.Open(GraveInd);

            this.FailOnDestroyedOrNull(CorpseInd);
            Toil gotoCorpse = Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch).FailOnDespawnedOrNull(CorpseInd);

            yield return Toils_Reserve.Reserve(CorpseInd);
            yield return gotoCorpse;
            yield return Toils_Haul.StartCarryThing(CorpseInd);
            yield return FindCellToDropCorpseToil();
            yield return Toils_Reserve.Reserve(CellInd);
            yield return Toils_Goto.GotoCell(CellInd, PathEndMode.Touch);
            yield return Toils_Haul.PlaceHauledThingInCell(CellInd, null, storageMode: false);

            yield return Forbid();

        }

        private Toil FindCellToDropCorpseToil()
        {
            return new Toil
            {
                initAction = delegate
                {
                    IntVec3 result = IntVec3.Invalid;

                    result = CellFinder.RandomClosewalkCellNear(
                        pawn.Position, pawn.Map, 10,
                        (IntVec3 x) => pawn.CanReserve(x) &&
                        x.GetFirstItem(pawn.Map) == null
                    );

                    job.SetTarget(TargetIndex.C, result);
                },
                atomicWithPrevious = true
            };
        }

        private Toil Forbid()
        {
            return new Toil
            {
                initAction = delegate
                {
                    CorpseThing?.SetForbidden(value: true);
                },
                atomicWithPrevious = true
            };
        }

        public static void ThrowDigMote(Vector3 loc, Map map)
        {
            if (!map.AllowedMoteSpawn(loc))
                return;

            MoteMaker.ThrowDustPuffThick(loc, map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
        }

        [DefOf]
        public class GraveDigDefaultDefOf
        {
            public static SoundDef Tunnel;
        }
    }

}
