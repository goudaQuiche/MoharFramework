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

        public Vector3 targetBuildingCoordinates;
        public Pawn pawn;

        private Graphic_Shadow GroundShadowGraphic;

        public ShadownMoteDef Def => def as ShadownMoteDef;

        public MoteSubEffect MSE => Def?.moteSubEffect ?? null;
        public bool HasMSE => MSE != null;

        public bool myDebug = true;

        // Flying shadow
        public ThingDef FlyingShadowTex => Def?.moteSubEffect?.flyingShadowRessource ?? null;
        public bool HasFlyingShadowTex => FlyingShadowTex != null;
        public Material FlyingShadowMaterial => FlyingShadowTex?.DrawMatSingle ?? null;
        public bool HasFlyingShadowMaterial => FlyingShadowMaterial != null;
        public ProjectileProperties FlyingShadowData => def?.projectile ?? null;
        public bool HasFlyingShadowData => FlyingShadowData != null;

        // Ground shadow
        public bool HasGroundShadowGraphic => GroundShadowGraphic != null;
        public ShadowData GroundShadowData => def?.graphicData?.shadowData ?? null;
        public bool HasGroundShadowData => GroundShadowData != null;

        //Impact
        public bool HasImpactMote => MSE.HasImpactMote;

        // Sound
        public SoundDef ThrowSound => MSE.throwSound ?? null;
        public SoundDef SkiddingSustainSound => MSE.skiddingSustainSound ?? null;
        public bool HasThrowSound => MSE.HasThrowSound;
        public bool HasSkiddingSound => MSE.HasSkiddingSound;

        // Needed
        public bool HasGroundShadow => HasGroundShadowData && HasGroundShadowGraphic;
        public bool HasFlyingShadow => HasFlyingShadowMaterial && HasFlyingShadowData;

        // Nature defining
        public bool IsGroundShadowOnly => !HasFlyingShadow && HasGroundShadow;
        public bool IsFlyingAndGroundShadow => HasFlyingShadow && HasGroundShadow;

        private bool IsGrounded => airTimeLeft <= 0;
        private bool IsFlying => !IsGrounded;
        public float DistanceCoveredFraction => Mathf.Clamp01(BaseDistance == 0 ? 1 : (1 - CurrentDistance / BaseDistance));
        public Quaternion ExactRotation => Quaternion.LookRotation((destination - origin).Yto0());

        private Sustainer skiddingSustainer = null;
        private bool ImpactOccured = false;
        private bool ActiveSustainer => skiddingSustainer != null;

        private bool LoggedCoordinates = false;

        void StartSustainer()
        {
            skiddingSustainer = SkiddingSustainSound.TrySpawnSustainer(new TargetInfo(Position, Map));
        }

        private void StopSustainer()
        {
            if (skiddingSustainer == null)
                return;

            skiddingSustainer.End();
            skiddingSustainer = null;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (HasThrowSound)
                ThrowSound.PlayOneShot(new TargetInfo(DrawPos.ToIntVec3(), Map));

            if (HasMSE)
                myDebug = MSE.debug;
        }

        protected override void TimeInterval(float deltaTime)
        {
            base.TimeInterval(deltaTime);
            if (Destroyed)
                return;

            if (!ImpactOccured)
            {
                if ( (IsFlyingAndGroundShadow && !IsFlying) || IsGroundShadowOnly)
                {
                    ImpactOccured = true;
                    StartSustainer();

                    if (HasImpactMote)
                        this.ThrowImpactMote();
                }
            }

            if (ActiveSustainer && Speed < 0.01)
                StopSustainer();

            if (!LoggedCoordinates && Speed == 0)
            {
                Tools.Warn(
                    pawn.LabelShort +
                    " non moving projectile" +
                    " drawPos: " + DrawPos +
                    " exactPosition: " + exactPosition +
                    " Position: " + Position +
                    " MyPosition: " + MyPosition +
                    " targetBuildingCoordinates: " + targetBuildingCoordinates
                    , myDebug
                );
                pawn.CalculateThrowDistance(targetBuildingCoordinates, MyPosition, myDebug);
                LoggedCoordinates = true;
            }
        }


        public void Initialization(Vector3 nOrigin, Vector3 nDestination, Vector3 nTarget, Pawn p)
        {
            InitCoordinates(nOrigin, nDestination, nTarget, p);
            InitGroundShadowGraphic();
        }

        private void InitCoordinates(Vector3 nOrigin, Vector3 nDestination, Vector3 nTarget, Pawn p)
        {
            origin = nOrigin;
            destination = nDestination;
            flatOrigin = origin;

            flatOrigin.y = nDestination.y = 0;
            BaseDistance = Vector3.Distance(flatOrigin, nDestination);

            targetBuildingCoordinates = nTarget;
            pawn = p;
        }

        private void InitGroundShadowGraphic()
        {
            if (HasGroundShadowGraphic)
                return;
            if (!HasGroundShadowData)
                return;

            GroundShadowGraphic = new Graphic_Shadow(GroundShadowData);
        }

        public float CurrentDistance
        {
            get
            {
                Vector3 flatCurPos = new Vector3
                {
                    x = DrawPos.x,
                    y = 0,
                    z = DrawPos.z
                };

                return Vector3.Distance(flatOrigin, flatCurPos);
            }
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

        float ArcRatio => IsFlyingAndGroundShadow ? ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction) : 0;
        Vector3 MyPosition => DrawPos + Vector3.forward * ArcRatio;

        public override void Draw()
        {
            Vector3 position = MyPosition;
            position.y = def.altitudeLayer.AltitudeFor();

            if (IsFlying)
            {
                if (IsFlyingAndGroundShadow)
                    DrawFlyingShadow(DrawPos, ArcRatio);
                else if(IsGroundShadowOnly)
                    GroundShadowGraphic.Draw(position, Rot4.North, this);
            }
            else if (IsGrounded)
            {
                if (HasGroundShadowGraphic)
                { 
                    GroundShadowGraphic.Draw(position, Rot4.North, this);
                    //Log.Warning("ShadownMote Position:" + position + " - shadow layer:" + AltitudeLayer.Shadows.AltitudeFor());
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
