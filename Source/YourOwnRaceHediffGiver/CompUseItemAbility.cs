using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace LTF_Slug
{
    public class CompUseItemAbility : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            LockdownEffect.UseCrystalHeart(usedBy);

            SoundDef.Named("LTF_Crystal_Lockdown").PlayOneShotOnCamera(usedBy.MapHeld);
        }
    }
}