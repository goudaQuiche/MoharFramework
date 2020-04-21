using Verse;
using System;
using RimWorld;

namespace MoharHediffs
{
    public class HediffComp_Filther : HediffComp
    {
        Pawn myPawn = null;

        private int ticksUntilFilth = 500;
        private int filthTicksLeft;

        private bool myDebug = false;

        public HediffCompProperties_Filther Props
        {
            get
            {
                return (HediffCompProperties_Filther)this.props;
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
            if (Props.filthDef == null)
                return;

            // Puff
            if (this.filthTicksLeft <= 0)
            {
                FilthMaker.TryMakeFilth(myPawn.Position, myPawn.Map, Props.filthDef);
                filthTicksLeft = ticksUntilFilth = Rand.RangeInclusive(Props.MinTicksBetweenSprays, Props.MaxTicksBetweenSprays);
            }
            // decrease ticks
            else
            {
                this.filthTicksLeft --;
            }
        }

        /*
        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                result += "Puff in " + this.filthTicksLeft.ToStringTicksToPeriod();
                return result;
            }
        }
        */
    }
}
