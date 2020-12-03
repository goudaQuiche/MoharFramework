using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class ConditionBuilder
    {
        public static void CopyHediffCondition(HediffCondition source, HediffCondition dest, bool debug = false)
        {
            string debugStr = debug ? "CopyHediffCondition - " : "";

            if (source.HasBodypartCondition)
            {
                Tools.Warn(debugStr + "found HasBodypartCondition, copying", debug);
                if (source.bodyPart.HasDef)
                    dest.bodyPart.bodyPartDef = source.bodyPart.bodyPartDef.ListFullCopy();
                if (source.bodyPart.HasLabel)
                    dest.bodyPart.bodyPartLabel = source.bodyPart.bodyPartLabel.ListFullCopy();
                if (source.bodyPart.HasTag)
                    dest.bodyPart.bodyPartTag = source.bodyPart.bodyPartTag.ListFullCopy();
            }

            if (source.HasPawnCondition)
            {
                Tools.Warn(debugStr + "found HasPawnCondition, copying", debug);
                if (source.pawn.HasRace)
                    dest.pawn.race = source.pawn.race.ListFullCopy();
                if (source.pawn.HasGender)
                    dest.pawn.gender = source.pawn.gender.ListFullCopy();

                dest.pawn.ageRange = source.pawn.ageRange;
            }

        }

        public static HediffCondition GetDefaultPlusSpecificHediffCondition(HediffCondition defaultHC, HediffCondition specificHC, bool debug = false)
        {
            string debugStr = debug ? "GetDefaultPlusSpecificHediffCondition - " : "";
            Tools.Warn(debugStr + "allocating answerHC", debug);
            HediffCondition answerHc = new HediffCondition
            {
                bodyPart = new BodyPartCondition
                {
                    bodyPartDef = new List<BodyPartDef>(),
                    bodyPartLabel = new List<string>(),
                    bodyPartTag = new List<BodyPartTagDef>()
                },
                pawn = new PawnCondition
                {
                    race = new List<ThingDef>(),
                    gender = new List<Gender>()
                }
            };

            if (defaultHC != null)
            {
                Tools.Warn(debugStr + "found defaultHC, copying", debug);
                CopyHediffCondition(defaultHC, answerHc, debug);
            }


            if (specificHC != null)
            {
                Tools.Warn(debugStr + "found specificHC, copying", debug);
                CopyHediffCondition(specificHC, answerHc, debug);
            }


            return answerHc;
        }

    }
}
