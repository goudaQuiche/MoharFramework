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

        private const int WaitingAmount = 300;
        private const int LowerCorpseHealthPeriod = 120;

        public Thing Emitter = null;

        public bool EmitterIsPawn => Emitter == pawn;

        public CorpseProduct corpseProduct = null;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            CheckAndFillCorpseProduct();
            pawn.CurJob.count = 1;
            return true;
        }

        private void CheckAndFillCorpseProduct()
        {
            if (corpseProduct == null)
            {
                Log.Warning("AiCorpse_Consume_JobDriver cant work without corpseProduct ");
                CorpseJobDef DefToUse = pawn.RetrieveCJD(out MyDebug, PreRetrieveDebug);
                CorpseRecipeSettings CRS = pawn.RetrieveCRS(DefToUse, MyDebug);
                corpseProduct = CRS.product;
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CheckAndFillCorpseProduct();

            this.FailOnDestroyedOrNull(CorpseInd);
            Toil gotoCorpse = Toils_Goto.GotoThing(CorpseInd, PathEndMode.Touch).FailOnDespawnedOrNull(CorpseInd);
            yield return gotoCorpse;
            this.FailOnDestroyedOrNull(CorpseInd);

            Toil toil =
                Toils_General.Wait(WaitingAmount)
                .FailOnDespawnedOrNull(CorpseInd).FailOnCannotTouch(CorpseInd, PathEndMode.Touch)
                .WithEffect(ConsumeCorpseDefaultDefOf.ButcherFlesh, CorpseInd)
                .PlaySustainerOrSound(ConsumeCorpseDefaultDefOf.Recipe_Surgery);

            toil.tickAction = delegate
            {
                if (pawn.IsHashIntervalTick(LowerCorpseHealthPeriod))
                {
                    Corpse.HitPoints = (int)(Corpse.HitPoints * .75f);
                }
            };
            yield return toil;

            yield return corpseProduct.SpawnProductDespawnCorpse(Corpse.Position, Corpse, pawn.Map);
        }

        [DefOf]
        public class ConsumeCorpseDefaultDefOf
        {
            public static EffecterDef ButcherFlesh;
            public static SoundDef Recipe_Surgery;
        }
    }

}
