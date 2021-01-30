using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;


namespace MoharHediffs
{
    public class InnerShineRecord
    {
        public string label;
        public List<Thing> spawned;
        public int ticksLeft;
        public Color lastColor;

        public InnerShineRecord(InnerShineItem ISI) {
            label = ISI.label;
            spawned = new List<Thing>();
            ticksLeft = ISI.NewPeriod();
            lastColor = Color.black;
        }

        public string Dump => $"label:{label} spawned:{spawned?.CountAllowNull()} ticksLeft:{ticksLeft} lastColor:{lastColor}";
    }

    public static class TracerUtils
    {
        public static void CreateTracer(this HediffComp_InnerShine comp)
        {
            comp.Tracer = new List<InnerShineRecord>();

            if (comp.Props.HasRawShinePool)
                foreach (InnerShineItem ISI in comp.Props.innerShinePool)
                    comp.Tracer.Add(new InnerShineRecord(ISI));

            if (comp.Props.HasShineDefPool)
                foreach (InnerShineDef ISD in comp.Props.innerShineDefPool)
                    comp.Tracer.Add(new InnerShineRecord(ISD.item));

            if (comp.MyDebug)
            {
                int i = 0;
                foreach (InnerShineRecord ISR in comp.Tracer)
                {
                    Log.Warning(i.ToString("00") + "=>" + ISR.Dump);
                    i++;
                }
            }
        }

        public static int NewPeriod(this InnerShineItem ISI) => ISI.spawningRules.period.RandomInRange;
        public static void ResetTicks(this InnerShineItem ISI, InnerShineRecord ISR) => ISR.ticksLeft = ISI.NewPeriod();
        public static bool HasMoteNumLimit(this InnerShineItem ISI) => !ISI.spawningRules.IsUnlimited;

        public static void TryPlaceMote(this InnerShineItem ISI, InnerShineRecord ISR, Pawn pawn)
        {
            //if (!HasShinePool)return;
            //Thing result = null;

            // wont exit because we want to record lastFootPos + use old value before recording it
            if (pawn.Position.InBounds(pawn.Map))
           {
                float rot = 0;
                ISI.InitSpecs(ISR, pawn, out Vector3 drawPosWithOffset, out float scale);

                if (drawPosWithOffset.ToIntVec3().InBounds(pawn.Map)){
                    ThingDef moteDef = ISI.motePool.RandomElementWithFallback();

                    if (drawPosWithOffset.TryAnyMoteSpawn(pawn.Map, rot, scale, moteDef, ISI.debug) is Mote mote)
                    {
                        ISI.ChangeMoteColor(ISR, mote);
                        ISR.spawned.Add(mote);
                        ISI.NewPeriod();
                    }
                }
            }
        }
        public static void UpdateMotes(this InnerShineItem ISI, InnerShineRecord ISR, Pawn pawn, bool debug = false)
        {
            if (ISR.spawned.NullOrEmpty())
                return;
            for (int i = ISR.spawned.Count - 1; i >= 0; i--)
            {
                Thing curT = ISR.spawned[i];

                if (curT.DestroyedOrNull())
                {
                    ISR.spawned.RemoveAt(i);
                    continue;
                }
                if (!ISI.HasCompatibleActivity(pawn))
                {
                    curT.Destroy();
                    ISR.spawned.RemoveAt(i);
                    continue;
                }
                if (curT is Mote mote)
                {
                    mote.exactPosition = pawn.DrawPos + pawn.GetLinkOffset(ISI.linkType) + ISI.GetDrawOffset(pawn);
                    //if (debug) Log.Warning(ISR.label + $" => mote.exactPosition: {mote.exactPosition} mote.DrawPos:{mote.DrawPos}");
                }

            }
        }

        public static void InitSpecs(this InnerShineItem ISI, InnerShineRecord ISR, Pawn pawn, out Vector3 drawPosWithOffset, out float scale)
        {
            Vector3 drawPos = pawn.DrawPos;
            ISI.GetSpecifities(pawn, out Vector3 bodyTypeOffset, out scale);
            Vector3 linkOffset = pawn.GetLinkOffset(ISI.linkType);
            drawPosWithOffset = drawPos + linkOffset + bodyTypeOffset;

            if (ISI.debug) Log.Warning(
                    pawn.ThingID + " " + ISI.label + " TryPlaceMote - " +
                    "drawPos: " + drawPos + " linkOffset:" + linkOffset + " bodyTypeOffset:" + bodyTypeOffset +
                    "scale: " + scale
                );
        }
    }
}
