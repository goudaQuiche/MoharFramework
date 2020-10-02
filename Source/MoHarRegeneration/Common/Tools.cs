using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class Tools
    {
        public static void DestroyParentHediff(Hediff parentHediff, bool debug=false)
        {
            if (parentHediff.pawn != null && parentHediff.def.defName != null)
                Warn(parentHediff.pawn.Label + "'s Hediff: " + parentHediff.def.defName + " says goodbye.", debug);

            parentHediff.Severity = 0;
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

        public static AlienPartGenerator.AlienComp GetAlien(Pawn pawn = null)
        {
            AlienPartGenerator.AlienComp alienComp = null;
            alienComp = pawn?.TryGetComp<AlienPartGenerator.AlienComp>();

            return alienComp;
        }
    }
}
