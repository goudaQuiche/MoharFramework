using UnityEngine;
using Verse;
using System;

namespace MoharGfx
{
    public class AlphaScaleLayer_Mote : MoteThrown
    {
        /*
        WIP Weighted range/scale
        */

        public AlphaScaleLayer_MoteDef Def => def as AlphaScaleLayer_MoteDef;
        public bool MyDebug => Def.debug;
        public string MainDebugStr => MyDebug ? Def.defName + " AlphaScaleLayer_Mote - " : string.Empty;

        public bool HasASL => Def.HasASL;

        public bool HasAlpha => Def.HasAlpha;
        public bool HasScale => Def.HasScale;
        public bool HasLayer => Def.HasLayer;

        public bool HasWeightedAlpha => Def.HasWeigthedAlpha;
        public bool HasWeigthedScale => Def.HasWeigthedScale;

        public int TickAge => Find.TickManager.TicksGame - spawnTick;
        public float LifeSpentRatio;

        public Vector3 InitialScale;
        public AltitudeLayer? NullableAltitudeLayer = null;

        public float? WeightedScale = null;
        public float? WeightedAlpha = null;

        public void SetWeightedScale (float ratio)
        {
            if (!HasWeigthedScale)
                WeightedScale = null;

            WeightedScale = Def.alphaScaleLayer.weightedScaleRange.LerpThroughRange(ratio);
        }

        public void SetWeightedAlpha(float ratio)
        {
            if (!HasWeightedAlpha)
                WeightedAlpha = null;

            WeightedAlpha = Def.alphaScaleLayer.weightedAlphaRange.LerpThroughRange(ratio);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            
            InitialScale = new Vector3(ExactScale.x, 0 , ExactScale.z);
        }

        public override float Alpha {
            get
            {
                //if (Def.debug && Def.HasAlpha) Log.Warning($"{MainDebugStr} Alpha");
                float result = base.Alpha;

                if (HasAlpha)
                    result *= Def.alphaScaleLayer.alpha.Evaluate(LifeSpentRatio);

                if (HasWeightedAlpha && WeightedAlpha != null)
                    result *= (float) WeightedAlpha;

                return result;
            }
        }

        protected override void TimeInterval(float deltaTime)
        {
            //if (Def.debug) Log.Warning($"{MainDebugStr} TimeInterval");

            base.TimeInterval(deltaTime);

            if (!HasScale)
                return;

            linearScale = new Vector3(InitialScale.x, 0,InitialScale.z);
            
            if (HasScale)
            {
                float LerpScaleRatio = Def.alphaScaleLayer.scale.Evaluate(LifeSpentRatio);
                linearScale.x *= LerpScaleRatio;
                linearScale.z *= LerpScaleRatio;
            }

            if(HasWeigthedScale && WeightedScale != null)
            {
                linearScale.x *= (float)WeightedScale;
                linearScale.z *= (float)WeightedScale;
            }

            linearScale.x = Mathf.Max(ExactScale.x, 0.0001f);
            linearScale.z = Mathf.Max(ExactScale.z, 0.0001f);

            //if (Def.debug) Log.Warning($"{MainDebugStr} TimeInterval LSR:{LifeSpentRatio} ratio:{ratio} Scale:{exactScale}");
            
        }

        public AltitudeLayer GetLayer
        {
            get
            {
                if (NullableAltitudeLayer != null)
                    return (AltitudeLayer)NullableAltitudeLayer;

                if (HasLayer)
                {
                    for (int i = 0; i < Def.alphaScaleLayer.layerSets.Count; i++)
                    {
                        LayerSet cls = Def.alphaScaleLayer.layerSets[i];
                        if (LifeSpentRatio <= cls.lifeRange)
                            return cls.layer;
                    }
                }

                return Def.altitudeLayer;
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (HasLayer || NullableAltitudeLayer != null)
            {
                DrawMote(GetLayer.AltitudeFor());
                return;
            }

            DrawMote(def.altitudeLayer.AltitudeFor());
        }

        public float GetLifeSpentRatio
        {
            get
            {
                int TickLifeSpan = (int)(Def.mote.Lifespan * 60);
                return (float)TickAge / TickLifeSpan;
            }
        }

        public override void Tick()
        {
            //if (Def.debug) Log.Warning($"{MainDebugStr} Tick");

            if (HasASL)
                LifeSpentRatio = GetLifeSpentRatio;

            base.Tick();
        }
    }
}
