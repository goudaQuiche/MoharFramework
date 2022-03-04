using Verse;
using System;
using RimWorld;

namespace DUDOD
{
    public class HediffComp_DestroyUponDeathOrDowned : HediffComp
    {
        Thing RememberWeapon = null;
        private bool myDebug = false;

        public HediffCompProperties_DestroyUponDeathOrDowned Props => (HediffCompProperties_DestroyUponDeathOrDowned)props;

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
                if (myDebug) Log.Warning(Pawn.LabelShort + " is dead and will get destroyed");
                if (Pawn.Corpse == null)
                {
                    if (myDebug) Log.Warning(Pawn.LabelShort + " found no corpse to work with, wont do anything");
                    return false;
                }
                Corpse corpse = Pawn.Corpse;

                if (Props.StripBeforeDeath && corpse.AnythingToStrip())
                    corpse.Strip();

                corpse.DeSpawn();
            }
            else if(Pawn.Downed)
            {
                if (myDebug) Log.Warning(Pawn.LabelShort + " is downed and will get destroyed");
                if (Props.StripBeforeDeath && Pawn.AnythingToStrip())
                    Pawn.Strip();

                Pawn.Destroy();
            }
            else
            {
                if (myDebug) Log.Warning(Pawn.LabelShort + " How?");
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
