using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;

namespace MoharDamage
{
    public static class ApplyEffect
    {
        public static bool TryApplyEffect(this DamageWorker_ExplosionOnImpact DWEOI, Pawn p)
        {
            if (p == null)
                return false;

            if (DWEOI.HasVictimEffect)
            {
                if (DWEOI.VictimEffect.HasImmuneThings && !DWEOI.VictimEffect.immuneToEffectThings.Contains(p.def))
                {
                    bool WillDoIt = false;
                    float baseChance = DWEOI.VictimEffect.chanceMultiplier.RandomInRange;
                    if (DWEOI.VictimEffect.HasResistance)
                        baseChance *= p.GetStatValue(DWEOI.VictimEffect.resistanceStatDef);

                    WillDoIt = Rand.Chance(baseChance);

                    if (WillDoIt)
                    {
                        if ((p.CurJob == null) || p.CurJob.def != DWEOI.VictimEffect.jobDef)
                        {
                            p.jobs.StartJob(
                                JobMaker.MakeJob(DWEOI.VictimEffect.jobDef)
                                , JobCondition.InterruptForced, null
                                , resumeCurJobAfterwards: DWEOI.VictimEffect.resumeJobAfterWards
                            );
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
