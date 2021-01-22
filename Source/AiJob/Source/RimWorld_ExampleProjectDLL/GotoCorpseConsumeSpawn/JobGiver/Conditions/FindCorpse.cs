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

        private static bool ValidateCorpse(Thing t, Map map, Faction pFaction, CorpseSpecification CS, bool myDebug = false)
        {
            if (t.NegligibleThing())
            {
                if (myDebug) Log.Warning("ValidateCorpse - negligible thing");
                return false;
            }

            if (!(t is Corpse))
            {
                if (myDebug) Log.Warning("ValidateCorpse - corpse is not corpse");
                return false;
            }

            if (!CS.categoryDef.NullOrEmpty() && !CS.categoryDef.Any(tc => t.def.IsWithinCategory(tc)))
            {
                if (myDebug) Log.Warning("ValidateCorpse - corpse is not within allowed categories");
                return false;
            }

            if (CS.HasRelevantHealthPerc && !CS.healthPerc.Includes(t.GetHitPointsPerc()))
            {
                if (myDebug) Log.Warning("ValidateCorpse - corpse is not within allowed health range");
                return false;
            }

            if(CS.HasRelevantMassPerc && !CS.massPerc.Includes(t.GetStatValue(StatDefOf.Mass))){
                if (myDebug) Log.Warning("ValidateCorpse - corpse is not within allowed mass range");
                return false;
            }

            if (CS.HasCorpseRotStages)
            {
                CompRottable comp = t.TryGetComp<CompRottable>();
                if(comp == null)
                {
                    if (myDebug) Log.Warning("ValidateCorpse - corpse has no compRottable");
                    return false;
                }
                if (!CS.rotStages.Contains(comp.Stage))
                {
                    if (myDebug) Log.Warning("ValidateCorpse - corpse has no desired rotStage");
                    return false;
                }
            }

            if (pFaction != null)
            {
                if (map.reservationManager.IsReservedByAnyoneOf(t, pFaction))
                {
                    if (myDebug) Log.Warning("ValidateCorpse - corpse is reserved by someone of the same faction");
                    return false;
                }
            }
                

            return true;
        }

        public static Corpse GetClosestCompatibleCorpse(this Pawn pawn, CorpseSpecification CS, bool myDebug = false)
        {
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
                    return ValidateCorpse(corpse, pawn.Map, pawn.Faction, CS, myDebug);
                }
            );
        }


    }
}
