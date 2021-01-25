using Verse;
using Verse.AI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace MoharAiJob
{
    public class AiCorpse_Consume_JobDriver : JobDriver
    {
        bool MyDebug = false;
        readonly bool MyCsDebug = false;
        //public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;
        readonly string DebugStr = "MoharAiJob.AiCorpse_Consume_JobDriver ";

        private TargetIndex CorpseInd => TargetIndex.A;
        //private TargetIndex CellInd => TargetIndex.C;
        //private static List<IntVec3> tmpCells = new List<IntVec3>();

        private Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;

        private Thing Target => Corpse;

        private const int DefaultWorkAmount = 300;
        private const int DefaultNibblingPeriod = 120;
        private const float DefaultNibblingAmount = 1;
        private bool NibblingRequired => NibblingAmount != DefaultNibblingAmount;

        public CorpseRecipeSettings RetrievedCRS = null;
        public bool HasRCRCS => RetrievedCRS != null;

        public CorpseProduct RetrievedCorpseProduct => (HasRCRCS && RetrievedCRS.HasProductSpec) ? RetrievedCRS.product : null;
        public bool HasCorpseProduct => RetrievedCorpseProduct != null;

        public WorkFlow RetrievedWorkFlow => (HasRCRCS && RetrievedCRS.HasWorkFlow) ? RetrievedCRS.workFlow : null;
        public bool HasWorkFlow => RetrievedWorkFlow != null;

        public int WorkAmount
        {
            get
            {
                if (HasWorkFlow && RetrievedWorkFlow.HasWorkAmountPerHS)
                    return (int)(RetrievedWorkFlow.workAmountPerHealthScale * Corpse.InnerPawn.RaceProps.baseHealthScale);

                return DefaultWorkAmount;
            }
        }

        public int NibblingPeriod {
            get
            {
                if (HasWorkFlow && RetrievedWorkFlow.HasNibblingPeriodPerHS)
                    return (int)(RetrievedWorkFlow.nibblingPeriodPerHealthScale * Corpse.InnerPawn.RaceProps.baseHealthScale);
                return DefaultNibblingPeriod;
            }
        }
        public float NibblingAmount => HasWorkFlow && RetrievedWorkFlow.HasNibblingAmount ? RetrievedWorkFlow.nibblingAmount : DefaultNibblingAmount;
        public SoundDef MySustainSound => HasWorkFlow && RetrievedWorkFlow.HasCustomSustainSound ? RetrievedWorkFlow.sustainSound : ConsumeCorpseDefaultDefOf.Recipe_Surgery;
        public EffecterDef MyEffecterDef => HasWorkFlow && RetrievedWorkFlow.HasCustomEffecterDef ? RetrievedWorkFlow.effecterDef : ConsumeCorpseDefaultDefOf.ButcherFlesh;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (CheckAndFillCorpseProduct())
            {
                if(RetrievedCRS.HasTargetSpec && RetrievedCRS.target.HasReservationProcess && RetrievedCRS.target.reservation.reserves)
                {
                    bool TryingToReserve = pawn.Reserve(TargetA, job, 1, -1, null);
                    if(MyDebug) Log.Warning(pawn + " reserved " + Corpse.ThingID + ":" + TryingToReserve);
                }
            }

            pawn.CurJob.count = 1;
            return true;
        }

        private bool CheckAndFillCorpseProduct()
        {
            if (HasCorpseProduct && HasWorkFlow)
                return true;

            if (MyCsDebug)
            {
                Log.Warning(
                    DebugStr + " cant work without corpseProduct, Loading it " +
                    "for pawn:" + pawn?.ThingID + " corpse:" + Corpse?.ThingID
                );
            }

            if (pawn.RetrieveCorpseJobDef(out MyDebug, MyCsDebug) is CorpseJobDef DefToUse)
            {
                string meFunc = MyCsDebug ? "CheckAndFillCorpseProduct" : string.Empty;

                IEnumerable<CorpseRecipeSettings> CRSList =
                    pawn.RetrieveCorpseRecipeSettings(DefToUse, MyDebug)
                    .Where(c => c.target.ValidateCorpse(Corpse, pawn, MyDebug, meFunc));

                /*
            IEnumerable<CorpseRecipeSettings> CRSPrime = pawn.RetrieveCorpseRecipeSettings(DefToUse, MyDebug);

            if (MyDebug && !CRSPrime.EnumerableNullOrEmpty())
                Log.Warning(meFunc + " found " + CRSPrime.Count() + " prime CRS");

            IEnumerable<CorpseRecipeSettings> CRSList = CRSPrime.Where(c => FindCorpse.ValidateCorpse(Corpse, pawn.Map, pawn.Faction, c.target, MyDebug, meFunc));
                  */
                if (CRSList.EnumerableNullOrEmpty())
                {
                    Log.Warning(DebugStr + "did not find CorpseRecipeSettings relative to Corpse " + Corpse.ThingID);
                    return false;
                }
                else
                {
                    if (MyDebug)
                        Log.Warning(meFunc + " found " + CRSList.Count() + " CRS");
                }

                RetrievedCRS = CRSList.FirstOrFallback();
                if (RetrievedCRS == null)
                {
                    Log.Warning(DebugStr + "did not find CorpseRecipeSettings relative to Corpse (2)");
                    return false;
                }

                if (MyDebug)
                    Log.Warning(DebugStr + " did it - OK");

                return true;
            }
            return false;

        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (!CheckAndFillCorpseProduct())
            {
                if (MyCsDebug) Log.Warning(DebugStr + " - Failed to initialize settings");
                yield break;
            }

            this.FailOnDestroyedOrNull(CorpseInd);

            Toil gotoCorpse = Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch).FailOnDespawnedOrNull(CorpseInd);
            yield return gotoCorpse;

            Toil toil =
                Toils_General.Wait(WorkAmount)
                .FailOnCannotTouch(CorpseInd, PathEndMode.Touch)
                .FailOnBurningImmobile(CorpseInd)
                .FailOnSomeonePhysicallyInteracting(CorpseInd)
                .WithEffect(MyEffecterDef, CorpseInd)
                .PlaySustainerOrSound(MySustainSound);

            toil.tickAction = delegate
            {
                if ( NibblingRequired && pawn.IsHashIntervalTick(NibblingPeriod))
                {
                    Corpse.HitPoints = (int)(Corpse.HitPoints * NibblingAmount);
                    RetrievedWorkFlow.SpawnFilth(Corpse, pawn.Map, MyDebug);
                }
            };
            yield return toil;

            yield return RetrievedCRS.SpawnProductDespawnCorpse(pawn, Corpse, MyDebug);
        }

        [DefOf]
        public class ConsumeCorpseDefaultDefOf
        {
            public static EffecterDef ButcherFlesh;
            public static SoundDef Recipe_Surgery;
        }
    }

}
