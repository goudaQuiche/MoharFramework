using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace Ubet
{
    public static class NoArgConditionMethods
    {
        
        public static bool PawnIsHuman(Pawn p)
        {
            return p.def == ThingDefOf.Human;
        }

        public static bool PawnIsMale(Pawn p)
        //public bool PawnIsMale(Pawn p)
        {
            return p.gender == Gender.Male;
        }

        public static bool PawnIsFemale(Pawn p)
        {
            return p.gender == Gender.Female;
        }

        public static bool PawnIsDrafted(Pawn p)
        {
            return p.Drafted;
        }

        public static bool PawnIsUndrafted(Pawn p)
        {
            return !p.Drafted;
        }
    }
}
