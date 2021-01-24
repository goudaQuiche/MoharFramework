using Verse;
using System;
using RimWorld;

namespace MoharHediffs
{
    public class HediffComp_Steamer : HediffComp
    {
        private int ticksUntilSpray = 500;
        private int sprayTicksLeft;

        private bool MyDebug => Props.debug;
        private Map MyMap => Pawn.Map;
        public HediffCompProperties_Steamer Props =>(HediffCompProperties_Steamer)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligible())
            {
                if (MyDebug) Log.Warning("null pawn");
                return;
            }

            // Puff
            if (sprayTicksLeft <= 0)
            {
   
                // Smoke if random ok
                if (Rand.Chance(Props.puffingChance))
                {
                    //Log.Warning("Puffing");
                    MoteMaker.ThrowAirPuffUp(Pawn.TrueCenter(), MyMap);
                    GenTemperature.PushHeat(Pawn.Position, MyMap, Props.temperatureIncreasePerPuff);
                }

                // reset avec random // ça fait x10 ?!
                sprayTicksLeft = ticksUntilSpray = Rand.RangeInclusive(Props.MinTicksBetweenSprays, Props.MaxTicksBetweenSprays);
            
            }
            // decrease ticks
            else
            {
                sprayTicksLeft --;
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                result += "Puff in " + sprayTicksLeft.ToStringTicksToPeriod();
                return result;
            }
        }
    }
}
