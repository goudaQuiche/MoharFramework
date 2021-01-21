using Verse;
using Verse.AI;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace MoharAiJob
{
    public static class FindCorpse
    {
        private static bool ValidateCorpse(Thing t, Map map, Faction pFaction, List<ThingCategoryDef> allowed, bool myDebug = false)
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

            if (!allowed.NullOrEmpty() && !allowed.Any(tc => t.def.IsWithinCategory(tc)))
            {
                if (myDebug) Log.Warning("ValidateCorpse - corpse is not within allowed categories");
                return false;
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

        public static Corpse GetClosestCompatibleCorpse(this Pawn pawn, List<ThingCategoryDef> allowed, float maxDistance, bool myDebug = false)
        {
            if (pawn.NegligiblePawn())
                return null;

            return (Corpse)GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Corpse),
                PathEndMode.ClosestTouch,
                TraverseParms.For(pawn),
                maxDistance,
                delegate (Thing corpse) {
                    return ValidateCorpse(corpse, pawn.Map, pawn.Faction, allowed, myDebug);
                }
            );
        }


    }
}
