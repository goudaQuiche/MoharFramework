using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace Ubet
{
    public static class NoArgConditionMethods_old
    {
        
        //
        public static bool ThingIsPawn(this Thing t)
        {
            return t is Pawn;
        }

        public static bool PawnIsHuman(this Pawn p)
        {
            return p.def == ThingDefOf.Human;
        }

        public static bool PawnIsMale(this Pawn p)
        {
            return p.gender == Gender.Male;
        }

        public static bool PawnIsFemale(this Pawn p)
        {
            return p.gender == Gender.Female;
        }

        public static bool PawnIsDrafted(this Pawn p)
        {
            return p.Drafted;
        }

        public static bool PawnIsUndrafted(this Pawn p)
        {
            return !p.Drafted;
        }
    }
}
