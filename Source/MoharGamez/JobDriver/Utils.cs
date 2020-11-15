﻿using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharGamez
{
    public static class JobInitialize
    {
        public static float TotalWeight(this GameProjectile GP)
        {
            float total = 0;

            List<ProjectileOption> POL = GP.projectileOptionList;

            for (int i = 0; i < POL.Count; i++)
                total += POL[i].weight;

            //Tools.Warn("TotalWeight: " + total, GP.Debug);
            return total;
        }

        public static int GetWeightedRandomIndex(this GameProjectile GP)
        {
            float DiceThrow = Rand.Range(0, GP.TotalWeight());
            List<ProjectileOption> POL = GP.projectileOptionList;

            for (int i = 0; i < POL.Count; i++)
            {
                if ((DiceThrow -= POL[i].weight) < 0)
                {
                    //Tools.Warn("GetWeightedRandomIndex : returning " + i, GP.Debug);
                    return i;
                }
            }

            Tools.Warn("GetWeightedRandomIndex : failed to return proper index, returning -1", GP.debug);

            return -1;
        }
        public static bool RetrieveProjectileParam(this JobDriver_PlayGenericTargetingGame PGTG)
        {
            bool myDebug = PGTG.gameProjectile.debug;

            string DebugStr = PGTG.pawn.LabelShort + " - RetrieveProjectileParam";
            Tools.Warn(DebugStr + " - Entering", myDebug);

            int randomIndex = PGTG.gameProjectile.GetWeightedRandomIndex();
            PGTG.projectileOption = PGTG.gameProjectile.projectileOptionList[randomIndex];

            if (PGTG.projectileOption.IsMoteType)
            {
                PGTG.PickedMoteParam = PGTG.projectileOption.mote;
                Tools.Warn( DebugStr + " - Found mote => " + PGTG.MoteDef.defName, myDebug);
            }
            else if (PGTG.projectileOption.IsShadowMoteType)
            {
                PGTG.PickedMoteParam = PGTG.projectileOption.shadowMote;
                Tools.Warn(DebugStr + " - Found shadow mote", myDebug);
            }
            else
            {
                Tools.Warn(DebugStr + " - Found nothing", myDebug);
            }

            return PGTG.HasProjectileOption;
        }

        public static void ResetPickedOption(this JobDriver_PlayGenericTargetingGame PGTG)
        {
            PGTG.projectileOption = null;
            PGTG.PickedMoteParam = null;
            PGTG.RetrieveProjectileParam();
        }
        
        
    }
}
