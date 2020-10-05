using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class HealingParams
    {
        public IntRange PeriodBase = new IntRange(300, 600);
        public FloatRange HealingQuality = new FloatRange(.75f, 2f);

        public ThingDef MoteDef;

        public string TreatmentLabel = string.Empty;

        public HediffDef HediffToApplyDuringProgress;
        public bool RemoveHediffWhenProgressOver = false;
        public HediffDef HediffToApplyWhenComplete;

        public float HungerCost = 0;
        public float RestCost = 0;

        public byte Priority = 0;
    }
}
