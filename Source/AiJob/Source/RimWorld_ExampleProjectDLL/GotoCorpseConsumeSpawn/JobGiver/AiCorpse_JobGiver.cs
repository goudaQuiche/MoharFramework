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

            CorpseJobDef DefToUse = pawn.RetrieveCorpseJobDef(out MyDebug, PreRetrieveDebug);
            if (DefToUse == null)
            {
                if (PreRetrieveDebug) Log.Warning(myDebugStr + " found no CorpseJobDef; exit");
                return null;
            }
                
            IEnumerable<CorpseRecipeSettings> CRSList = pawn.RetrieveCorpseRecipeSettings(DefToUse, MyDebug);
            if (CRSList.EnumerableNullOrEmpty())
            {
                if (MyDebug) Log.Warning(myDebugStr + " found no CorpseRecipeSettings; exit");
                return null;
            }
                

            foreach (CorpseRecipeSettings CRS in CRSList)
            {
                Corpse FoundCorpse = pawn.GetClosestCompatibleCorpse(CRS.target, MyDebug);

                if (FoundCorpse.NegligibleThing())
                {
                    if (MyDebug) Log.Warning(myDebugStr + "corpse " + FoundCorpse?.Label + " " + FoundCorpse?.Position + " is negligible; exit");
                    continue;
                }

                if (MyDebug) Log.Warning(myDebugStr + " accepting " + DefToUse.jobDef.defName + " for corpse " + FoundCorpse?.Label + " " + FoundCorpse?.Position + " => go go");
                Job job = JobMaker.MakeJob(DefToUse.jobDef, FoundCorpse);

                return job;
            }
            return null;
        }
    }
}
