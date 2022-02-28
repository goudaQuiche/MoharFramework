using Verse;
using System;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;

namespace YAHA
{
    public static class YahaUtility
    {
        //public static readonly MethodInfo ParentHolder = AccessTools.Method(type: typeof(IThingHolder), name: "ParentHolder");
        //shortHashGiver.Invoke(obj: null, parameters: new object[] {yourDef, typeof(yourDefType)});

        public static void CheckTriggeredAssociations(IEnumerable<Hediff> YahaHediffs, TriggerEvent triggerEvent)
        {
            foreach (Hediff h in YahaHediffs)
            {
                HediffComp_YetAnotherHediffApplier YahaComp = h.TryGetComp<HediffComp_YetAnotherHediffApplier>();
                bool MyDebug = YahaComp.Props.debug;
                if (MyDebug)
                    Log.Warning("CheckTriggeredAssociations - Found " + h.def.defName + " Yaha hediff");

                IEnumerable<int> indexes = YahaComp.GetTriggeredHediffAssociationIndex(triggerEvent, MyDebug);
                if (indexes.EnumerableNullOrEmpty())
                {
                    if (MyDebug)
                        Log.Warning("No " + h.def.defName + " Yaha hediff found with " + triggerEvent.GetDesc());
                    return;
                }

                foreach (int i in indexes)
                {
                    HediffAssociation CurHA = YahaComp.Props.associations[i];
                    AssociatedHediffHistory CurAHH = YahaComp.Registry[i];

                    if (MyDebug)
                    {
                        Log.Warning("CheckTriggeredAssociations - Found " + triggerEvent.GetDesc() + " ; i=" + i);
                    }

                    YahaComp.UpdateHediffDependingOnConditionsItem(CurHA, CurAHH, false, MyDebug);
                }
            }
        }

        public static void UpdateDependingOnTriggerEvent(Pawn p, TriggerEvent te, bool debug=false)
        {
            IEnumerable<Hediff> allYahaHediffs = p.health.hediffSet.hediffs.Where(hi => hi.TryGetComp<HediffComp_YetAnotherHediffApplier>() != null);

            if (allYahaHediffs.EnumerableNullOrEmpty())
                return;

            CheckTriggeredAssociations(allYahaHediffs, te);
        }

    }
}
