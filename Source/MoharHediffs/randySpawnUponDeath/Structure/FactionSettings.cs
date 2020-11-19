using Verse;
using RimWorld;
using System.Linq;

namespace MoharHediffs
{
    public class FactionPickerParameters
    {
        // Item faction
        public bool inheritedFaction = false;
        public FactionDef forcedFaction = null;
        public bool playerFaction = false;
        public bool defaultPawnKindFaction = false;
        public bool noFaction = false;

        public float weight;

        public bool HasInheritedFaction => inheritedFaction == true;
        public bool HasForcedFaction => forcedFaction != null;
        public bool HasPlayerFaction => playerFaction == true;
        public bool HasNoFaction => noFaction == true;
        public bool HasDefaultPawnKindFaction => defaultPawnKindFaction == true;

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
            if(HasDefaultPawnKindFaction)
                shouldBeOne += 1;

            return shouldBeOne == 1;
        }

        public void Dump()
        {
            Log.Warning(
                "inherited:" + HasInheritedFaction + "; "+
                "forced:" + HasForcedFaction + "; " +
                "player:" + HasPlayerFaction + "; " +
                "noFaction:" + HasNoFaction + "; " +
                "defaultPawnKindFaction:" + HasDefaultPawnKindFaction+ "; "
            );
        }
    }
}