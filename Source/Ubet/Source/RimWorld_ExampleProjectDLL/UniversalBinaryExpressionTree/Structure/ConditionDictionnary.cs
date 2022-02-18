using Verse;
using System.Collections.Generic;
using System;
using RimWorld;

namespace Ubet
{
    public static class ConditionDictionnary
    {
        readonly public static Dictionary<ConditionType, Func<Pawn, bool>> noArgconditions = new Dictionary<ConditionType, Func<Pawn, bool>>()
        {
            { ConditionType.isMale, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsMale) },
            { ConditionType.isFemale, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsFemale) } ,
            { ConditionType.isDrafted,  new Func<Pawn, bool> (NoArgConditionMethods.PawnIsDrafted) } ,
            { ConditionType.isUndrafted, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsUndrafted) } ,
        };

        readonly public static Dictionary<ConditionType, Func<Pawn, List<string>, bool>> StringArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, bool>>()
        {
            { ConditionType.isPerformingJob, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnIsPerformingJob) },
            { ConditionType.belongsToLifeStage,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnBelongsToLifeStage) },
            { ConditionType.isPawnKind,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnIsPawnKind) },
            { ConditionType.hasTrait, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasTrait) },
            { ConditionType.hasBackstory, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasBackstory) },
            { ConditionType.isOnMapWithWeather, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnMapWeather) },
            { ConditionType.isOnMapWithSeason, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnMapSeason) },
            { ConditionType.hasAliveRelation,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasAliveRelation) },
            { ConditionType.hasDeadRelation,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasDeadRelation) },
            { ConditionType.hasBodyPart, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasBodyPart) },
            { ConditionType.wearsApparelMadeOf,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnWearsApparelMadeOf) },
            { ConditionType.usesWeaponMadeOf,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnUsesWeaponMadeOf) },
        };

        readonly public static Dictionary<ConditionType, Func<Pawn, List<IntRange>, bool>> IntRangeArgconditions = new Dictionary<ConditionType, Func<Pawn, List<IntRange>, bool>>()
        {
            { ConditionType.IsWithinDayOfQuadrumRange,  new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfQuadrumWithin)  },
            { ConditionType.IsWithinDayOfSeasonRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfSeasonWithin)  },
            { ConditionType.IsWithinDayOfTwelfthRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfTwelfthWithin)  },
            { ConditionType.IsWithinDayOfYearRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfYearWithin)  },
            { ConditionType.IsWithinHourOfDayRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.HourOfDayWithin)  },
            { ConditionType.IsWithinTwelfthRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.TwelfthWithin)  },

        };
    }
 
}
