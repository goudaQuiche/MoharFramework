using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public class RegenParamsSpecial
    {
        public IntRange PeriodBase = new IntRange(600, 1200);
        public FloatRange RegenerationBase = new FloatRange(.05f, .15f);

        public float BPMaxHealth = .35f;

        public ThingDef MoteDef;

        public float HungerCost = 0;
        public float RestCost = 0;

        public byte Priority = 0;
    }
}
