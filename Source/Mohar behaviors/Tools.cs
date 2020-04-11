using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;


namespace MoharBehaviors
{
    public class Tools
    {

        public static bool OkPawn(Pawn pawn)
        {
            return ((pawn != null) && (pawn.Map != null));
        }

        public static string OkStr(bool boolean=false)
        {
            return "[" + ((boolean) ? ("OK") : ("KO")) + "]";
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }
        public static void WarnRare(string warning, int period=300, bool debug = false)
        {
            if (debug)
            {
                bool display = ((Find.TickManager.TicksGame % period)==0);
                if (display)
                    Log.Warning(warning);
            }
        }
        
        public static string PawnResumeString(Pawn pawn)
        {
            return (pawn?.LabelShort.CapitalizeFirst() +
                    ", " +
                    (int)pawn?.ageTracker?.AgeBiologicalYears + " y/o" +
                    //" " + pawn?.gender.ToString()?.Translate()?.ToLower() +
                    " " + pawn?.gender.GetLabel() + 
                    ", " + pawn?.def?.label + "("+pawn.kindDef+")"
                    );
        }

        //debug Toggle kinda pointless
        public static string DebugStatus(bool debug)
        {
            return (debug+ "->" + !debug);
        }

        //PauseOnError for debug purpose
        public static void PauseOnErrorToggle()
        {
            Prefs.PauseOnError = !Prefs.PauseOnError;
        }

    }
}
