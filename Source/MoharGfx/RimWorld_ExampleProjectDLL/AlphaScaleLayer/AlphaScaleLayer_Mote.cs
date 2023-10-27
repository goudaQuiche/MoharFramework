using UnityEngine;
using Verse;
using System;

namespace MoharGfx
{
    public class AlphaScaleLayer_Mote : MoteThrown
    {
        /*
        private Vector3 linkDrawPos = new Vector3(-1000f, -1000f, -1000f);
        public bool isLinked = false;
        */

        public AlphaScaleLayer_MoteDef Def => def as AlphaScaleLayer_MoteDef;
        public bool MyDebug => Def.debug;
        public string MainDebugStr => MyDebug ? Def.defName + " AlphaScaleLayer_Mote - " : string.Empty;

        public bool HasASL => Def.HasASL;
        public bool HasAlpha => Def.HasAlpha;
        public bool HasScale => Def.HasScale;
        public bool HasLayer => Def.HasLayer;

        public float LifeSpentRatio;
        public Vector3 InitialScale;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            InitialScale = new Vector3( exactScale.x, 0 , exactScale.z);
        }

        public override float Alpha {
            get
            {
                //if (Def.debug && Def.HasAlpha) Log.Warning($"{MainDebugStr} Alpha");

                if (HasAlpha)
                    return Def.alphaScaleLayer.alpha.Evaluate(LifeSpentRatio);

                return base.Alpha;
            }
        }

        protected override void TimeInterval(float deltaTime)
        {
            //if (Def.debug) Log.Warning($"{MainDebugStr} TimeInterval");

            base.TimeInterval(deltaTime);

            if (!HasScale)
                return;

            //float ratio = Def.alphaScaleLayer.scale.Evaluate(deltaTime);
            float ratio = Def.alphaScaleLayer.scale.Evaluate(LifeSpentRatio);
            exactScale = new Vector3(ratio * InitialScale.x, 0, ratio * InitialScale.z);

            exactScale.x = Mathf.Max(exactScale.x, 0.0001f);
            exactScale.z = Mathf.Max(exactScale.z, 0.0001f);

            //if (Def.debug) Log.Warning($"{MainDebugStr} TimeInterval LSR:{LifeSpentRatio} ratio:{ratio} Scale:{exactScale}");
            
        }

        public AltitudeLayer GetLayer
        {
            get
            {
                if (!HasLayer)
                {
                    if (Def.debug) Log.Warning(MainDebugStr + "GetLayer - Found layer conf - should not happen");
                    return AltitudeLayer.Building;
                }

                for (int i = 0; i < Def.alphaScaleLayer.layerSets.Count; i++)
                {
                    LayerSet cls = Def.alphaScaleLayer.layerSets[i];
                    if (cls.lifeRange.Includes(LifeSpentRatio))
                        return cls.layer;
                }

                if (Def.debug) Log.Warning(MainDebugStr + "GetLayer - Found no layer - should not happen");
                return AltitudeLayer.Building;
            }
        }

        public override void Draw()
        {
            if (HasLayer)
            {
                Draw(GetLayer.AltitudeFor());
                return;
            }

            base.Draw();
        }

        public float GetLifeSpentRatio
        {
            get
            {
                int TickAge = Find.TickManager.TicksGame - spawnTick;
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
