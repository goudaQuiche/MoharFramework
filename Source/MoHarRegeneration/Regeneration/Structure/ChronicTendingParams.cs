using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class ChronicTendingParams
    {
        public IntRange PeriodBase = new IntRange(300, 600);
        public FloatRange TendingQuality = new FloatRange(.75f, 2f);
        public List<HediffDef> HediffDefs = null;

        public byte Priority = 0;
    }
}
