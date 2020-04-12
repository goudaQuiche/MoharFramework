using Verse;
using System;
using RimWorld;

namespace MoharHediffs
{
    public class HediffComp_Steamer : HediffComp
    {
        Pawn steamEmitter = null;

        private int ticksUntilSpray = 500;
        private int sprayTicksLeft;

        /*public Action startSprayCallback;
        public Action endSprayCallback;*/

        public HediffCompProperties_Steamer Props
        {
            get
            {
                return (HediffCompProperties_Steamer)this.props;
            }
        }
        
    public override void CompPostTick(ref float severityAdjustment)
    {
        steamEmitter = this.parent.pawn;
            //Log.Warning(steamEmitter.Label + " tick = " + this.sprayTicksLeft + "limit=" +this.ticksUntilSpray);

         if (steamEmitter == null)
        {
            Log.Warning("pawn null");
            return;

        }
        if (steamEmitter.Map == null)
        {
            Log.Warning("pawn.Map null");
            return;
        }

        // Puff
        if (this.sprayTicksLeft <= 0)
        {
   
            // Smoke if random ok
            if (Rand.Value < this.Props.puffingChance)
            {
                //Log.Warning("Puffing");
                MoteMaker.ThrowAirPuffUp(steamEmitter.TrueCenter(), steamEmitter.Map);

            }

            // Temperature
            if (Find.TickManager.TicksGame % 20 == 0)
            {
                GenTemperature.PushHeat( steamEmitter.Position, steamEmitter.Map, 40f);
            }

            // reset avec random // ça fait x10 ?!
            this.sprayTicksLeft = this.ticksUntilSpray = Rand.RangeInclusive(this.Props.MinTicksBetweenSprays, this.Props.MaxTicksBetweenSprays);
            
        }
        // decrease ticks
        else
        {
            this.sprayTicksLeft --;
        }

        if (this.ticksUntilSpray <= 0)
        {
            this.sprayTicksLeft = Rand.RangeInclusive(this.Props.MinTicksBetweenSprays, this.Props.MaxTicksBetweenSprays);
        }

    }

    public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                result += "Puff in " + this.sprayTicksLeft.ToStringTicksToPeriod();
                return result;
            }
        }
    
    }
}
