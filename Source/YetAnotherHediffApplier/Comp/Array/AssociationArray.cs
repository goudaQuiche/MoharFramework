using System.Collections.Generic;
using Verse;
using RimWorld;

namespace YAHA
{
    public class HediffAssociation
    {
        public List<HediffDef> hediffPool;
        public List<RandomHediffPool> randomHediffPool;

        public List<string> bodyPart;
        public ApplyCondition condition;
        public ApplySpecifics specifics;

        public bool HasHediffPool => !hediffPool.NullOrEmpty();
        public bool HasRandomHediffPool => !randomHediffPool.NullOrEmpty();

        public bool HasBodyPartToApplyHediff => !bodyPart.NullOrEmpty();
    }

    public class ApplySpecifics
    {
        public FloatRange severity = new FloatRange(1, 1);

        // -1 unlimited
        public int applyNumLimit = 1;
        public bool removeIfFalse = false;
        //public bool removeOthersIfFalse = false;
        //public bool removeIfOtherTrue = false;

        public bool HasSeverity => !(severity.min != 1 && severity.max == 1);
        public bool HasLimit => applyNumLimit > 0;
    }

    public class ApplyCondition
    {
        public FloatRange randomChance = new FloatRange(1, 1);

        public PawnNatureCondition nature;
        public JobCondition job;
        public EnvironmentCondition environment;

        public bool HasRandomChance => !(randomChance.min != 1 && randomChance.max == 1);

        public bool HasNatureCondition => nature != null;
        public bool HasJobCondition => job != null;
        public bool HasEnvironmentCondition => environment != null;

    }

    public class EnvironmentCondition
    {
        public List<Season> season;
        public List<WeatherDef> weather;

        public bool HasSeason => !season.NullOrEmpty();
        public bool HasWeather => !weather.NullOrEmpty();
    }

    public class JobCondition
    {
        public List<JobDef> currentJob;
        //public List<JobDef> pastJob;

        public bool HasJob => !currentJob.NullOrEmpty();
        //public bool HasEndJob => !pastJob.EnumerableNullOrEmpty();
    }

    public class PawnNatureCondition
    {
        public GenderPossibility gender = GenderPossibility.Either;
        public List<LifeStageDef> lifeStage;
        public List<PawnKindDef> pawnKind;
        public List<TraitDef> trait;

        public bool HasGender => gender != GenderPossibility.Either;
        public bool HasPawnkind => !pawnKind.NullOrEmpty();
        public bool HasLifeStage => !lifeStage.NullOrEmpty();
        public bool HasTrait => !trait.NullOrEmpty();
    }
    public class RandomHediffPool
    {
        public List<RandomHediff> randomHediff;
    }
    public class RandomHediff
    {
        public HediffDef hediff;
        public float weight;
    }

}
