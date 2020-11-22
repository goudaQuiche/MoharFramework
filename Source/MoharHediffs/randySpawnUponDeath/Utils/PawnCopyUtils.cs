using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class PawnCopyUtils
    {
        public static void SetAge(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            ThingSettings TS = comp.ChosenItem;
            if (TS.IsCopier && TS.copyParent.age)
            {
                LifeStageDef LSDef = comp.Pawn.ageTracker.CurLifeStage;
                LifeStageAge LSAge = comp.Pawn.def.race.lifeStageAges.Where(LS => LS.def == LSDef).FirstOrFallback();
                if(LSAge == null)
                    return;

                newPawn.ageTracker.AgeBiologicalTicks = (long)(LSAge.minAge * MyDefs.OneYearTicks);
                newPawn.ageTracker.AgeChronologicalTicks = Math.Max(comp.Pawn.ageTracker.AgeBiologicalTicks, comp.Pawn.ageTracker.AgeChronologicalTicks);
                return;
            }
            /*
            else if (TS.HasAgeRange)
            {
                if(TS.HasBiologicalAgeRange)
                    newPawn.ageTracker.AgeBiologicalTicks = MyDefs.OneYearTicks * TS.biologicalAgeRange.RandomInRange;
                if (TS.HasChronologicalAgeRange)
                    newPawn.ageTracker.AgeBiologicalTicks = MyDefs.OneYearTicks * TS.chronologicalAgeRange.RandomInRange;
            }
            */
            newPawn.ageTracker.AgeBiologicalTicks = MyDefs.OneYearTicks * TS.biologicalAgeRange.RandomInRange;
            newPawn.ageTracker.AgeChronologicalTicks = MyDefs.OneYearTicks * TS.chronologicalAgeRange.RandomInRange + newPawn.ageTracker.AgeBiologicalTicks;
            
        }

        public static void SetName(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParent.name)
                newPawn.Name = comp.Pawn.Name;
        }

        public static void SetGender(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParent.gender)
                newPawn.gender = comp.Pawn.gender;
        }

        public static void SetMelanin(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParent.melanin)
                newPawn.story.melanin = comp.Pawn.story.melanin;
        }
        public static void SetAlienSkinColor(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (!(comp.Pawn.IsAlien() || newPawn.IsAlien()))
                return;

            AlienPartGenerator.AlienComp templateAlien = comp.Pawn.TryGetComp<AlienPartGenerator.AlienComp>();
            AlienPartGenerator.AlienComp copyAlien = newPawn?.TryGetComp<AlienPartGenerator.AlienComp>();

            if (templateAlien == null || copyAlien == null)
                return;

            Color skinColor1 = templateAlien.GetChannel("skin").first;
            Color skinColor2 = templateAlien.GetChannel("skin").second;

            copyAlien.GetChannel("skin").first = skinColor1;
            copyAlien.GetChannel("skin").second = skinColor2;
        }
        public static void SetAlienCrownType(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (!(comp.Pawn.IsAlien() && newPawn.IsAlien()))
                return;

            AlienPartGenerator.AlienComp templateAlien = comp.Pawn.TryGetComp<AlienPartGenerator.AlienComp>();
            AlienPartGenerator.AlienComp copyAlien = newPawn?.TryGetComp<AlienPartGenerator.AlienComp>();

            if (templateAlien == null || copyAlien == null)
                return;

            copyAlien.crownType = templateAlien.crownType;
        }

        public static void SetBodyType(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParent.bodyType)
                newPawn.story.bodyType = comp.Pawn.story.bodyType;
        }
        public static void SetCrownType(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (!comp.ChosenItem.copyParent.crownType)
                return;

            if (comp.Pawn.IsHuman())
                newPawn.story.crownType = comp.Pawn.story.crownType;
            else if (comp.Pawn.IsAlien())
                comp.SetAlienCrownType(newPawn);

        }
        public static void SetHair(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParent.hair)
                newPawn.story.hairDef = comp.Pawn.story.hairDef;
        }
        public static void SetHairColor(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.copyParent.hairColor)
                newPawn.story.hairColor = comp.Pawn.story.hairColor;
        }
        public static void SetHediff(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (!comp.ChosenItem.copyParent.hediff)
                return;

            newPawn.health.hediffSet.hediffs = new List<Hediff>();
            List<Hediff> newHediffSet =
                comp.ChosenItem.copyParent.HasHediffExclusion ?
                comp.Pawn.health.hediffSet.hediffs.ListFullCopy().Where(
                    h => !comp.ChosenItem.copyParent.excludeHediff.Contains(h.def) &&
                    (comp.ChosenItem.copyParent.excludeTendableHediffs ? !h.def.tendable : true) &&
                    (comp.ChosenItem.copyParent.excludePermanentHediffs ? h.TryGetComp<HediffComp_GetsPermanent>() == null : true)
                ).ToList() :
                comp.Pawn.health.hediffSet.hediffs.ListFullCopy();


            newPawn.health.hediffSet.hediffs = newHediffSet;
        }

        public static void SetSkills(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, bool debug = false)
        {
            if (!comp.ChosenItem.copyParent.skills)
                return;

            string myDebugSr = newPawn.LabelShort + " - SetSkills - ";

            for (int i = 0; i < newPawn.skills.skills.Count; i++)
            {
                float decayRatio = comp.ChosenItem.copyParent.skillDecay.RandomInRange;
                int wantedLevel = (int)(comp.Pawn.skills.skills[i].levelInt * decayRatio);

                Tools.Warn(
                    myDebugSr + " browsing " + comp.Pawn.skills.skills[i].def.defName +
                    " ori: " + comp.Pawn.skills.skills[i].levelInt +
                    " new: " + newPawn.skills.skills[i].levelInt +
                    " decayRatio: " + decayRatio +
                    " wantedSkill: " + wantedLevel
                    //" rememberSkillTracker: " + comp.rememberSkillTracker.skills[i].levelInt
                    , debug
                );

                if (wantedLevel > newPawn.skills.skills[i].levelInt)
                {
                    Tools.Warn(myDebugSr + "Calling gainskill", debug);
                    comp.GainSkill(newPawn, wantedLevel, i, debug);
                }
                else if (wantedLevel < newPawn.skills.skills[i].levelInt)
                {
                    Tools.Warn(myDebugSr + "Calling loseskill", debug);
                    comp.LoseSkill(newPawn, wantedLevel, i, debug);
                }
                    

                Tools.Warn(
                    myDebugSr + " copied skill [" + i + "]:" + comp.Pawn.skills.skills[i].def.defName +

                    " new: " + newPawn.skills.skills[i].levelInt
                    , debug
                );



                //newSkill.xpSinceLastLevel = originalSkill.xpSinceLastLevel;
            }
        }

        public static void GainSkill(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, int wantedLevel, int index, bool debug = false)
        {
            string myDebugSr = newPawn.LabelShort + " - GainSkill - ";
            Tools.Warn(myDebugSr + "Entering", debug);

            int loopBreaker = 20;
            while (wantedLevel > newPawn.skills.skills[index].levelInt && loopBreaker > 0)
            {
                float xpInjected = newPawn.skills.skills[index].XpRequiredForLevelUp;

                Tools.Warn(
                    myDebugSr +
                    " loop: " + loopBreaker +
                    " xpInjected: " + xpInjected +
                    " ori: " + comp.Pawn.skills.skills[index].levelInt +
                    " new: " + newPawn.skills.skills[index].levelInt
                    , debug
                );

                newPawn.skills.skills[index].Learn(xpInjected, true);
                loopBreaker--;
            }
        }

        public static void LoseSkill(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, int wantedLevel, int index, bool debug = false)
        {
            string myDebugSr = newPawn.LabelShort + " - LoseSkill - ";
            Tools.Warn(myDebugSr + "Entering", debug);

            int loopBreaker = 20;
            while (wantedLevel < newPawn.skills.skills[index].levelInt && loopBreaker > 0)
            {
                float xpInjected = -(newPawn.skills.skills[index].levelInt*1000);

                Tools.Warn(
                    myDebugSr +
                    " loop: " + loopBreaker +
                    " xpInjected: " + xpInjected +
                    " ori: " + comp.Pawn.skills.skills[index].levelInt +
                    " new: " + newPawn.skills.skills[index].levelInt
                    , debug
                );

                newPawn.skills.skills[index].Learn(xpInjected, true);
                loopBreaker--;
            }
        }

        public static void SetPassions(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, bool debug = false)
        {
            if (!comp.ChosenItem.copyParent.passions)
                return;

            for (int i = 0; i < newPawn.skills.skills.Count; i++)
                newPawn.skills.skills[i].passion = comp.Pawn.skills.skills[i].passion;
        }

        public static void InitRememberBackstories(out Backstory childBS, out Backstory adultBS)
        {
            childBS = adultBS = null;
        }

        public static void ResetBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            newPawn.story.childhood = null;
            newPawn.story.adulthood = null;
        }

        public static void RememberBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, out Backstory childBS, out Backstory adultBS)
        {
            childBS = newPawn.story.childhood;
            adultBS = newPawn.story.adulthood;
        }

        public static void ReinjectBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, Backstory childBS, Backstory adultBS)
        {
            newPawn.story.childhood = childBS;
            newPawn.story.adulthood = adultBS;
        }

        public static void SetBackstories(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if(comp.ChosenItem.copyParent.childBS)
                newPawn.story.childhood = comp.Pawn.story.childhood;
            if (comp.ChosenItem.copyParent.adultBS)
                newPawn.story.adulthood = comp.Pawn.story.adulthood;
        }

        public static void SetTraits(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (!comp.ChosenItem.copyParent.traits)
                return;

            for (int i = newPawn.story.traits.allTraits.Count - 1; i >= 0; i--)
            {
                newPawn.story.traits.allTraits.RemoveAt(i);
            }

            newPawn.story.traits.allTraits = comp.Pawn.story.traits.allTraits.ListFullCopy();
        }

        public static void UpdateDisabilities(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            newPawn.skills.Notify_SkillDisablesChanged();
        }

        /*
        public static void ResetDisabledWorks(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, bool debug=false)
        {
            string myDebugSr = newPawn.LabelShort + " - ResetDisabledWorks - ";

            List<WorkTypeDef> Disabilities = newPawn.GetDisabledWorkTypes(true);

            Tools.Warn(myDebugSr + "Entered disabilities to remove:" + Disabilities.Count, debug);
            for (int i = Disabilities.Count() - 1; i >= 0; i--)
            {
                Tools.Warn(myDebugSr + "Trying to remove " + Disabilities[i], debug);
                newPawn.GetDisabledWorkTypes(true).RemoveAt(i);
            }
            newPawn.Notify_DisabledWorkTypesChanged();
        }

        public static void CopyDisabledWorks(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn, bool debug = false)
        {
            string myDebugSr = newPawn.LabelShort + " - CopyDisabledWorks - ";
            List<WorkTypeDef> Disabilities = comp.Pawn.GetDisabledWorkTypes(true);

            Tools.Warn(myDebugSr + "Entered disabilities to remove:" + Disabilities.Count, debug);
            foreach (WorkTypeDef WTD in Disabilities)
            {
                Tools.Warn(myDebugSr + "Trying to add " + WTD, debug);
                newPawn.GetDisabledWorkTypes(true).Add(WTD);
            }
            newPawn.Notify_DisabledWorkTypesChanged();
        }
        */
    }
}