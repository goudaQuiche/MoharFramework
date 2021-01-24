using Verse;
using UnityEngine;
using System.Linq;

namespace MoharAiJob
{
    public static class RetrieveGraveDiggerDef
    {
        public static GraveDiggerDef RetrieveGDD(this Pawn p, out bool outDebug, bool MyDebug = false)
        {
            string myDebugStr = MyDebug ? p.LabelShort + " RetrieveGraveDiggerDef RetrieveGDD " : "";
            GraveDiggerDef DefToUse = DefDatabase<GraveDiggerDef>.AllDefs.Where(gdd => gdd.workerPawnKind.Contains(p.kindDef)).FirstOrFallback(null);
            outDebug = false;
            if (DefToUse == null || DefToUse.IsEmpty)
            {
                if (MyDebug) Log.Warning(myDebugStr + "found no GraveDiggerDef for " + p.kindDef + "; exit");
                return null;
            }
            outDebug = DefToUse.debug;
            return DefToUse;
        }
        

        public static GraveDig_JobParameters RetrieveGDJP(this Pawn p, GraveDiggerDef GDD, bool MyDebug = false)
        {
            string myDebugStr = MyDebug ? p.LabelShort + " RetrieveGraveDiggerDef RetrieveGDJP " : "";

            GraveDig_JobParameters GDJP = p.WorkerFulfillsRequirements(GDD);
            if (GDJP == null)
            {
                if (MyDebug) Log.Warning(myDebugStr + "pawns does not fulfil requirements; exit");
                return null;
            }

            if (!GDJP.target.HasEligibleGraves)
            {
                if (MyDebug) Log.Warning(myDebugStr + "CRS has no eligible category def; exit");
                return null;
            }
            
            return GDJP;
        }

    }
}
