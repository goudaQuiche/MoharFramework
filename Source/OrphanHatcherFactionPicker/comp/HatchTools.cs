using RimWorld.Planet;
using Verse;
using RimWorld;

namespace OHFP
{
    public static class HatchTools
    {

        public static bool MyTrySpawnHatchedOrBornPawn(this Thing motherOrEgg, Pawn pawn)
        {
            if (motherOrEgg.SpawnedOrAnyParentSpawned)
            {
                return GenSpawn.Spawn(pawn, motherOrEgg.PositionHeld, motherOrEgg.MapHeld) != null;
            }
            Pawn pawn2 = motherOrEgg as Pawn;
            if (pawn2 != null)
            {
                if (pawn2.IsCaravanMember())
                {
                    pawn2.GetCaravan().AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny: true);
                    Find.WorldPawns.PassToWorld(pawn);
                    return true;
                }
                if (pawn2.IsWorldPawn())
                {
                    Find.WorldPawns.PassToWorld(pawn);
                    return true;
                }
            }
            else if (motherOrEgg.ParentHolder != null)
            {
                Pawn_InventoryTracker pawn_InventoryTracker = motherOrEgg.ParentHolder as Pawn_InventoryTracker;
                if (pawn_InventoryTracker != null)
                {
                    if (pawn_InventoryTracker.pawn.IsCaravanMember())
                    {
                        pawn_InventoryTracker.pawn.GetCaravan().AddPawn(pawn, addCarriedPawnToWorldPawnsIfAny: true);
                        Find.WorldPawns.PassToWorld(pawn);
                        return true;
                    }
                    if (pawn_InventoryTracker.pawn.IsWorldPawn())
                    {
                        Find.WorldPawns.PassToWorld(pawn);
                        return true;
                    }
                }
            }
            return false;
        }

        //Faction own, Faction forced, int duration
        public static bool MakeManhunter(this Pawn p, bool MyDebug = false)
        {
            if (p.NegligiblePawn())
                return false;

            MentalStateDef manhunterState = null;
            manhunterState = MentalStateDefOf.Manhunter;
            Tools.Warn(p.LabelShort + " trying to go " + manhunterState.defName, MyDebug);
            //mindTarget.mindState.mentalStateHandler.TryStartMentalState(chosenState, null, true, false, null);
            string reason = "because ";

            if (p.mindState == null || p.mindState.mentalStateHandler == null)
            {
                Tools.Warn(p.LabelShort + " null mindstate", MyDebug);
                return false;
            }

            Tools.Warn(p.LabelShort + " got applied " + manhunterState.defName, MyDebug);
            p.mindState.mentalStateHandler.TryStartMentalState(manhunterState, reason, true, false, null);

            return true;
        }

        public static void InheritParentSettings(this Pawn p, Pawn hatcheeParent, Faction hatcheeFaction)
        {
            if (p.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
            {
                p.playerSettings.AreaRestriction = hatcheeParent.playerSettings.AreaRestriction;
            }
        }

        public static void AddParentRelations(this Pawn p, Pawn hatcheeParent)
        {
            if (!p.RaceProps.IsMechanoid)
            {
                p.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
            }
        }

        public static void AddOtherParentRelations(this Pawn p, Pawn hatcheeParent, Pawn otherParent)
        {
            if (otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && !p.RaceProps.IsMechanoid)
            {
                p.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
            }
        }
    }
}
