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

        //Gender
        [Description("is Male")]
        isMale,
        [Description("is Female")]
        isFemale,

        //Activity
        [Description("is Drafted")]
        isDrafted,
        [Description("is Undrafted")]
        isUndrafted,
        [Description("is performing job")]
        isPerformingJob,

        //Nature
        [Description("belongs to life stage")]
        belongsToLifeStage,
        [Description("belongs to pawn kind")]
        isPawnKind,
        [Description("has trait")]
        hasTrait,
        [Description("has backstory")]
        hasBackstory,

        //Environment - map
        [Description("is on a map with weather")]
        isOnMapWithWeather,
        [Description("is on map with season")]
        isOnMapWithSeason,

        //Environment - time
        [Description("is within day of year range")]
        IsWithinDayOfYearRange,
        [Description("is within day of season range")]
        IsWithinDayOfSeasonRange,
        [Description("is within day of quadrum range")]
        IsWithinDayOfQuadrumRange,
        [Description("is within day of twelfth range")]
        IsWithinDayOfTwelfthRange,
        [Description("is within hour of day range")]
        IsWithinHourOfDayRange,
        [Description("is within twelfth range")]
        IsWithinTwelfthRange,




        //Relations
        [Description("has alive relation")]
        hasAliveRelation,
        [Description("has dead relation")]
        hasDeadRelation,

        //Body
        [Description("has body part")]
        hasBodyPart,

        //Equipment
        [Description("wears apparel made of")]
        wearsApparelMadeOf,
        [Description("uses weapon made of")]
        usesWeaponMadeOf,

        [Description("empty condition")]
        empty
    }
}
