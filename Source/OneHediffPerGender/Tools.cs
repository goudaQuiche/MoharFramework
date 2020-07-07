using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace OHPG
{
    public static class Tools
    {
        public static bool OkPawn(Pawn pawn)
        {
            return ((pawn != null) && (pawn.Map != null));
        }

        public static bool TrueEveryNTicks(int NTicks=60)
        {
                return Find.TickManager.TicksGame % NTicks == 0;
        }
        public static bool TrueEverySec
        {
            get
            {
                return TrueEveryNTicks();
            }
        }
        public static bool TrueEvery5Sec
        {
            get
            {
                return TrueEveryNTicks(300);
            }
        }
        public static bool TrueEvery30Sec
        {
            get
            {
                return TrueEveryNTicks(1800);
            }
        }

        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

        public static bool Get_GenderHediffAssociation_HediffComp_debug_and_race(HediffDef hediffdef, out bool debug, out string raceDefName)
        {
            HediffCompProperties_GenderHediffAssociation maybe;
            if (!Is_GenderHediffAssociation_HediffComp(hediffdef, out maybe))
            {
                debug = false;
                raceDefName = string.Empty;

                return false;
            }

            debug = maybe.debug;
            raceDefName = maybe.race;
            return true;
        }
        public static bool Is_GenderHediffAssociation_HediffComp(HediffDef hediffdef, out HediffCompProperties_GenderHediffAssociation hediffCompProperties_Gender)
        {
            IEnumerable<HediffCompProperties> IEmaybe = hediffdef.comps;
            if (IEmaybe.EnumerableNullOrEmpty())
            {
                hediffCompProperties_Gender = null;
                return false;
            }

            hediffCompProperties_Gender = (HediffCompProperties_GenderHediffAssociation)IEmaybe.First();
            return true;
        }
    }
}
