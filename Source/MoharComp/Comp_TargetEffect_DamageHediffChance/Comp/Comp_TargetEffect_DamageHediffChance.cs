using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MoharComp
{
    public class CompProperties_TargetEffect_HediffDamageChance : CompProperties
    {
        public CompProperties_TargetEffect_HediffDamageChance()
        {
            this.compClass = typeof(Comp_TargetEffect_DamageHediffChance);
        }

        public List<ThingDef> backfireImmuneUser;
        public List<ThingDef> immuneVictim;

        public bool HasBackfireImmunitySet => !backfireImmuneUser.NullOrEmpty();
        public bool HasVictimImmunitySet => !immuneVictim.NullOrEmpty();

        public float damageChance = 0f;
        public float hediffChance = 0f;

        public float backfireDamageChance = 0f;
        public float backfirehediffChance = 0f;

        public List<DamageItem> damagePool;
        public List<HediffItem> hediffPool;

        public List<DamageItem> backfireDamagePool;
        public List<HediffItem> backfireHediffPool;

        public bool HasDamage => !damagePool.NullOrEmpty();
        public bool HasHediff => !hediffPool.NullOrEmpty();
        public bool HasBackfireDamage => !backfireDamagePool.NullOrEmpty();
        public bool HasBackfireHediff => !backfireHediffPool.NullOrEmpty();

        public bool debug = false;
    }

    public class Comp_TargetEffect_DamageHediffChance : CompTargetEffect
    {
        protected CompProperties_TargetEffect_HediffDamageChance Props => (CompProperties_TargetEffect_HediffDamageChance)props;

        public bool DebugOn => Props.debug;

        public override void DoEffectOn(Pawn user, Thing target)
        {
            if (!(target is Pawn victim))
                return;

            if (victim.Dead)
                return;

            if (DebugOn)
                Log.Warning(
                    parent.def +" DoEffectOn"
                    + " - agressor: " + user
                    + " victim: " + victim
                    );

            // Damage and Hediff to victim
            if( Props.HasDamage || Props.HasHediff )
            {
                if (Props.HasVictimImmunitySet && !UserIsImmune(user) || !Props.HasVictimImmunitySet)
                {
                    if (Props.HasDamage && Rand.Chance(Props.damageChance))
                    {
                        if (DebugOn) Log.Warning("found damage and dice roll said yes");
                        DoDamage(user, victim, Props.damagePool);
                    }
                    else if (DebugOn) Log.Warning("found no damage or dice roll said no");


                    if (Props.HasHediff && Rand.Chance(Props.hediffChance))
                    {
                        if (DebugOn) Log.Warning("found damage and dice roll said yes");
                        DoHediff(user, victim, Props.hediffPool);
                    }
                    else if (DebugOn) Log.Warning("found no hediff or dice roll said no");
                }
            }

            // Backfire damage and hediff to user
            if (Props.HasBackfireDamage || Props.HasBackfireHediff)
            {
                if (Props.HasBackfireImmunitySet && UserIsImmune(user))
                {
                    if (DebugOn) Log.Warning("found immunity and " + user.LabelShort);
                    return;
                }
                else if (DebugOn) Log.Warning("found no immunity or " + user.LabelShort + " thingDef not in <immuneToBackfireUser>");
            }
            else
            {
                if (DebugOn) Log.Warning("found no backfire damage or hediff");
                return;
            }

            if (Props.HasBackfireDamage && Rand.Chance(Props.backfireDamageChance))
            {
                if (DebugOn) Log.Warning("found backfire damage and dice roll said yes");
                DoDamage(user, user, Props.backfireDamagePool);
            } else if (DebugOn) Log.Warning("found no backfire damage or dice roll said no");

            if (Props.HasBackfireHediff && Rand.Chance(Props.backfirehediffChance)) {
                if (DebugOn) Log.Warning("found backfire hediff and dice roll said yes");
                DoHediff(user, user, Props.backfireHediffPool);
            } else if (DebugOn) Log.Warning("found no backfire damage or dice roll said no");

        }

        public bool UserIsImmune(Pawn actor)
        {
            return Props.backfireImmuneUser.Contains(actor.def);
        }

        public bool VictimIsImmune(Pawn victim)
        {
            return Props.immuneVictim.Contains(victim.def);
        }

        public void DoDamage(Pawn agressor, Pawn victim, List<DamageItem> dPool)
        {
            if (dPool.NullOrEmpty())
                return;

            DamageItem di = dPool.PickRandomWeightedItem();
            BodyPartRecord bpr = null;

            if (di.HasBodyPartTag)
                victim.RaceProps.body.GetPartsWithTag(di.bodyPartTag).TryRandomElement(out bpr);

            if (bpr == null)
                return;

            float damageAmount = di.damageAmount.RandomInRange;
            victim.TakeDamage(new DamageInfo(di.damageDef, damageAmount, 0f, -1f, agressor, bpr, parent.def, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
        }

        public void DoHediff(Pawn agressor, Pawn victim, List<HediffItem> hPool)
        {
            if (hPool.NullOrEmpty())
                return;

            HediffItem hi = hPool.PickRandomWeightedItem();
            BodyPartRecord bpr = null;

            if (hi.HasBodyPartTag)
                victim.RaceProps.body.GetPartsWithTag(hi.bodyPartTag).TryRandomElement(out bpr);

            Hediff hediff = HediffMaker.MakeHediff(hi.hediffDef, victim, bpr);
            hediff.Severity = hi.severity.RandomInRange;

            BattleLogEntry_ItemUsed battleLogEntry_ItemUsed = new BattleLogEntry_ItemUsed(agressor, victim, parent.def, RulePackDefOf.Event_ItemUsed);
            hediff.combatLogEntry = new WeakReference<LogEntry>(battleLogEntry_ItemUsed);
            hediff.combatLogText = battleLogEntry_ItemUsed.ToGameStringFromPOV(null);
            victim.health.AddHediff(hediff, bpr);

            Find.BattleLog.Add(battleLogEntry_ItemUsed);
        }
    }
}