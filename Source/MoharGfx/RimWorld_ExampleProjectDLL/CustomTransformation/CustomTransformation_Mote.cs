using UnityEngine;
using Verse;
using System;

namespace MoharGfx
{
    public class CustomTransformation_Mote : MoteThrown
    {
        /*
        private Vector3 linkDrawPos = new Vector3(-1000f, -1000f, -1000f);
        public bool isLinked = false;
        */

        public CustomTransformation_MoteDef Def => def as CustomTransformation_MoteDef;
        public bool MyDebug => Def.debug;
        public string MainDebugStr => MyDebug ? Def.defName + " CustomTransformation_Mote - " : string.Empty;

        // Color Alpha
        public bool HasAlpha => Def.HasAlpha;

        // periodic random rotation
        public int randRot_NextPeriod;
        public bool IsPeriodicRandomRotationTime => Def.HasPeriodicRandomRotation && this.IsHashIntervalTick(randRot_NextPeriod);

        //Straighten up
        int TickAge => Find.TickManager.TicksGame - spawnTick;
        int TickLifeSpan => (int)(Def.mote.Lifespan * 60);
        float LifeSpentRatio => (float)TickAge / TickLifeSpan;

        public StraightenUpRotation SUDef => Def.transformation.rotation.straightenUp;
        public float SUAim => SUDef.aimedRotation;
        public float SUTolerance => SUDef.tolerance;
        public float SUGoalRatio => SUDef.goalLifeSpanRatio;

        public bool HasStraightenUp => Def.HasStraightenUp;
        public float SUWorkTodo => Math.Abs(SUAim - exactRotation);
        public bool SUReachedGoal => SUWorkTodo < SUTolerance;

        public bool SUReached = false;
        public bool NeedsStraightenUp => HasStraightenUp && !SUReached;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            //if (MyDebug) Log.Warning(MainDebugStr + "SpawnSetup");

            if (Def.HasPeriodicRandomRotation)
            {
                //Log.Warning(Def.defName + "=> HasRandomRotation");
                SetNextPeriod();
            }
        }

        void SetNextPeriod()
        {
            randRot_NextPeriod = Def.transformation.rotation.periodicRandRot.period.RandomInRange;
        }

        public override float Alpha => HasAlpha ? Def.transformation.color.arbitraryAlpha * base.Alpha : base.Alpha;

        float GetSUExactRotation
        {
            get
            {
                if (exactRotation > 180)
                    return exactRotation - 360;
                else if (exactRotation < -180)
                    return exactRotation + 360;
                else
                    return exactRotation;
            }
        }

        public bool TryStraightenUp()
        {
            /*
            if (MyDebug && HasStraightenUp)
            {
                Log.Warning(
                    $" {SUAim} {exactRotation} => {SUWorkTodo} ? {SUTolerance}"+
                    "NeedsStraightenUp:" + NeedsStraightenUp +
                    "; ReachedStraightenUpGoal:" + ReachedStraightenUpGoal
                );
            }
            */

            if (!NeedsStraightenUp)
                return false;

            if (SUDef.IsWithinGracePeriod(LifeSpentRatio))
            {
                //if (MyDebug) Log.Warning("Is in grace period: " + LifeSpentRatio);
                return false;
            }
            //else if (MyDebug) Log.Warning("Is not within grace period: " + LifeSpentRatio);

            //if (MyDebug) Log.Warning("trying to straighten up");
            float SURotation = GetSUExactRotation;

            //rotationLeftToApply
            float diff = SUAim - SURotation;
            int ticksUntilLimit = (int)(SUGoalRatio * TickLifeSpan - TickAge);
            if (ticksUntilLimit != 0)
            {
                float IncRot = diff / ticksUntilLimit;
                //if (MyDebug) Log.Warning($"nowRotation{SURotation} diff{diff} ticksUntilLimit{ticksUntilLimit} IncRot{IncRot}");
                exactRotation += IncRot;
            }
            SUReached = SUReachedGoal;

            return true;
        }

        public void TryPeriodicRandomRotation()
        {
            if (!IsPeriodicRandomRotationTime)
                return;

            if (Rand.Chance(Def.transformation.rotation.periodicRandRot.chance))
                exactRotation += (Rand.Chance(.5f) ? -1 : 1) * Def.transformation.rotation.periodicRandRot.randomAngle.RandomInRange;
            SetNextPeriod();
        }

        public override void Tick()
        {
            base.Tick();

            //if (MyDebug) Log.Warning(MainDebugStr + "ticking Def.HasStraightenUp:" + Def.HasStraightenUp);
            TryStraightenUp();
            //if ( TryStraightenUp() && MyDebug) Log.Warning("Did straighten Up");
            TryPeriodicRandomRotation();
        }

        /*
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (IsLinked)
            {

            }
            base.Destroy(mode);

        }
        */
    }
}
