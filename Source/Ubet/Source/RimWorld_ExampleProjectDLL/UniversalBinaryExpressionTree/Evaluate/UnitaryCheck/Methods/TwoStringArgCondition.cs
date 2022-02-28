using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class TwoStringArgConditionMethods
    {
        public static bool PawnUsesSpecificWeaponMadeOf(Pawn p, List<string> WeaponDef, List<string> Stuff)
        {
            if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
                return false;

            ThingWithComps w = p.equipment.Primary;

            return WeaponDef.Contains( w.def.defName ) && p.ThingIsMadeOfStuff(Stuff);
        }

        public static bool PawnWearsSpecificApparelMadeOf(Pawn p, List<string> ApparelDef, List<string> Stuff)
        {
            if (p.apparel == null || p.apparel.WornApparelCount == 0)
                return false;

            return p.apparel.WornApparel.Any(a =>
               ApparelDef.Contains(a.def.defName) &&
               a.ThingIsMadeOfStuff(Stuff)
            );
        }

        public static bool PawnDoingBill(Pawn p, List<string> BuildingDef, List<string> RecipeDef)
        {
            // no job
            if (p.CurJob == null)
                return false;

            // not good job
            if (p.CurJobDef != JobDefOf.DoBill)
                return false;

            // targetA is billGiver
            if (!(p.CurJob.targetA.Thing is Thing t))
                return false;

            // found building
            if (!((Building)t is Building b))
                return false;

            // correct building depending on def
            if (!BuildingDef.Contains(b.def.defName))
                return false;

            // pawn in position
            if (p.Position != b.InteractionCell)
                return false;

            // performing right recipe
            if ( !RecipeDef.NullOrEmpty() && (!RecipeDef.Contains(p.CurJob.RecipeDef?.defName)))
                return false;

            return true;

        }
    }
}
