using Verse;
using Verse.AI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace MoharAiJob
{
    public class Ai_GraveDigger_JobGiver : ThinkNode_JobGiver
    {
        public bool MyDebug = false;
        public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;

        protected override Job TryGiveJob(Pawn pawn)
        {
            string myDebugStr = PreRetrieveDebug ? pawn.LabelShort + " Ai_GraveDigger_JobGiver TryGiveJob " : "";

            if (pawn.NegligiblePawn())
            {
                if (PreRetrieveDebug) Log.Warning(myDebugStr + "negligible; exit");
                return null;
            }

            GraveDiggerDef DefToUse = pawn.RetrieveGDD(out MyDebug, PreRetrieveDebug);
            GraveDig_JobParameters GDJP = pawn.RetrieveGDJP(DefToUse, MyDebug);

            if(!pawn.GetClosestCompatibleGrave(GDJP.target, out Thing FoundGrave, out Thing FoundCorpse, MyDebug))
            {
                if (MyDebug) Log.Warning(myDebugStr + "grave or corpse " + FoundGrave?.Label + " " + FoundGrave?.Position + " is not ok; exit");
                return null;
            }

            if (MyDebug) Log.Warning(myDebugStr + " accepting "+ DefToUse.jobDef.defName + " for grave " + FoundGrave?.Label + " " + FoundGrave?.Position + " => go go");
            Job job = JobMaker.MakeJob(DefToUse.jobDef, FoundGrave, FoundCorpse);

            return job;
        }
    }
}
