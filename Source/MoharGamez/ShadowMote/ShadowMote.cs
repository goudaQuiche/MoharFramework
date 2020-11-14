using UnityEngine;
using Verse.Sound;
using Verse;

namespace MoharGamez
{
    public class ShadowMote : MoteThrown
    {
        public Vector3 origin;
        public Vector3 flatOrigin;
        public Vector3 destination;
        public float BaseDistance;

        private Graphic_Shadow GroundShadowGraphic;

        public ShadownMoteDef Def
        {
            get
            {
                return def as ShadownMoteDef;
            }
        }
        public float DistanceCoveredFraction => Mathf.Clamp01(BaseDistance == 0 ? 1 : (1 - CurrentDistance / BaseDistance));
        public Quaternion ExactRotation => Quaternion.LookRotation((destination - origin).Yto0());

        public bool HasFlyingShadow => def.projectile.shadowSize > 0f;
        public Material FlyingShadowMaterial => Def.shadowMaterialTex.DrawMatSingle;
        public bool HasFlyingShadowMaterial => FlyingShadowMaterial != null;

        public ShadowData GroundShadowData => def.graphicData?.shadowData;
        public bool HasGroundShadowData => GroundShadowData != null;
        public bool HasGroundShadowGraphic => GroundShadowGraphic != null;

        private bool IsGrounded => airTimeLeft <= 0;
        private bool IsFlying => !IsGrounded;

        public void Initialization(Vector3 nOrigin, Vector3 nDestination)
        {
            InitCoordinates(nOrigin, nDestination);
            InitGroundShadowGraphic();
        }

        private void InitCoordinates(Vector3 nOrigin, Vector3 nDestination)
        {
            origin = nOrigin;
            destination = nDestination;
            flatOrigin = origin;

            flatOrigin.y = nDestination.y = 0;
            BaseDistance = Vector3.Distance(flatOrigin, nDestination);
        }

        private void InitGroundShadowGraphic()
        {
            if (GroundShadowGraphic != null || !HasGroundShadowData)
                return;

            GroundShadowGraphic = new Graphic_Shadow(GroundShadowData);
        }

        public float CurrentDistance
        {
            get
            {
                Vector3 flatCurPos = new Vector3();
                flatCurPos.x = DrawPos.x;
                flatCurPos.y = 0;
                flatCurPos.z = DrawPos.z;

                return Vector3.Distance(flatOrigin, flatCurPos);
            }
        }

        protected override void TimeInterval(float deltaTime)
        {
            base.TimeInterval(deltaTime);
            
        }
        /*
        protected virtual Vector3 NextExactPosition(float deltaTime)
        {
            return exactPosition + velocity * deltaTime;
        }
        */

        private float ArcHeightFactor
        {
            get
            {
                float num = def.projectile.arcHeightFactor;
                float num2 = (destination - origin).MagnitudeHorizontalSquared();
                if (num * num > num2 * 0.2f * 0.2f)
                {
                    num = Mathf.Sqrt(num2) * 0.2f;
                }
                return num;
            }
        }

        public override void Draw()
        {
            float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction);
            Vector3 position = DrawPos + new Vector3(0f, 0f, 1f) * num;
            position.y = def.altitudeLayer.AltitudeFor();

            if (IsFlying)
            {
                if (HasFlyingShadow)
                    DrawFlyingShadow(DrawPos, num);
            }
            else if (IsGrounded)
            {
                if (HasGroundShadowGraphic)
                {
                    /*
                    Vector3 groundShadowPos = DrawPos;
                    groundShadowPos.y = def.altitudeLayer.AltitudeFor();
                    */
                    GroundShadowGraphic.Draw(position, Rot4.North, this);
                    Log.Warning("ShadownMote Position:" + position + " - shadow layer:" + AltitudeLayer.Shadows.AltitudeFor());
                }

            }

            Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), position, ExactRotation, def.DrawMatSingle, 0);
            //Comps_PostDraw();
        }

        private void DrawFlyingShadow(Vector3 drawLoc, float height)
        {
            if (!HasFlyingShadowMaterial)
                return;

            float num = def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
            Vector3 s = new Vector3(num, 1f, num);
            Vector3 b = new Vector3(0f, -0.01f, 0f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawLoc + b, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, FlyingShadowMaterial, 0);

        }
    }


}
