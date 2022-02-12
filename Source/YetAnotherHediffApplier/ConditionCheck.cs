using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace YAHA
{
    public static class ConditionCheck
    {
        public static bool CheckAllConditions(this Pawn p, HediffAssociation ha)
        {
            if (!p.OkPawn())
                return false;

            if (ha.condition.HasNatureCondition)
            {
                if (ha.condition.nature.HasGender && !p.CheckGenderCondition(ha.condition.nature.gender))
                    return false;
                if (ha.condition.nature.HasLifeStage && !p.CheckLifeStageCondition(ha.condition.nature.lifeStage))
                    return false;
                if (ha.condition.nature.HasPawnkind && !p.CheckPawnKindCondition(ha.condition.nature.pawnKind))
                    return false;
                if (ha.condition.nature.HasTrait && !p.CheckTraitCondition(ha.condition.nature.trait))
                    return false;
            }

            if (ha.condition.HasJobCondition)
            {
                if(ha.condition.job.HasJob && !p.CheckCurrentJob(ha.condition.job))
                    return false;
            }

            if (ha.condition.HasEnvironmentCondition)
            {
                if (ha.condition.environment.HasSeason && !p.CheckSeason(ha.condition.environment.season))
                    return false;
                if (ha.condition.environment.HasWeather && !p.CheckWeather(ha.condition.environment.weather))
                    return false;
            }

            return true;
        }

        public static bool CheckGenderCondition(this Pawn p, GenderPossibility gp)
        {
            if (gp == GenderPossibility.Either)
                return true;

            if (gp == GenderPossibility.Male && p.gender == Gender.Male)
                return true;

            if (gp == GenderPossibility.Female && p.gender == Gender.Female)
                return true;

            return false;
        }

        public static bool CheckLifeStageCondition(this Pawn p, List<LifeStageDef> lsd)
        {
            LifeStageDef lifeStageDef = p.ageTracker.CurLifeStage;

            if (lsd.Contains(lifeStageDef))
                return true;

            return false;
        }

        public static bool CheckPawnKindCondition(this Pawn p, List<PawnKindDef> pkd)
        {
            PawnKindDef PKDef = p.kindDef;

            if (pkd.Contains(PKDef))
                return true;

            return false;
        }

        public static bool CheckTraitCondition(this Pawn p, List<TraitDef> td)
        {

            foreach(TraitDef ctd in td)
            {
                if (p.story.traits.allTraits.Any(t => t.def == ctd))
                    return true;
            }

            return false;
        }

        public static bool CheckCurrentJob(this Pawn p, JobCondition jc)
        {
            if (p.CurJob == null)
                return false;

            if (jc.currentJob.Contains(p.CurJobDef))
                return true;

            return false;
        }

        public static bool CheckSeason(this Pawn p, List<Season> seasons)
        {
            if (p.Map == null)
                return false;

            if (seasons.Contains(GenLocalDate.Season(p.Map)))
                return true;

            return false;
        }

        public static bool CheckWeather(this Pawn p, List<WeatherDef> weathers)
        {
            if (p.Map == null)
                return false;

            if (weathers.Contains(p.Map.weatherManager.curWeather))
                return true;

            return false;
        }
    }
}
