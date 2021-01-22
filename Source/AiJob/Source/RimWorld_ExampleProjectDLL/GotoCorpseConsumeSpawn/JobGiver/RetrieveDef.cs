using Verse;
using UnityEngine;
using System.Linq;

namespace MoharAiJob
{
    public static class RetrieveDefs
    {
        public static CorpseJobDef RetrieveCJD(this Pawn p, out bool outDebug, bool MyDebug = false)
        {
            string myDebugStr = MyDebug ? p.LabelShort + " AiCorpse_JobGiver TryGiveJob " : "";
            CorpseJobDef DefToUse = DefDatabase<CorpseJobDef>.AllDefs.Where(cjd => cjd.workerPawnKind.Contains(p.kindDef)).FirstOrFallback(null);
            outDebug = false;
            if (DefToUse == null || DefToUse.IsEmpty)
            {
                if (MyDebug) Log.Warning(myDebugStr + "found no CorpseJobDef for " + p.kindDef + "; exit");
                return null;
            }
            outDebug = DefToUse.debug;
            return DefToUse;
        }
        

        public static CorpseRecipeSettings RetrieveCRS(this Pawn p, CorpseJobDef CJD, bool MyDebug = false)
        {
            string myDebugStr = MyDebug ? p.LabelShort + " RetrieveDefs RetrieveCRS " : "";

            CorpseRecipeSettings CRS = p.WorkerFulfillsRequirements(CJD);
            if (CRS == null)
            {
                if (MyDebug) Log.Warning(myDebugStr + "pawns does not fulfil requirements; exit");
                return null;
            }

            if (!CRS.target.HasCorpseCategoryDef)
            {
                if (MyDebug) Log.Warning(myDebugStr + "CRS has no Corpse category def; exit");
                return null;
            }

            return CRS;
        }

    }
}
