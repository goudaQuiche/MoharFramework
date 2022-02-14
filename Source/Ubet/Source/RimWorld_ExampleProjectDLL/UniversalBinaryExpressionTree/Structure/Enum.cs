using Verse;

namespace Ubet
{
    public enum ConditionType
    {
        isNegligible,

        [Description("is Pawn")]
        isPawn,
        [Description("is Building")]
        isBuilding,

        [Description("is Human")]
        isHuman,
        [Description("is Male")]
        isMale,
        [Description("is Female")]
        isFemale,

        [Description("is Drafted")]
        isDrafted,
        [Description("is Undrafted")]
        isUndrafted,

        [Description("belongs to life stage")]
        belongsToLifeStage,
        [Description("belongs to pawn kind")]
        isPawnKind,
        [Description("has trait")]
        hasTrait,
        [Description("is performing job")]
        isPerformingJob,

        [Description("is on a map with weather")]
        isOnMapWithWeather,
        [Description("is on map with season")]
        isOnMapWithSeason,

        [Description("empty condition")]
        empty
    }

    public enum Operand
    {
        [Description("AND operand")]
        and,
        [Description("OR operand")]
        or,
        [Description("NOT operand")]
        not,

        [Description("empty operand")]
        empty
    }
}
