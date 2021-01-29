using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;


namespace MoharHediffs
{
    public static class InnerShinerUtils
    {
        public static void SelfDestroy(this HediffComp_InnerShine comp)
        {
            comp.parent.Severity = 0;
            comp.Pawn.health.RemoveHediff(comp.parent);
        }

        public static void ChangeMoteColor(this InnerShineItem ISI, InnerShineRecord ISR, Mote mote)
        {
            if (!ISI.HasColorRange || mote == null)
                return;

            if (ISR.lastColor == Color.black)
                ISR.lastColor = ISI.colorRange.colorA;

            ISR.lastColor = ISI.colorRange.RandomPickColor(ISR.lastColor, ISI.debug);

            mote.instanceColor = ISR.lastColor;
        }

        public static void GetSpecifities(this InnerShineItem ISI, Pawn p, out Vector3 offset, out float scale)
        {
            offset = Vector3.zero;
            scale = 1;

            if (p.story?.bodyType == null || !ISI.HasBodyTypeDrawRules) 
            {
                if (ISI.HasDefaultDrawRules)
                {
                    offset = ISI.defaultDrawRules.offset;
                    scale = ISI.defaultDrawRules.randomScale.RandomInRange;
                }
                 return;
            }

            BodyTypeSpecificities BTS = ISI.bodyTypeDrawRules.Where(b => b.bodyTypeDef == p.story.bodyType).FirstOrFallback();
            if (BTS == null)
            {
                if (ISI.HasDefaultDrawRules)
                {
                    offset = ISI.defaultDrawRules.offset;
                    scale = ISI.defaultDrawRules.randomScale.RandomInRange;
                }

                return;
            }

            offset = BTS.drawRules.offset;
            scale = BTS.drawRules.randomScale.RandomInRange;
        }

        public static bool ShouldSpawnMote(this InnerShineItem ISI, InnerShineRecord ISR, Pawn p)
        {
            if (!ISI.HasCompatibleActivity(p))
                return false;

            if (ISI.HasMoteNumLimit())
            {
                return !ISR.AlreadyReachedMax(ISI.spawningRules.spawnedMax);
            }

            return true;
        }

        public static Vector3 GetDrawOffset(this InnerShineItem ISI, Pawn p)
        {
            if (p.story?.bodyType == null || !ISI.HasBodyTypeDrawRules)
            {
                if (ISI.HasDefaultDrawRules)
                    return ISI.defaultDrawRules.offset;
                else
                    return Vector3.zero;
            }

            BodyTypeSpecificities BTS = ISI.bodyTypeDrawRules.Where(b => b.bodyTypeDef == p.story.bodyType).FirstOrFallback();
            if (BTS == null)
                return ISI.HasDefaultDrawRules ? ISI.defaultDrawRules.offset : Vector3.zero;

            return BTS.drawRules.offset;
        }

        public static bool AlreadyReachedMax(this InnerShineRecord ISR, int max)
        {
            if (ISR.spawned.NullOrEmpty())
                return false;

            return ISR.spawned.Count() >= max;
        }

        public static bool HasCompatibleActivity(this InnerShineItem ISI, Pawn p)
        {
            if (!ISI.HasRestriction)
                return true;

            ActivityRestriction restrict = ISI.restriction;

            if (restrict.HasPostureRestriction && !restrict.allowedPostures.Contains(p.GetPosture()))
                return false;

            if (restrict.HasJobRestriction && p.CurJob != null && !restrict.allowedJobs.Contains(p.CurJob.def))
                return false;

            if (restrict.HasAllowedRotation && !restrict.allowedRotation.Contains(p.Rotation))
                return false;

            return true;
        }
    }
}
