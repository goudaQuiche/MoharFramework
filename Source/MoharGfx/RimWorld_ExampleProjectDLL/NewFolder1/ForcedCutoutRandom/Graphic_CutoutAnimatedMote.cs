using Verse;
using System;
using UnityEngine;

namespace MoharGfx
{
    [StaticConstructorOnStartup]
    //public class Graphic_CutoutAnimatedMote : Graphic_ForceCutoutCollection
    public class Graphic_CutoutAnimatedMote : Graphic_Collection
    {
        protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        protected virtual bool ForcePropertyBlock => false;
        //protected virtual bool ForcePropertyBlock => true;

        // Animation
        public int TicksPerFrame = 7;
        public int FrameOffset = 0;
        public IndexEngine.TickEngine Engine = IndexEngine.TickEngine.synced;

        public bool Flipped = false;

        // Alpha channel
        private FloatRange FlickeringAlphaRange = new FloatRange(1, 1);

        //Scale
        public Vector2 PulsingScaleRange = new Vector2(0, 0);
        public float PulsingScaleSpeed = .5f;


        public bool MyDebug = false;
        public bool HasFlickeringAlpha => FlickeringAlphaRange.Span != 0;
        public bool HasPulsingScale => PulsingScaleRange != Vector2.zero;

        //public int ThingHash(Thing t) => Mathf.Abs(t.thingIDNumber ^ 0x80FD52);

        //public override Material MatSingle => subGraphics[0].MatSingle;

        public int TickEngine(Mote mote)
        {
            if (Engine == IndexEngine.TickEngine.moteLifespan)
            {
                //Log.Warning("TickEngine - " + Find.TickManager.TicksGame + " - " + mote.spawnTick + " = " + (Find.TickManager.TicksGame - mote.spawnTick));
                return Find.TickManager.TicksGame - mote.spawnTick;
            }


            if (Engine == IndexEngine.TickEngine.relativeMoteLifespan)
                return (int)((Find.TickManager.TicksGame - mote.spawnTick) * (mote.AgeSecs / mote.def.mote.Lifespan));

            //IndexEngine.TickEngine.synced
            return Find.TickManager.TicksGame;
        }

        // Historical
        /*
        public int GetIndex
        {
            get
            {
                int stairCaseInput = (int)Math.Floor((double)(Find.TickManager.TicksGame / TicksPerFrame));
                int curFrame = stairCaseInput % subGraphics.Length;
                return curFrame;
            }
        }
        */

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

        public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
        {
            Mote mote = (Mote)thing;
            float alpha = mote.Alpha;
            if (alpha <= 0f)
                return;

            //Log.Warning("DrawMoteInternal - " + thing.ThingID + " iC:" + mote.instanceColor);
            Color color = Color * mote.instanceColor;
            color.a *= alpha;
            if (HasFlickeringAlpha)
            {
                //Log.Warning(thing.def.defName + ": HasFlickeringAlpha");
                color.a *= FlickeringAlphaRange.RandomInRange;
            }

            Vector3 exactScale = mote.exactScale;
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

            Matrix4x4 matrix = default(Matrix4x4);
            Vector3 finalePos = new Vector3()
            {
                x = mote.DrawPos.x + mote.def.graphicData.drawOffset.x,
                y = mote.DrawPos.y + mote.def.graphicData.drawOffset.y,
                z = mote.DrawPos.z + mote.def.graphicData.drawOffset.z
            };

            matrix.SetTRS(finalePos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
            /*
            int FrameOffset = 0;
            if(thing is CustomTransformation_Mote CTM)
            {
                FrameOffset = CTM.FrameOffset;
                if (MyDebug) Log.Warning("Found CTM in Graphic_AnimatedMote FrameOffset:" + FrameOffset);
            }
            */
            //else if (MyDebug) Log.Warning("CTM Not Found in Graphic_AnimatedMote; Thing:" + thing.def);

            int index = (GetAnotherIndex(mote) + FrameOffset) % subGraphics.Length;

            //if (MyDebug)Log.Warning(" in Graphic_animatedMote:" +"; FrameOffset:" + FrameOffset + "; index:" + index + "; GetIndex:" + GetIndex);
            Log.Warning("Graphic_CutoutAnimatedMote - FrameOffset:" + FrameOffset + "; TicksPerFrame:" + TicksPerFrame + "; index:" + index);


            Graphic graphic = subGraphics[index];
            Material myMaterial = graphic.MatSingle;

            Mesh myMesh = Flipped ? MeshPool.plane10Flip : MeshPool.plane10;

            if (!ForcePropertyBlock && color.IndistinguishableFrom(myMaterial.color))
            {
                Graphics.DrawMesh(myMesh, matrix, myMaterial, layer, null, 0);
                return;
            }
            propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Graphics.DrawMesh(myMesh, matrix, myMaterial, layer, null, 0, propertyBlock);
        }
    }
}
