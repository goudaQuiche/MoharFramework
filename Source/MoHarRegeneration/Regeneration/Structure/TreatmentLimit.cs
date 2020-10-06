using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class TreatmentLimit
    {
        public bool IsQuantityLimited = false;
        public int LimitedTreatmentQuantity = 0;

        public bool IsQualityLimited = false;
        public float LimitedTreatmentQuality = 0;
    }
}
