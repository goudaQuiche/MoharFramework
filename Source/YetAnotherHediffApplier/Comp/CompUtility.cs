using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;
using Ubet;

namespace YAHA
{
    public static class CompUtility
    {
        public static void SetTriggerOnly(this HediffComp_YetAnotherHediffApplier c)
        {
            c.TriggeredOnlyHediffs = c.Props.associations.All(a => a.specifics.IsTriggered);
        }

        public static void RememberTrigger(List<TriggerEvent> haTriggerEvents, List<TriggerEvent> triggerEventsRegistry, TriggerEvent te)
        {
            if (haTriggerEvents.Contains(te) && !triggerEventsRegistry.Contains(te))
                triggerEventsRegistry.Add(te);
        }

        public static void SetHasAnyWoundTrigger(this HediffComp_YetAnotherHediffApplier c)
        {
            foreach(HediffAssociation ha in c.Props.associations)
            {
                if (!ha.HasSpecifics || !ha.specifics.IsTriggered)
                    continue;

                RememberTrigger(ha.specifics.triggerEvent, c.WoundTriggers, TriggerEvent.anyWound);
                RememberTrigger(ha.specifics.triggerEvent, c.WoundTriggers, TriggerEvent.friendlyFire);
                RememberTrigger(ha.specifics.triggerEvent, c.WoundTriggers, TriggerEvent.enemyWound);
            }
        }

        public static void CreateRegistry(this HediffComp_YetAnotherHediffApplier c)
        {
            if (c.HasRegistry)
                return;

            foreach (HediffAssociation ha in c.Props.associations)
            {
                if (c.Props.debug)
                    Log.Warning("Added 1 HediffAssociation");

                c.Registry.Add(new AssociatedHediffHistory());
            }
        }

        public static void DidNothing(this HediffComp_YetAnotherHediffApplier c)
        {
            if (c.HasEmptyRegistry)
                return;

            foreach (AssociatedHediffHistory curAHH in c.Registry)
            {
                curAHH.DidSomethingThisTick = false;
            }
        }

        public static void WoundTriggerManager(this HediffComp_YetAnotherHediffApplier c, DamageInfo dinfo)
        {
            if (!c.HasWoundTrigger)
                return;

            if (c.WoundTriggers.Contains(TriggerEvent.enemyWound) || c.WoundTriggers.Contains(TriggerEvent.friendlyFire))
            {
                if (dinfo.Instigator.Faction is Faction InstigatorFaction && c.Pawn.Faction is Faction PawnFaction)
                {
                    TriggerEvent te = TriggerEvent.empty;

                    te = PawnFaction.AllyOrNeutralTo(InstigatorFaction) ? TriggerEvent.friendlyFire : TriggerEvent.enemyWound;

                    if (te == TriggerEvent.empty)
                        return;

                    YahaUtility.UpdateDependingOnTriggerEvent(c.Pawn, te);
                }
            }
            else
            {
                YahaUtility.UpdateDependingOnTriggerEvent(c.Pawn, TriggerEvent.anyWound);
            }
        }
    }
}
