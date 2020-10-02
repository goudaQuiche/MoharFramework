using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class BleedTendingParams
    {
        public IntRange PeriodBase = new IntRange(300, 600);
        public FloatRange TendingQuality = new FloatRange(.75f, 2f);
//        public HediffDef HediffDef = HediffDefOf.BloodLoss;

        public byte Priority = 0;
    }
}
