using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MoharBlood
{
    public static class Harmony_Utils
    {
        public static void LogWarning()
        {
            Log.Warning("LogWarning Harmony_Utils Transpiler");
        }

        public static void LogWarningTarget(TargetInfo A)
        {
            Log.Warning("LogWarningTarget :" + A.HasThing + " - " + A.Thing?.def);
        }

        public static void LogWarningSubEffecterDef(SubEffecterDef SEDef)
        {
            Log.Warning(
                "LogWarningSubEffecterDef :" + SEDef?.fleckDef);
        }

        public static void LogWarningColor(Color color)
        {
            Log.Warning("LogWarningColor :" + color);
        }

        public static void LogWarningPawn(Pawn p)
        {
            Log.Warning("LogWarningPawn :" + p?.LabelShort);
        }

        public static void LogWarningFloat(float a)
        {
            Log.Warning("LogWarningFloat :" + a);
        }

        public static void LogAround(List<CodeInstruction> instructionList, int i, int iMinDiff, int iMaxDiff)
        {
            string ErrorLog = string.Empty;

            for (int j = iMinDiff; j <= iMaxDiff; j++)
            {
                ErrorLog += "[" + (j + i) + "][" + j + "]" + instructionList[j + i].ToString() + "\n";
            }
            Log.Error(ErrorLog);
        }
    }


}
