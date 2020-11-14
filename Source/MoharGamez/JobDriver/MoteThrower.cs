using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharGamez
{
    public static class MoteThrower
    {
 
        public static void ThrowObjectAt(this JobDriver_PlayGenericTargetingGame PGTG, IntVec3 targetCell)
        {
            Pawn thrower = PGTG.pawn;
            ThingDef moteDef = PGTG.MoteDef;
            SkillDef skillDef = PGTG.SkillDefScaling;

            if (thrower.Position.ShouldSpawnMotesAt(thrower.Map) && !thrower.Map.moteCounter.Saturated)
            {
                float randomSpeed = PGTG.Speed.RandomInRange;
                Vector3 vector = 
                    targetCell.ToVector3Shifted() + 
                    Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(skillDef).Level / 20f) * 1.8f);

                vector.y = thrower.DrawPos.y;
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);

                moteThrown.Scale = 1f;
                moteThrown.rotationRate = PGTG.Rotation.RandomInRange;
                moteThrown.exactPosition = thrower.DrawPos;
                moteThrown.SetVelocity((vector - moteThrown.exactPosition).AngleFlat(), randomSpeed);

                moteThrown.airTimeLeft = Mathf.RoundToInt((moteThrown.exactPosition - vector).MagnitudeHorizontal() / randomSpeed);

                GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);
            }
        }

        public static Thing MoteSpawner_ThrowObjectAt(this JobDriver_PlayGenericTargetingGame PGTG)
        {
            IntVec3 targetCell = PGTG.PetanqueSpotCell;
            Pawn thrower = PGTG.pawn;
            ThingDef moteDef = PGTG.MoteDef;
            SkillDef skillDef = PGTG.SkillDefScaling;

            if (thrower.Position.ShouldSpawnMotesAt(thrower.Map) && !thrower.Map.moteCounter.Saturated)
            {
                float randomSpeed = PGTG.Speed.RandomInRange;
                Vector3 vector =
                    targetCell.ToVector3Shifted() +
                    Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(skillDef).Level / 20f) * 1.8f);

                vector.y = thrower.DrawPos.y;
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);

                moteThrown.Scale = 1f;
                moteThrown.rotationRate = PGTG.Rotation.RandomInRange;
                moteThrown.exactPosition = thrower.DrawPos;
                moteThrown.SetVelocity((vector - moteThrown.exactPosition).AngleFlat(), randomSpeed);

                moteThrown.airTimeLeft = Mathf.RoundToInt((moteThrown.exactPosition - vector).MagnitudeHorizontal() / randomSpeed);

                return GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);
            }

            return null;
        }

        private static MoteThrown NewBaseImpactMote(this JobDriver_PlayGenericTargetingGame PGTG)
        {
            MoteThrown obj = (MoteThrown)ThingMaker.MakeThing(PGTG.ImpactMoteDef);
            obj.Scale = PGTG.projectileOption.impactMoteParam.scale.RandomInRange;
            obj.rotationRate = PGTG.projectileOption.impactMoteParam.rotationRate.RandomInRange;

            return obj;
        }

        public static void ThrowImpactMote(this JobDriver_PlayGenericTargetingGame PGTG, MoteThrown projectileMote)
        {
            Vector3 loc = projectileMote.DrawPos;
            Map map = PGTG.pawn.Map;

            if (loc.ToIntVec3().ShouldSpawnMotesAt(map) && !map.moteCounter.SaturatedLowPriority)
            {
                MoteThrown moteThrown = PGTG.NewBaseImpactMote();
                moteThrown.exactPosition = loc;

                moteThrown.exactPosition += new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f));

                moteThrown.SetVelocity(
                    PGTG.projectileOption.impactMoteParam.angle.RandomInRange, 
                    PGTG.projectileOption.impactMoteParam.speed.RandomInRange
                );

                GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
            }
        }
    }
}
