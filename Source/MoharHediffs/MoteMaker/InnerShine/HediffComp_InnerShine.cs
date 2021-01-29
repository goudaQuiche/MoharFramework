using UnityEngine;
using Verse;
using System.Collections.Generic;
using System.Linq;

using MoharGfx;

namespace MoharHediffs
{
    //MoteMaker.MakeWaterSplash(vector, pawn.Map, Mathf.Sqrt(pawn.BodySize) * 2f, 1.5f);

    public class HediffComp_InnerShine : HediffComp
    {
        public Map MyMap => Pawn.Map;
        public bool NullMap => MyMap == null;

        public List<InnerShineRecord> Tracer;

        public bool MyDebug => Props.debug;
        public bool HasEmptyTracer => Tracer.NullOrEmpty();
        public bool HasShinePool => Props.HasShinePool;

        public HediffCompProperties_InnerShine Props => (HediffCompProperties_InnerShine)props;

        public override void CompPostMake()
        {
            if (!StaticCheck.IsOk)
                this.SelfDestroy();

            PropsCheck();
            this.CreateTracer();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligible())
            {
                if (MyDebug) Log.Warning("null pawn");
                return;
            }

            if (HasEmptyTracer)
            {
                this.CreateTracer();
            }

            foreach (InnerShineRecord ISR in Tracer)
            {
                InnerShineItem ISI = Props.innerShinePool.Where(i => i.label == ISR.label).FirstOrFallback();
                if (ISI == null)
                {
                    if (MyDebug) Log.Warning("Did not find ISI with label:" + ISR.label);
                    continue;
                }

                if (ISR.ticksLeft <= 0)
                {
                    if (ISI.ShouldSpawnMote(ISR, Pawn))
                        ISI.TryPlaceMote(ISR, Pawn);

                    ISI.ResetTicks(ISR);
                }
                else
                {
                    ISR.ticksLeft--;
                }

                ISI.UpdateMotes(ISR, Pawn, MyDebug);
            }


            //if (MyDebug) Log.Warning(Pawn.ThingID + " trying to spawn mote - bodytype:" + Pawn.story?.bodyType?.defName);

        }
        public void PropsCheck()
        {
            if (!HasShinePool)
            {
                if (MyDebug) Log.Warning("no shine pool, giving up");
                this.SelfDestroy();
                return;
            }

            if (!MyDebug)
                return;

            if (Props.innerShinePool.Where(s => s.HasMotePool) is IEnumerable<InnerShineItem> noMotePoolItems)
                foreach (InnerShineItem isi in noMotePoolItems)
                    Log.Warning(isi.label + " has no mote pool");

            if (Props.innerShinePool.Where(s => !s.HasDefaultDrawRules && !s.HasBodyTypeDrawRules) is IEnumerable<InnerShineItem> noDrawRulesItems)
                foreach (InnerShineItem isi in noDrawRulesItems)
                    Log.Warning(isi.label + " has no default nor bodytypedef draw rules, at least one is required");

            foreach (InnerShineItem ISI in Props.innerShinePool)
                Log.Warning(ISI.Dump());

        }
    }
}
