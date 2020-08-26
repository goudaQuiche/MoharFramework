using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace OHPG
{
    public class CustomRace_HediffGiver_OHPG : HediffGiver
    {
        bool SafeRemoval = LoadedModManager.GetMod<OHPG_Mod>().GetSettings<OHPG_Settings>().SafeRemoval;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (SafeRemoval) return;

            if (pawn == null || pawn.Map == null || !pawn.Spawned)
                return;

            string myPawnResume = pawn.LabelShort + "(" + pawn.def.defName + ")";
            string myHediffDesc = "Apply hediff depending on gender Hediff";

            bool Is_Wanted_Hediff = Tools.Get_GenderHediffAssociation_HediffComp_debug_and_race(hediff, out bool debug, out string raceDefName);

            if (!Is_Wanted_Hediff)
            {
                Tools.Warn(myPawnResume + " calling hediff(" + hediff?.defName + ") is wrong ", debug);
                return;
            }

            if (!pawn.IsRaceMember(raceDefName))
            {
                Tools.Warn(myPawnResume + " is not race member of " + raceDefName, debug);
                return;
            }

            if (pawn.HasHediff(hediff))
            {
                //Tools.Warn(myPawnResume + " already has " + myHediffDesc, debug);
                return;
            }

            Tools.Warn(myPawnResume + " => got " + myHediffDesc + " applied on ", debug);
            HealthUtility.AdjustSeverity(pawn, hediff, .1f);
        }
    }
}
