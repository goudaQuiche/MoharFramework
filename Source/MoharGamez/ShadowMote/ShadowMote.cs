﻿using UnityEngine;
using Verse.Sound;
using Verse;

namespace MoharGamez
{
    public class ShadowMote : MoteThrown
    {
        public Vector3 throwerOrigin;
        public Vector3 flatOrigin;
        public Vector3 destination;
        public float BaseDistance;

        public Vector3 targetBuildingCoordinates;

        public JobDriver_PlayGenericTargetingGame TG_parent;
        public bool HasTargetingGameParent => TG_parent != null;
        public Pawn MyPawn => TG_parent?.pawn ?? null;
        public string PawnLabel => HasTargetingGameParent ? TG_parent.PawnLabel : "unknown";


        public bool HasGameSettings => HasTargetingGameParent && TG_parent.HasGameSettings;
        //public bool HasTargetingGameParentWithThoughts => HasGameSettings && TG_parent.gameSettings.HasThought;
        //public bool HasGameWithThrowThoughts => HasTargetingGameParentWithThoughts && TG_parent.gameSettings.HasThrowThought;
        public bool HasGameWithThrowThoughts => HasGameSettings && TG_parent.gameSettings.HasThrowThought;

        public bool IsPlayerPlayingTargetingGame =>
            !MyPawn.NegligeablePawn() && HasTargetingGameParent && TG_parent.HasGameSettings &&
            MyPawn.CurJob != null && MyPawn.CurJobDef == TG_parent.gameSettings.jobDef;

        public Graphic_Shadow GroundShadowGraphic;

        public ShadowMoteDef Def => def as ShadowMoteDef;

        public MoteSubEffect MSE => Def?.moteSubEffect ?? null;
        public bool HasMSE => MSE != null;

        public bool ThoughtDebug => HasGameSettings && TG_parent.gameSettings.debugThrowThought;
        public bool MoteSubEffectDebug => HasMSE && MSE.debug;

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
        public bool SustainerNeedsToStop => ActiveSustainer && Speed < 0.01;
        private Sustainer skiddingSustainer = null;
        private bool ActiveSustainer => skiddingSustainer != null;

        // Needed
        public bool HasGroundShadow => HasGroundShadowData && HasGroundShadowGraphic;
        public bool HasFlyingShadow => HasFlyingShadowMaterial && HasFlyingShadowData;

        // Nature defining
        public bool IsGroundShadowOnly => !HasFlyingShadow && HasGroundShadow;
        public bool IsFlyingAndGroundShadow => HasFlyingShadow && HasGroundShadow;

        private bool IsGrounded => airTimeLeft <= 0;
        private bool IsFlying => !IsGrounded;
        public float DistanceCoveredFraction => Mathf.Clamp01(BaseDistance == 0 ? 1 : (1 - CurrentDistance / BaseDistance));
        public Quaternion ExactRotation => Quaternion.LookRotation((destination - throwerOrigin).Yto0());

        private bool ImpactOccured = false;
        private bool LoggedCoordinates = false;
        private bool ProjectileStoppedMovingForFirstTime => !LoggedCoordinates && Speed == 0;

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

        /*
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref throwerOrigin, "throwerOrigin");
            Scribe_Values.Look(ref flatOrigin, "flatOrigin");
            Scribe_Values.Look(ref destination, "destination");
            Scribe_Values.Look(ref BaseDistance, "BaseDistance");
            Scribe_Values.Look(ref targetBuildingCoordinates, "targetBuildingCoordinates");
            Scribe_Deep.Look(ref TG_parent, "TG_parent");
            //Scribe_Values.Look(ref TG_parent, "TG_parent");
            Scribe_Values.Look(ref skiddingSustainer, "skiddingSustainer");

            Scribe_Values.Look(ref LoggedCoordinates, "LoggedCoordinates");
            Scribe_Values.Look(ref ImpactOccured, "ImpactOccured");
        }
        */

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (HasThrowSound)
                ThrowSound.PlayOneShot(new TargetInfo(DrawPos.ToIntVec3(), Map));
        }

        protected override void TimeInterval(float deltaTime)
        {
            base.TimeInterval(deltaTime);
            if (Destroyed)
                return;

            if (!ImpactOccured)
            {
                if ((IsFlyingAndGroundShadow && !IsFlying) || IsGroundShadowOnly)
                {
                    ImpactOccured = true;
                    StartSustainer();
                    //Tools.Warn("")

                    if (HasImpactMote)
                        this.ThrowImpactMote();
                }
            }

            if (SustainerNeedsToStop)
                StopSustainer();

            /*
            Tools.Warn(
                   pawn.LabelShort +
                   " non moving projectile" +
                   " drawPos: " + DrawPos +
                   " exactPosition: " + exactPosition +
                   " Position: " + Position +
                   " MyPosition: " + MyPosition +
                   " targetBuildingCoordinates: " + targetBuildingCoordinates
                   , myDebug
            );*/
            if (ProjectileStoppedMovingForFirstTime) {
                /*
                Tools.Warn( PawnLabel + " ProjectileStoppedMovingForFirstTime " +
                    " drawPos: " + DrawPos + " exactPosition: " + exactPosition +
                    " Position: " + Position + " MyPosition: " + MyPosition +
                    " targetBuildingCoordinates: " + targetBuildingCoordinates
                    , ThoughtDebug
                );
                */
                LoggedCoordinates = true;
                //if (HasTargetingGameParentWithThoughts) {
                if (HasGameWithThrowThoughts) {
                    //Tools.Warn(PawnLabel + " ProjectileStoppedMovingForFirstTime ", ThoughtDebug);
                    if (IsPlayerPlayingTargetingGame) {
                        //Tools.Warn(PawnLabel + " IsPlayerPlayingTargetingGame ", ThoughtDebug);
                        float ThrowDistance = MyPawn.CalculateThrowDistance(targetBuildingCoordinates, MyPosition, ThoughtDebug);
                        TG_parent.ComputeThrowQuality(ThrowDistance, ThoughtDebug);
                    }
                }
            }
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
                float num2 = (destination - throwerOrigin).MagnitudeHorizontalSquared();
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
