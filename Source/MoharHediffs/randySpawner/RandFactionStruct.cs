using Verse;
using RimWorld;
using System.Linq;

namespace MoharHediffs
{
    public class RandomFactionParameter
    {
        // Item faction
        public bool inheritedFaction = false;
        public FactionDef forcedFaction = null;
        public bool playerFaction = false;
        public bool noFaction = false;

        public float weight;

        public bool HasInheritedFaction => inheritedFaction == true;
        public bool HasForcedFaction => forcedFaction != null;
        public bool HasPlayerFaction => playerFaction == true;
        public bool HasNoFaction => noFaction == true;

        public bool IsLegitRandomFactionParameter()
        {
            int shouldBeOne = 0;
            if (HasInheritedFaction)
                shouldBeOne += 1;
            if (HasForcedFaction)
                shouldBeOne += 1;
            if (HasPlayerFaction)
                shouldBeOne += 1;
            if (HasNoFaction)
                shouldBeOne += 1;

            return shouldBeOne == 1;
        }

        public Faction GetFaction(Pawn p)
        {
            FactionDef fDef = GetFactionDef(p);
            return Find.FactionManager.AllFactions.Where(F => F.def == fDef).FirstOrFallback();
        }

        public FactionDef GetFactionDef(Pawn p)
        {
            if (HasInheritedFaction)
                return p.Faction.def;
            else if (HasForcedFaction)
                return forcedFaction;
            else if (HasPlayerFaction)
                return Faction.OfPlayerSilentFail.def;
            else if (HasNoFaction)
                return null;

            return null;
        }

    }
}