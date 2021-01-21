using Verse;
using Verse.AI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace MoharAiJob
{
    public class AiCorpse_JobGiver : ThinkNode_JobGiver
    {
        //static CorpseJobDef DefToUse = AICorpseJobDefOf.myJobDef;
        public bool MyDebug = false;
        public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;

        protected override Job TryGiveJob(Pawn pawn)
        {
            string myDebugStr = PreRetrieveDebug ? pawn.LabelShort + " AiCorpse_JobGiver TryGiveJob " : "";

            if (pawn.NegligiblePawn())
            {
                if (PreRetrieveDebug) Log.Warning(myDebugStr + "negligible; exit");
                return null;
            }

            CorpseJobDef DefToUse = pawn.RetrieveCJD(out MyDebug, PreRetrieveDebug);
            CorpseRecipeSettings CRS = pawn.RetrieveCRS(DefToUse, MyDebug);

            Corpse FoundCorpse = pawn.GetClosestCompatibleCorpse(CRS.target.categoryDef, CRS.target.maxDistance, MyDebug);

            if (FoundCorpse.NegligibleThing())
            {
                if (MyDebug) Log.Warning(myDebugStr + "corpse " + FoundCorpse?.Label + " " + FoundCorpse?.Position + " is negligible; exit");
                return null;
            }

            if (MyDebug) Log.Warning(myDebugStr + " accepting job for corpse " + FoundCorpse?.Label + " " + FoundCorpse?.Position + " => go go");

            Job job = JobMaker.MakeJob(AICorpseJobDefOf.MoharAiJob_ConsumeCorpse, FoundCorpse);
            return job;
        }
    }


    [DefOf]
    public static class AICorpseJobDefOf
    {
        public static JobDef MoharAiJob_ConsumeCorpse;
    }
    
}
