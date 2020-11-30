using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharJoy
{
    public static class MoteThrower
    {

        public static Thing ShadowMoteSpawner_ThrowObjectAt(this JobDriver_ThrowRocks JDTR)
        {
            bool MyDebug = JDTR.gameSettings.debug;
            string debugStr = MyDebug ? JDTR.PawnLabel + "ShadowMoteSpawner_ThrowObjectAt - " : "";

            Pawn thrower = JDTR.pawn;
            IntVec3 targetCell = JDTR.TargetCell;
            SkillDef skillDef = JDTR.SkillDefScaling;
            
            ThingDef moteDef = JDTR.MoteDef;

            if (!thrower.AllowedMoteSpawn())
                return null;
            
            float randomSpeed = JDTR.Speed.RandomInRange;
            Vector3 destinationCell =
                targetCell.ToVector3Shifted() +
                Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(skillDef).Level / 20f) * 1.8f);
            
            destinationCell.y = thrower.DrawPos.y;
            if (moteDef == null)
            {
                Tools.Warn(debugStr + "found no moteDef", MyDebug);
                return null;
            }

            ShadowMote moteThrown = (ShadowMote)ThingMaker.MakeThing(moteDef);
            
            //moteThrown.Initialization(thrower.DrawPos, destinationCell, PGTG.PetanqueSpotCell.ToVector3Shifted(), thrower);
            moteThrown.Initialization(JDTR, destinationCell);
            
            moteThrown.Scale = 1f;
            moteThrown.rotationRate = JDTR.Rotation.RandomInRange;
            moteThrown.exactPosition = thrower.DrawPos;
            moteThrown.SetVelocity((destinationCell - moteThrown.exactPosition).AngleFlat(), randomSpeed);
            
            moteThrown.airTimeLeft = Mathf.RoundToInt((moteThrown.exactPosition - destinationCell).MagnitudeHorizontal() / randomSpeed);
            
            return GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);
        }

        public static Thing ShadowMoteSpawner_ThrowObjectAt(this JobDriver_PlayGenericTargetingGame PGTG)
        {
            Pawn thrower = PGTG.pawn;
            IntVec3 targetCell = PGTG.PetanqueSpotCell;
            SkillDef skillDef = PGTG.SkillDefScaling;

            ThingDef moteDef = PGTG.MoteDef;

            if (!thrower.AllowedMoteSpawn())
                return null;

            float randomSpeed = PGTG.Speed.RandomInRange;
            Vector3 destinationCell =
                targetCell.ToVector3Shifted() +
                Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(skillDef).Level / 20f) * 1.8f);

            destinationCell.y = thrower.DrawPos.y;
            ShadowMote moteThrown = (ShadowMote)ThingMaker.MakeThing(moteDef);

            //moteThrown.Initialization(thrower.DrawPos, destinationCell, PGTG.PetanqueSpotCell.ToVector3Shifted(), thrower);
            moteThrown.Initialization(PGTG, destinationCell);

            moteThrown.Scale = 1f;
            moteThrown.rotationRate = PGTG.Rotation.RandomInRange;
            moteThrown.exactPosition = thrower.DrawPos;
            moteThrown.SetVelocity((destinationCell - moteThrown.exactPosition).AngleFlat(), randomSpeed);

            moteThrown.airTimeLeft = Mathf.RoundToInt((moteThrown.exactPosition - destinationCell).MagnitudeHorizontal() / randomSpeed);

            return GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);
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
                Vector3 destinationCell =
                    targetCell.ToVector3Shifted() +
                    Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(skillDef).Level / 20f) * 1.8f);

                destinationCell.y = thrower.DrawPos.y;
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);

                moteThrown.Scale = 1f;
                moteThrown.rotationRate = PGTG.Rotation.RandomInRange;
                moteThrown.exactPosition = thrower.DrawPos;
                moteThrown.SetVelocity((destinationCell - moteThrown.exactPosition).AngleFlat(), randomSpeed);

                moteThrown.airTimeLeft = Mathf.RoundToInt((moteThrown.exactPosition - destinationCell).MagnitudeHorizontal() / randomSpeed);

                return GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);
            }
            return null;
        }

        public static Thing MoteSpawner_ThrowObjectAt(this JobDriver_ThrowRocks JDTR)
        {
            IntVec3 targetCell = JDTR.TargetCell;
            Pawn thrower = JDTR.pawn;
            ThingDef moteDef = JDTR.MoteDef;
            SkillDef skillDef = JDTR.SkillDefScaling;

            if (!thrower.Position.ShouldSpawnMotesAt(thrower.Map) || thrower.Map.moteCounter.Saturated)
                return null;

            float randomSpeed = JDTR.Speed.RandomInRange;
            Vector3 destinationCell =
                targetCell.ToVector3Shifted() +
                Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(skillDef).Level / 20f) * 1.8f);

            destinationCell.y = thrower.DrawPos.y;
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);

            moteThrown.Scale = 1f;
            moteThrown.rotationRate = JDTR.Rotation.RandomInRange;
            moteThrown.exactPosition = thrower.DrawPos;
            moteThrown.SetVelocity((destinationCell - moteThrown.exactPosition).AngleFlat(), randomSpeed);

            moteThrown.airTimeLeft = Mathf.RoundToInt((moteThrown.exactPosition - destinationCell).MagnitudeHorizontal() / randomSpeed);

            return GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);

        }


        private static bool NewBaseImpactMote(this MoteSubEffect MSE, out MoteThrown moteThrown)
        {
            if (!MSE.HasImpactMote)
            {
                Tools.Warn("NewBaseImpactMote - Failed to find impact mote; giving up", MSE.debug);
                moteThrown = null;
                return false;
            }

            moteThrown = (MoteThrown)ThingMaker.MakeThing(MSE.impactMote.moteDef);
            moteThrown.Scale = MSE.impactMote.scale.RandomInRange;
            moteThrown.rotationRate = MSE.impactMote.rotationRate.RandomInRange;

            Tools.Warn("NewBaseImpactMote - ok", MSE.debug);

            return true;
        }

        public static void ThrowImpactMote(this ShadowMote shadowMote)
        {
            if (!shadowMote.HasMSE || !shadowMote.MSE.HasImpactMote)
                return;

            Vector3 loc = shadowMote.exactPosition;
            Map map = shadowMote.Map;

            MoteSubEffect MSE = shadowMote.MSE;

            //Tools.Warn("ThrowImpactMote ; has MSE; has impactMote", MSE.debug);

            if (!loc.AllowedMoteSpawn(map))
                return;
            //Tools.Warn("ThrowImpactMote ; AllowedMoteSpawn", MSE.debug);

            if(!MSE.NewBaseImpactMote(out MoteThrown moteThrown))
            {
                Tools.Warn("ThrowImpactMote ; NewBaseImpactMote ko; giving up", MSE.debug);
                return;
            }
            

            moteThrown.exactPosition = loc;
            //Tools.Warn("ThrowImpactMote ; what", MSE.debug);
            moteThrown.exactPosition += new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f));

            //Tools.Warn("ThrowImpactMote ; exactPosition ok", MSE.debug);

            moteThrown.SetVelocity(
                MSE.impactMote.angle.RandomInRange,
                MSE.impactMote.speed.RandomInRange
            );

            //Tools.Warn("ThrowImpactMote ; SetVelocity ok", MSE.debug);

            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
            Tools.Warn("ThrowImpactMote ; GenSpawn.Spawn ok", MSE.debug);
        }
    }
}
