using UnityEngine;
using Verse;

namespace MoharGfx
{
    public class CustomTransformation_Mote : MoteThrown
    {
        private Vector3 attacheeLastPosition = new Vector3(-1000f, -1000f, -1000f);

        public CustomTransformation_MoteDef Def => def as CustomTransformation_MoteDef;
        public bool MyDebug => Def.debug;

        public int randRot_NextPeriod;
        public bool HasRandomRotation => Def.HasRandomRotation && this.IsHashIntervalTick(randRot_NextPeriod);

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (Def.HasRandomRotation)
            {
                //Log.Warning(Def.defName + "=> HasRandomRotation");
                SetNextPeriod();
            }
        }

        void SetNextPeriod()
        {
            randRot_NextPeriod = Def.transformation.randomRotation.period.RandomInRange;
        }

        public override void Tick()
        {
            base.Tick();

            if (!HasRandomRotation)
                return;

            if (Rand.Chance(Def.transformation.randomRotation.chance))
                exactRotation += (Rand.Chance(.5f) ? -1 : 1) * Def.transformation.randomRotation.randomAngle.RandomInRange;
            SetNextPeriod();
        }
    }
}
