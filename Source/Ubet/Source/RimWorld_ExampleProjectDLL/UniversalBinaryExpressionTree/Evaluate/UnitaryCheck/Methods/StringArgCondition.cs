using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace Ubet
{
    public static class StringArgConditionMethods
    {

        //
        public static bool PawnBelongsToLifeStage(this Pawn p, List<string> parameters)
        {
            foreach (string s in parameters)
            {
                LifeStageDef LSD = DefDatabase<LifeStageDef>.GetNamed(s);
                if (p.ageTracker.CurLifeStage == LSD)
                    return true;
            }
            return false;
        }

        public static bool PawnIsPawnKind(this Pawn p, List<string> parameters)
        {
            foreach (string s in parameters)
            {
                PawnKindDef PKD = DefDatabase<PawnKindDef>.GetNamed(s);
                if (p.kindDef == PKD)
                    return true;
            }
            return false;
        }

        public static bool PawnIsFromRace(this Pawn p, List<string> parameters)
        {
            return parameters.Contains(p.def.defName);
        }

        public static bool PawnHasTrait(this Pawn p, List<string> parameters)
        {
            foreach (string s in parameters)
            {
                TraitDef TD = DefDatabase<TraitDef>.GetNamed(s);
                if (p.story.traits.allTraits.Any(t => t.def == TD))
                    return true;
            }
            return false;
        }

        public static bool PawnIsPerformingJob(this Pawn p, List<string> parameters)
        {
            if (p.CurJob == null)
            {
                if (parameters.NullOrEmpty())
                    return true;

                return false;
            }


            foreach (string s in parameters)
            {
                JobDef JD = DefDatabase<JobDef>.GetNamed(s);
                if (p.CurJobDef == JD)
                    return true;
            }
            return false;
        }

        public static bool PawnMapWeather(this Pawn p, List<string> parameters)
        {
            if (p.Map == null)
                return false;

            foreach (string s in parameters)
            {
                WeatherDef WD = DefDatabase<WeatherDef>.GetNamed(s);
                if (p.Map.weatherManager.curWeather == WD)
                    return true;
            }
            return false;
        }

        public static bool PawnMapSeason(this Pawn p, List<string> parameters)
        {
            if (p.Map == null)
                return false;
            Season mapSeason = GenLocalDate.Season(p.Map);

            foreach (string s in parameters)
            {
                switch (s)
                {
                    case "Spring":
                        if (mapSeason == Season.Spring)
                            return true;
                        continue;

                    case "Summer":
                        if (mapSeason == Season.Summer)
                            return true;
                        continue;

                    case "Fall":
                        if (mapSeason == Season.Fall)
                            return true;
                        continue;

                    case "Winter":
                        if (mapSeason == Season.Winter)
                            return true;
                        continue;

                    case "PermanentSummer":
                        if (mapSeason == Season.PermanentSummer)
                            return true;
                        continue;

                    case "PermanentWinter":
                        if (mapSeason == Season.PermanentWinter)
                            return true;
                        continue;

                    default: continue;
                }

            }
            return false;
        }

        public static bool PawnHasBackstory(this Pawn p, List<string> parameters)
        {
            if (p.story == null)
                return false;

            return p.story.AllBackstories.Any(b => parameters.Contains(b.identifier));
        }

        public static bool PawnHasRelation(this Pawn p, List<string> parameters, bool alive)
        {
            if (p.relations == null || !p.relations.RelatedToAnyoneOrAnyoneRelatedToMe)
                return false;

            IEnumerable<PawnRelationDef> PRD = DefDatabase<PawnRelationDef>.AllDefs.Where(x => parameters.Contains(x.defName));
            if (PRD.EnumerableNullOrEmpty())
                return false;

            if (p.relations.DirectRelations.Any(r => PRD.Contains(r.def) && (alive?(!r.otherPawn.Dead):r.otherPawn.Dead) ))
                return true;

            return false;
        }

        public static bool PawnHasDeadRelation(this Pawn p, List<string> parameters)
        {
            return p.PawnHasRelation(parameters, false);
        }

        public static bool PawnHasAliveRelation(this Pawn p, List<string> parameters)
        {
            return p.PawnHasRelation(parameters, true);
        }

        public static bool PawnHasBodyPart(this Pawn p, List<string> parameters)
        {
            IEnumerable<BodyPartRecord> bodyPartRecords = p.health.hediffSet.GetNotMissingParts().Where(bpr => parameters.Contains(bpr.untranslatedCustomLabel) || parameters.Contains(bpr.def.defName));

            return !bodyPartRecords.EnumerableNullOrEmpty();
        }

        public static bool ThingIsMadeOfStuff(this Thing t, List<string> parameters)
        {
            return t.def.MadeFromStuff && t.Stuff != null && parameters.Contains(t.Stuff.defName);
        }
        public static bool ThingHasIngredient(this Thing t, List<string> parameters)
        {
            return !t.def.costList.NullOrEmpty() && t.def.costList.Any(b => parameters.Contains(b.thingDef.defName));
        }

        public static bool PawnWearsApparelMadeOf(this Pawn p, List<string> parameters)
        {
            if (p.apparel == null || p.apparel.WornApparelCount == 0)
                return false;

            return p.apparel.WornApparel.Any(a =>
               a.ThingIsMadeOfStuff(parameters) ||
               a.ThingHasIngredient(parameters)
            );
        }

        public static bool PawnUsesWeaponMadeOf(this Pawn p, List<string> parameters)
        {
            if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
                return false;

            ThingWithComps w = p.equipment.Primary;

            return w.ThingIsMadeOfStuff(parameters) || w.ThingHasIngredient(parameters);
        }

        public static bool PawnUsesWeapon(this Pawn p, List<string> parameters)
        {
            if (p.equipment == null || p.equipment.Primary.DestroyedOrNull())
                return false;

            ThingWithComps w = p.equipment.Primary;

            return parameters.Contains( w.def.defName );
        }

        public static bool PawnUsesApparel(this Pawn p, List<string> parameters)
        {
            if (p.apparel == null || p.apparel.WornApparelCount == 0)
                return false;

            return p.apparel.WornApparel.Any(a =>
               parameters.Contains(a.def.defName)
            );
        }

        public static bool PawnIsInSpecificMentalState(this Pawn p, List<string> MentalStateDefName)
        {
            if (p.MentalState == null)
                return false;

            return MentalStateDefName.Contains(p.MentalStateDef.defName);
        }

        public static bool PawnHasHediff(this Pawn p, List<string> Hediff)
        {
            if (p.health.hediffSet.hediffs.NullOrEmpty())
                return false;

            return p.health.hediffSet.hediffs.Any(h =>
               Hediff.Contains(h.def.defName)
            );
        }


    }
}
