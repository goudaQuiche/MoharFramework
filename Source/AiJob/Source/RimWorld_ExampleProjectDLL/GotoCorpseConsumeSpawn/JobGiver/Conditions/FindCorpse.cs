using Verse;
using Verse.AI;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace MoharAiJob
{
    public static class FindCorpse
    {
        private static float GetHitPointsPerc(this Thing t) => (float)t.HitPoints / t.MaxHitPoints;

        public static bool ValidateCorpse(this CorpseSpecification CS, Thing t, Pawn worker, bool myDebug = false, string callingFunc = "")
        {
            Faction pFaction = worker.Faction;
            Map map = worker.Map;

            string DebugStr = myDebug ? t?.ThingID + " ValidateCorpse (" + callingFunc + ")- " : string.Empty;
            if (myDebug)
                Log.Warning(
                    DebugStr + " Trying to validate ... " +
                    "NullMap:" + (map == null) + " mapId" + map?.uniqueID +
                    "; Faction:" + (pFaction != null) + " FacN:" + pFaction?.GetCallLabel()
                    );

            /*
            if (t.NegligibleThing())
            {
                if (myDebug) Log.Warning("ValidateCorpse - negligible thing");
                return false;
            }
            */

            if (!(t is Corpse))
            {
                if (myDebug) Log.Warning(DebugStr + " is not corpse");
                return false;
            }

            if (CS.HasCorpseCategoryDef && !CS.categoryDef.Any(tc => t.def.IsWithinCategory(tc)))
            //if (CS.HasCorpseCategoryDef && !CS.categoryDef.Where(cd => t.def.IsWithinCategory(cd)).EnumerableNullOrEmpty())
            {
                if (myDebug) Log.Warning(DebugStr + " is not within allowed categories");
                return false;
            }

            if (CS.HasRelevantHealthPerc && !CS.healthPerc.Includes(t.GetHitPointsPerc()))
            {
                if (myDebug) Log.Warning(DebugStr + " is not within allowed health range");
                return false;
            }

            if (CS.HasRelevantMassPerc && !CS.mass.Includes(t.GetStatValue(StatDefOf.Mass))) {
                if (myDebug) Log.Warning(DebugStr + "is not within allowed mass range");
                return false;
            }

            if (CS.HasCorpseRotStages)
            {
                CompRottable comp = t.TryGetComp<CompRottable>();
                if (comp == null)
                {
                    if (myDebug) Log.Warning(DebugStr + " has no compRottable");
                    return false;
                }
                if (!CS.rotStages.Contains(comp.Stage))
                {
                    if (myDebug) Log.Warning(DebugStr + " has no desired rotStage");
                    return false;
                }
            }

            //if (CS.requiresExclusiveReservation && map != null && map.reservationManager.IsReservedByAnyoneOf(t, pFaction))
            if (map != null && CS.HasReservationProcess  && CS.reservation.respectsThingReservation)
            {
                if (myDebug) Log.Warning(DebugStr + " checking reservations");

                //foreach (Thing reservedT in map.reservationManager.AllReservedThings()) Log.Warning(reservedT.ThingID);

                LocalTargetInfo LTI = new LocalTargetInfo(t);
                if (!map.reservationManager.ReservationsReadOnly
                    .Where(r=> r.Target == LTI)
                    .Where(r => r.Claimant != worker)
                    .Where(r => CS.reservation.respectsPawnKind ? r.Claimant.kindDef == worker.kindDef : false)
                    .Where(r => CS.reservation.respectsFaction ? r.Claimant.Faction == pFaction : false)
                    .EnumerableNullOrEmpty())
                {
                    if (myDebug) Log.Warning(DebugStr + "is reserved");
                    return false;
                }
                else
                {
                    if (myDebug) Log.Warning(DebugStr + " found no reservation for " + t);
                }
            }

            if (myDebug) Log.Warning(DebugStr + "is valid - OK");

            return true;
        }

        public static Corpse GetClosestCompatibleCorpse(this Pawn pawn, CorpseSpecification CS, bool myDebug = false)
        {
            string meFunc = myDebug ? "GetClosestCompatibleCorpse" : string.Empty;

            if (pawn.NegligiblePawn())
                return null;

            return (Corpse)GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Corpse),
                PathEndMode.ClosestTouch,
                TraverseParms.For(pawn),
                CS.maxDistance,
                delegate (Thing corpse) {
                    return CS.ValidateCorpse(corpse, pawn, myDebug, meFunc);
                    //return CS.ValidateCorpse(corpse, pawn);
                }
            );
        }


    }
}
