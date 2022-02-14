using Verse;
using Ubet;

namespace UbetTester
{
    public class HediffCompProperties_UbetTester : HediffComp
    {
        public HediffCompProperties_UbetTesterProperties Props => (HediffCompProperties_UbetTesterProperties)props;
        bool MyDebug => Props.debug;

        UbetDef UD => Props.ubet;
        bool ubetResult = false;

        public void CheckUbet(bool debug = false)
        {
            ubetResult = Pawn.TrunkNodeComputation(UD.trunk, MyDebug);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!Pawn.Spawned)
            {
                if (MyDebug) Log.Warning("pawn unspawned");
                return;
            }

            if (Props.PeriodicCheck && Pawn.IsHashIntervalTick(Props.checkFrequency))
            {
                if (MyDebug) Log.Warning("Checking Ubet");
                CheckUbet(MyDebug);
            }
                
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (Props.debug)
                {
                    result += Pawn.PawnResumeString() + "; UbetTest:" + ubetResult.ToString();
                }

                return result;
            }
        }
    }
}
