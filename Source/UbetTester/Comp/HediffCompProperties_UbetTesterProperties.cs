using Verse;
using Ubet;
using System.Collections.Generic;
using RimWorld;

namespace UbetTester
{
    public class HediffCompProperties_UbetTesterProperties : HediffCompProperties
    {
        public UbetDef ubet;

        public int checkFrequency = 600;

        public bool debug = false;
        public bool PeriodicCheck => checkFrequency > 0;

        public HediffCompProperties_UbetTesterProperties()
        {
            compClass = typeof(HediffCompProperties_UbetTester);
        }
    }
}