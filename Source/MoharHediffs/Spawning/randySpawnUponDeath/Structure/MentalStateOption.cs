using Verse;
using RimWorld;
using System.Linq;

namespace MoharHediffs
{
    public class MentalStateOption
    {
        public MentalStateDef mentalDef;
        public float weight = 1;

        public void Dump()
        {
            Log.Warning(
                "MentalStateDef:" + mentalDef.defName + "; " +
                "weight:" + weight + "; "
            );
        }
    }
}