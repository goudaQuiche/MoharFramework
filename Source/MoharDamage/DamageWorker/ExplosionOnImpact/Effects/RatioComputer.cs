using Verse;
using RimWorld;
using System;

namespace MoharDamage
{
    public static class RatioComputer
    {
        static float SkillRatioOrMin(this Pawn p, SkillDef skillDef, float min)
        {
            if (p == null || p.skills.GetSkill(skillDef).TotallyDisabled)
                return min;

            return p.skills.GetSkill(skillDef).Level / 20f;
        }

        static float SkillDiceThrow(this Pawn p, SkillDef skillDef, float min)
        {
            float skillRatio = p.SkillRatioOrMin(skillDef, min);
            return Rand.Range(skillRatio * skillRatio, skillRatio);
        }

        public static float ChanceToTriggerExplosion(this DamageWorker_ExplosionOnImpact DWEOI, Pawn Instigator)
        {
            float chanceBase = SkillDiceThrow(Instigator, DWEOI.Chance.linkedSkill, DWEOI.Chance.limits.min);
            float curvedChance = DWEOI.Chance.chanceCurve.Evaluate(chanceBase);

            curvedChance *= DWEOI.Chance.randomChanceFactor.RandomInRange;

            float finaleChance = Math.Min(Math.Max(curvedChance, DWEOI.Chance.limits.min), DWEOI.Chance.limits.max);

            return finaleChance;
        }

        // not a ratio : finale damage
        public static float PawnWeightedExplosionDamage(this DamageWorker_ExplosionOnImpact DWEOI, Pawn Instigator, float DamageBase)
        {
            float skillDiceThrow = SkillDiceThrow(Instigator, DWEOI.Nature.damageLinkedSkill, DWEOI.Nature.damageRatioMin);
            float curvedValue = DWEOI.Nature.damageCurve.Evaluate(skillDiceThrow);

            float multipliedValue = curvedValue * DWEOI.Nature.damageMultiplier.RandomInRange * DamageBase;

            float finaleDmg = Math.Min(Math.Max(multipliedValue, DWEOI.Nature.damageLimit.min), DWEOI.Nature.damageLimit.max);

            return finaleDmg;
        }

        // not a ratio : finale radius
        public static float PawnWeightedExplosionRadius(this DamageWorker_ExplosionOnImpact DWEOI, Pawn Instigator)
        {
            float skillDiceThrow = SkillDiceThrow(Instigator, DWEOI.Nature.radiusLinkedSkill, DWEOI.Nature.radiusRatioMin);

            float curvedValue = DWEOI.Nature.radiusCurve.Evaluate(skillDiceThrow);
            float finaleRadius = curvedValue * DWEOI.Nature.radius.RandomInRange;

            return finaleRadius;
        }
    }
}
