using Verse;
using System;
using UnityEngine;

namespace MoharGfx
{
    [StaticConstructorOnStartup]
    public class Graphic_AnimatedMote : Graphic_Collection
    {
        protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        protected virtual bool ForcePropertyBlock => false;

        public int TicksPerFrame = 7;
        public int FrameOffset = 0;
        public IndexEngine.TickEngine Engine = IndexEngine.TickEngine.synced;

        public bool Flipped = false;

        //private FloatRange FlickeringAlphaRange = new FloatRange(1, 1);

        public Vector2 PulsingScaleRange = new Vector2(0, 0);
        public float PulsingScaleSpeed = .5f;

        public bool MyDebug = false;

        //public bool HasFlickeringAlpha => FlickeringAlphaRange.Span != 0;
        public bool HasPulsingScale => PulsingScaleRange != Vector2.zero;

        //public int ThingHash(Thing t) => Mathf.Abs(t.thingIDNumber ^ 0x80FD52);

        //public override Material MatSingle => subGraphics[0].MatSingle;

        public int TickEngine(Mote mote)
        {
            if (Engine == IndexEngine.TickEngine.moteLifespan)
            {
                //Log.Warning(Find.TickManager.TicksGame + " - " + mote.spawnTick + " = " + (Find.TickManager.TicksGame - mote.spawnTick));
                return Find.TickManager.TicksGame - mote.spawnTick;
            }
                

            if (Engine == IndexEngine.TickEngine.relativeMoteLifespan)
                return (int)( (Find.TickManager.TicksGame - mote.spawnTick) * (mote.AgeSecs / mote.def.mote.Lifespan));

            //IndexEngine.TickEngine.synced
            return Find.TickManager.TicksGame;
        }

        // Historical
        public int GetIndex
        {
            get
            {
                int stairCaseInput = (int)Math.Floor((double)(Find.TickManager.TicksGame / TicksPerFrame));
                int curFrame = stairCaseInput % subGraphics.Length;
                return curFrame;
            }
        }

        public int GetAnotherIndex(Mote mote)
        {
            int stairCaseInput = (int)Math.Floor((double)(TickEngine(mote) / TicksPerFrame));
            int curFrame = stairCaseInput % subGraphics.Length;
            return curFrame;
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            DrawMoteInternal(loc, rot, thingDef, thing, 0);
        }

        public void ResolveScale(Mote mote, Thing thing, out Vector3 finalePos, out Vector3 exactScale)
        {
            exactScale = mote.linearScale;
            //Log.Warning(thing.def.defName + " scaling " + HasPulsingScale + " : " + PulsingScaleRange);

            exactScale.x *= data.drawSize.x;
            exactScale.z *= data.drawSize.y;

            if (HasPulsingScale)
            {
                float xVal = thing.VanillaPulse(PulsingScaleSpeed, PulsingScaleRange.x);
                float yVal = thing.VanillaPulse(PulsingScaleSpeed, PulsingScaleRange.y);
                //Log.Warning(thing.def.defName + " exSca: " + exactScale + "; val: " + xVal + ";" + yVal);
                exactScale.x += xVal;
                exactScale.z += yVal;
            }

            
            finalePos = new Vector3()
            {
                x = mote.DrawPos.x + mote.def.graphicData.drawOffset.x,
                y = mote.DrawPos.y + mote.def.graphicData.drawOffset.y,
                z = mote.DrawPos.z + mote.def.graphicData.drawOffset.z
            };
        }

        public void ResolveAlpha(Mote mote, out Color color)
        {
            float alpha = mote.Alpha;

            color = Color * mote.instanceColor;

            if (alpha <= 0f)
                return;

            color.a *= alpha;

            //Log.Warning("Graphic_AnimatedMote - color " + color);
            /*
            if (HasFlickeringAlpha)
            {
                //Log.Warning(thing.def.defName + ": HasFlickeringAlpha");
                color.a *= FlickeringAlphaRange.RandomInRange;
            }
            */
        }

        public void ResolveAnimationFram(Mote mote, out Material myMaterial)
        {
            int index = (GetAnotherIndex(mote) + FrameOffset) % subGraphics.Length;

            //if (MyDebug)Log.Warning(" in Graphic_animatedMote:" +"; FrameOffset:" + FrameOffset + "; index:" + index + "; GetIndex:" + GetIndex);
            //Log.Warning("Graphic_CutoutAnimatedMote - FrameOffset:" + FrameOffset + "; TicksPerFrame:" + TicksPerFrame + "; index:" + index);

            Graphic graphic = subGraphics[index];
            myMaterial = graphic.MatSingle;
        }

        public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
        {
            Mote mote = (Mote)thing;

            ResolveAlpha(mote, out Color color);
            ResolveScale(mote, thing, out Vector3 finalePos, out Vector3 exactScale);

            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(finalePos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);

            ResolveAnimationFram(mote, out Material myMaterial);

            Mesh myMesh = Flipped ? MeshPool.plane10Flip : MeshPool.plane10;

            if (!ForcePropertyBlock && color.IndistinguishableFrom(myMaterial.color))
            {
                Graphics.DrawMesh(myMesh, matrix, myMaterial, layer, null, 0);
                return;
            }
            //Log.Warning("Graphic_AnimatedMote - propertyBlock " + color);
            propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Graphics.DrawMesh(myMesh, matrix, myMaterial, layer, null, 0, propertyBlock);
        }
    }
}
