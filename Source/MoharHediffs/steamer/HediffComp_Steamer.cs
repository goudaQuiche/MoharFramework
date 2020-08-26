using Verse;
using System;
using RimWorld;

namespace MoharHediffs
{
    public class HediffComp_Steamer : HediffComp
    {
        Pawn myPawn = null;

        private int ticksUntilSpray = 500;
        private int sprayTicksLeft;

        private bool myDebug = false;

        public HediffCompProperties_Steamer Props
        {
            get
            {
                return (HediffCompProperties_Steamer)this.props;
            }
        }

        public override void CompPostMake()
        {
            myDebug = Props.debug;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            myPawn = parent.pawn;

            if (myPawn == null)
            {
                Tools.Warn("pawn null", myDebug);
                return;
            }
            if (myPawn.Map == null)
            {
                //Tools.Warn(myPawn.Label + " - pawn.Map null", myDebug);
                return;
            }

            // Puff
            if (this.sprayTicksLeft <= 0)
            {
   
                // Smoke if random ok
                if (Rand.Value < this.Props.puffingChance)
                {
                    //Log.Warning("Puffing");
                    MoteMaker.ThrowAirPuffUp(myPawn.TrueCenter(), myPawn.Map);
                }

                // Temperature
                if (Find.TickManager.TicksGame % 20 == 0)
                {
                    GenTemperature.PushHeat(myPawn.Position, myPawn.Map, 40f);
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
