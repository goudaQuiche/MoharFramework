using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace OHPLS
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

        public static bool Get_LifeStageHediffAssociation_HediffComp_debug_and_race(HediffDef hediffdef, out bool debug, out string raceDefName)
        {
            HediffCompProperties_LifeStageHediffAssociation maybe;
            if (!Is_LifeStageHediffAssociation_HediffComp(hediffdef, out maybe))
            {
                debug = false;
                raceDefName = string.Empty;

                return false;
            }

            debug = maybe.debug;
            raceDefName = maybe.race;
            return true;
        }
        public static bool Is_LifeStageHediffAssociation_HediffComp(HediffDef hediffdef, out HediffCompProperties_LifeStageHediffAssociation hediffCompProperties_LifeStageHediffAssociation)
        {
            IEnumerable<HediffCompProperties> IEmaybe = hediffdef.comps;
            if (IEmaybe.EnumerableNullOrEmpty())
            {
                hediffCompProperties_LifeStageHediffAssociation = null;
                return false;
            }

            hediffCompProperties_LifeStageHediffAssociation = (HediffCompProperties_LifeStageHediffAssociation)IEmaybe.First();
            return true;
        }
    }
}
