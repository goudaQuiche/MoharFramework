using Verse;
using System;
using RimWorld;

namespace DUDOD
{
    public class HediffComp_DestroyUponDeathOrDowned : HediffComp
    {
        Thing RememberWeapon = null;

        private bool myDebug = false;

        public HediffCompProperties_DestroyUponDeathOrDowned Props
        {
            get
            {
                return (HediffCompProperties_DestroyUponDeathOrDowned)this.props;
            }
        }

        public override void CompPostMake()
        {
            myDebug = Props.debug;
        }

        public void MemorizeWeapon()
        {
            RememberWeapon = Pawn.equipment.Primary ?? null;
        }

        private bool PawnDestroy()
        {
            if (Pawn.Dead)
            {
                Tools.Warn(Pawn.LabelShort + " is dead and will get destroyed", myDebug);
                if (Pawn.Corpse == null)
                {
                    Tools.Warn("found no corpse to work with, wont do anything", myDebug);
                    return false;
                }
                Corpse corpse = Pawn.Corpse;

                if (Props.StripBeforeDeath && corpse.AnythingToStrip())
                    corpse.Strip();

                corpse.DeSpawn();
            }
            else if(Pawn.Downed)
            {
                Tools.Warn(Pawn.LabelShort + " is downed and will get destroyed", myDebug);
                if (Props.StripBeforeDeath && Pawn.AnythingToStrip())
                    Pawn.Strip();

                Pawn.Destroy();
            }
            else
            {
                Tools.Warn("How?", myDebug);
            }

            if (Props.DestroyWeapon && RememberWeapon != null && RememberWeapon.Spawned)
                RememberWeapon.Destroy();

            return true;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if(Find.TickManager.TicksGame % Props.WeaponRefreshRate == 0)
                MemorizeWeapon();
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();

            if(Props.DestroyUponDeath)
                PawnDestroy();
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);

            if(Props.DestroyUponDown && Pawn.Downed)
                PawnDestroy();
        }

    }
}
