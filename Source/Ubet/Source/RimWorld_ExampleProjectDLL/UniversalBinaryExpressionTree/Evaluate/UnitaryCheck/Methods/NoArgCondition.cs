using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace Ubet
{
    public static class NoArgConditionMethods
    {

        //Nature
        public static bool PawnIsHuman(Pawn p)
        {
            return p.def == ThingDefOf.Human;
        }

        //Gender
        public static bool PawnIsMale(Pawn p)
        {
            return p.gender == Gender.Male;
        }

        public static bool PawnIsFemale(Pawn p)
        {
            return p.gender == Gender.Female;
        }


        // Draft
        public static bool PawnIsDrafted(Pawn p)
        {
            return p.Drafted;
        }

        public static bool PawnIsUndrafted(Pawn p)
        {
            return !p.Drafted;
        }

        // Bed
        public static bool PawnIsInBed(Pawn p)
        {
            return p.InBed();
        }
        public static bool PawnIsInLoveBed(Pawn p)
        {
            Building_Bed Bed = p.CurrentBed();
            if (Bed == null)
                return false;

            for (int i = 0; i < Bed.OwnersForReading.Count; i++)
            {
                if (LovePartnerRelationUtility.LovePartnerRelationExists(p, Bed.OwnersForReading[i]))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool PawnIsInMedicalBed(Pawn p)
        {
            Building_Bed Bed = p.CurrentBed();
            if (Bed == null)
                return false;

            return Bed.Medical;
        }

        //Faction
        public static bool PawnIsFromPlayerFaction(Pawn p)
        {
            if (p.Faction == null)
                return false;

            return p.Faction.IsPlayer;
        }

        public static bool PawnIsInMentalState(this Pawn p)
        {
            return p.MentalState != null;
        }
    }
}
