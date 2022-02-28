using Verse;
using System.Collections.Generic;
using System;
using RimWorld;

namespace Ubet
{
    public static class ConditionDictionnary
    {
        // noArgconditions
        readonly public static Dictionary<ConditionType, Func<Pawn, bool>> noArgconditions = new Dictionary<ConditionType, Func<Pawn, bool>>()
        {
            //{ ConditionType.isPawn, new Func<Pawn, bool> (NoArgConditionMethods.) },

            { ConditionType.isMale, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsMale) },
            { ConditionType.isFemale, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsFemale) } ,
            { ConditionType.isHuman, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsHuman) } ,
            { ConditionType.belongsToPlayerFaction, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsFromPlayerFaction) } ,

            { ConditionType.isDrafted,  new Func<Pawn, bool> (NoArgConditionMethods.PawnIsDrafted) } ,
            { ConditionType.isUndrafted, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsUndrafted) } ,
            { ConditionType.isInMentalState, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsInMentalState) } ,

            { ConditionType.lyingInBed, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsInBed) } ,
            { ConditionType.lyingInLoveBed, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsInLoveBed) } ,
            { ConditionType.lyingInMedicalBed, new Func<Pawn, bool> (NoArgConditionMethods.PawnIsInMedicalBed) } ,

            
        };

        // StringArgconditions
        readonly public static Dictionary<ConditionType, Func<Pawn, List<string>, bool>> StringArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, bool>>()
        {
            { ConditionType.isPerformingJob, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnIsPerformingJob) },
            { ConditionType.isInSpecificMentalState, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnIsInSpecificMentalState) },

            { ConditionType.belongsToLifeStage,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnBelongsToLifeStage) },
            { ConditionType.isPawnKind,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnIsPawnKind) },
            { ConditionType.belongsToRace,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnIsFromRace) },
            { ConditionType.hasTrait, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasTrait) },
            { ConditionType.hasBackstory, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasBackstory) },
            { ConditionType.hasBodyPart, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasBodyPart) },

            { ConditionType.isOnMapWithWeather, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnMapWeather) },
            { ConditionType.isOnMapWithSeason, new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnMapSeason) },

            { ConditionType.hasAliveRelation,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasAliveRelation) },
            { ConditionType.hasDeadRelation,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnHasDeadRelation) },

            { ConditionType.wearsApparelMadeOf,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnWearsApparelMadeOf) },
            { ConditionType.usesWeaponMadeOf,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnUsesWeaponMadeOf) },
            { ConditionType.wearsApparel,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnUsesApparel) },
            { ConditionType.usesWeapon,  new Func<Pawn, List<string>, bool> (StringArgConditionMethods.PawnUsesWeapon) },
        };

        // TwoStringArgconditions
        readonly public static Dictionary<ConditionType, Func<Pawn, List<string>, List<string>, bool>> TwoStringArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, List<string>, bool>>()
        {
            { ConditionType.wearsSpecificApparelMadeOf, new Func<Pawn, List<string>, List<string>, bool> (TwoStringArgConditionMethods.PawnWearsSpecificApparelMadeOf) },
            { ConditionType.usesSpecificWeaponMadeOf, new Func<Pawn, List<string>, List<string>, bool> (TwoStringArgConditionMethods.PawnUsesSpecificWeaponMadeOf) },
            { ConditionType.isDoingBill, new Func<Pawn, List<string>, List<string>, bool> (TwoStringArgConditionMethods.PawnDoingBill) },
        };

        // StringFloatArgconditions
        readonly public static Dictionary<ConditionType, Func<Pawn, List<string>, List<FloatRange>, bool>> StringFloatArgconditions = new Dictionary<ConditionType, Func<Pawn, List<string>, List<FloatRange>, bool>>()
        {
            { ConditionType.hasNeedInRange, new Func<Pawn, List<string>, List<FloatRange>, bool> (StringFloatArgConditionMethods.PawnHasNeedInRange) },
        };

        // FloatArgconditions
        readonly public static Dictionary<ConditionType, Func< FloatRange, bool>> FloatRangeArgconditions = new Dictionary<ConditionType, Func< FloatRange, bool>>()
        {
            { ConditionType.floatRangeRandom, new Func< FloatRange, bool> (FloatRangeArgConditionMethods.RandomRoll) },
        };

        // CurveArgconditions
        readonly public static Dictionary<ConditionType, Func<Pawn, SimpleCurve, bool>> CurveArgconditions = new Dictionary< ConditionType, Func<Pawn, SimpleCurve, bool>>()
        {
            { ConditionType.ageCurveRandom, new Func<Pawn, SimpleCurve, bool> (CurveArgConditionMethods.PawnAgeCurveRandomRoll) },
            { ConditionType.healthCurveRandom, new Func<Pawn, SimpleCurve, bool> (CurveArgConditionMethods.PawnHealthCurveRandomRoll) },
        };

        // IntRangeArgconditions
        readonly public static Dictionary<ConditionType, Func<Pawn, List<IntRange>, bool>> IntRangeListArgconditions = new Dictionary<ConditionType, Func<Pawn, List<IntRange>, bool>>()
        {
            { ConditionType.isWithinDayOfQuadrumRange,  new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfQuadrumWithin)  },
            { ConditionType.isWithinDayOfSeasonRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfSeasonWithin)  },
            { ConditionType.isWithinDayOfTwelfthRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfTwelfthWithin)  },
            { ConditionType.isWithinDayOfYearRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.DayOfYearWithin)  },
            { ConditionType.isWithinHourOfDayRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.HourOfDayWithin)  },
            { ConditionType.isWithinTwelfthRange, new Func<Pawn, List<IntRange>, bool> (IntRangeArgConditionMethods.TwelfthWithin)  },
        };
    }
 
}
