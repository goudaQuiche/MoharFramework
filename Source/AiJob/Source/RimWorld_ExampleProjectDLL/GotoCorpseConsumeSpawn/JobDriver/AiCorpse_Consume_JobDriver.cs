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
        public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;

        private const TargetIndex CorpseInd = TargetIndex.A;
        private const TargetIndex CellInd = TargetIndex.C;

        //private static List<IntVec3> tmpCells = new List<IntVec3>();

        private Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;

        private Thing Target => Corpse;

        private const int DefaultWorkAmount = 300;
        private const int DefaultNibblingPeriod = 120;
        private const float DefaultNibblingAmount = 1;
        private bool NibblingRequired => NibblingAmount != DefaultNibblingAmount;

        public CorpseProduct corpseProduct = null;
        public bool HasCorpseProduct => corpseProduct != null;
        public WorkFlow workFlow = null;
        public bool HasWorkFlow => workFlow != null;

        public int WorkAmount
        {
            get
            {
                if (HasWorkFlow && workFlow.HasWorkAmountPerHS)
                    return (int)(workFlow.workAmountPerHealthScale * Corpse.InnerPawn.RaceProps.baseHealthScale);

                return DefaultWorkAmount;
            }
        }

        public int NibblingPeriod {
            get
            {
                if (HasWorkFlow && workFlow.HasNibblingPeriodPerHS)
                    return (int)(workFlow.nibblingPeriodPerHealthScale * Corpse.InnerPawn.RaceProps.baseHealthScale);
                return DefaultNibblingPeriod;
            }
        }
        public float NibblingAmount => HasWorkFlow && workFlow.HasNibblingAmount ? workFlow.nibblingAmount : DefaultNibblingAmount;
        public SoundDef MySustainSound => HasWorkFlow && workFlow.HasCustomSustainSound ? workFlow.sustainSound : ConsumeCorpseDefaultDefOf.Recipe_Surgery;
        public EffecterDef MyEffecterDef => HasWorkFlow && workFlow.HasCustomEffecterDef ? workFlow.effecterDef : ConsumeCorpseDefaultDefOf.ButcherFlesh;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            CheckAndFillCorpseProduct();
            pawn.CurJob.count = 1;
            return true;
        }

        private bool CheckAndFillCorpseProduct()
        {
            if (!HasCorpseProduct || !HasWorkFlow)
            {
                if (PreRetrieveDebug || MyDebug) Log.Warning("AiCorpse_Consume_JobDriver cant work without corpseProduct, Loading it");

                if (pawn.RetrieveCorpseJobDef(out MyDebug, PreRetrieveDebug) is CorpseJobDef DefToUse)
                {
                    //CS.categoryDef.Any(tc => t.def.IsWithinCategory(tc))
                    IEnumerable<CorpseRecipeSettings> CRSList = 
                        pawn.RetrieveCorpseRecipeSettings(DefToUse, MyDebug)
                        .Where(c => c.HasTargetCategory && c.target.HasCorpseCategoryDef)
                        .Where(c => FindCorpse.ValidateCorpse(Corpse, pawn.Map, pawn.Faction, c.target)) ;

                    if (CRSList.EnumerableNullOrEmpty())
                        return false;

                    CorpseRecipeSettings CRS = CRSList.FirstOrFallback();
                    if(CRS == null)
                        return false;

                    corpseProduct = CRS.product;
                    workFlow = CRS.workFlow;
                    return true;
                }
                return false;
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (!CheckAndFillCorpseProduct())
            {
                if (PreRetrieveDebug) Log.Warning("AiCorpse_Consume_JobDriver - Failed to initialize settings");
                yield break;
            }

            this.FailOnDestroyedOrNull(CorpseInd);
            Toil gotoCorpse = Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch).FailOnDespawnedOrNull(CorpseInd);
            yield return gotoCorpse;
            this.FailOnDestroyedOrNull(CorpseInd);

            Toil toil =
                Toils_General.Wait(WorkAmount)
                .FailOnDespawnedOrNull(CorpseInd).FailOnCannotTouch(CorpseInd, PathEndMode.Touch)
                .WithEffect(MyEffecterDef, CorpseInd)
                .PlaySustainerOrSound(MySustainSound);

            toil.tickAction = delegate
            {
                if ( NibblingRequired && pawn.IsHashIntervalTick(NibblingPeriod))
                {
                    Corpse.HitPoints = (int)(Corpse.HitPoints * NibblingAmount);
                }
            };
            yield return toil;

            yield return workFlow.SpawnFilth(Corpse, pawn.Map, MyDebug);
            yield return corpseProduct.SpawnProductDespawnCorpse(pawn, Corpse.Position, Corpse, pawn.Map, MyDebug);
        }

        [DefOf]
        public class ConsumeCorpseDefaultDefOf
        {
            public static EffecterDef ButcherFlesh;
            public static SoundDef Recipe_Surgery;
        }
    }

}
