using Verse;
using Verse.AI;
using RimWorld;

namespace MoharJoy
{
    

    public class JoyGiver_JoyOfNature : JoyGiver
    {
        public JoyOfNature_JoyGiverDef Def => def as JoyOfNature_JoyGiverDef;
        bool MyDebug => Def.debug;

        public override float GetChance(Pawn pawn)
        {
            string debugStr = MyDebug ? pawn.LabelShort + " JoyGiver_JoyOfNature - " : "";

            float getChance = base.GetChance(pawn);
            Tools.Warn("GetChance: "+getChance, MyDebug);

            return getChance;
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            string debugStr = MyDebug ? pawn.LabelShort + " TryGiveJob - " : "";

            Tools.Warn(debugStr + "entering ", MyDebug);

            if (!JoyUtility.EnjoyableOutsideNow(pawn) || pawn.Map.weatherManager.curWeather.rainRate > 0.1f)
            {
                Tools.Warn(debugStr + "EnjoyableOutsideNow/rain: ko", MyDebug);
                return null;
            }
            if (!this.TryFindPawnAndTargetCells(pawn, out IntVec3 pawnCell, out IntVec3 targetCell))
            {
                Tools.Warn(debugStr + "TryFindPawnAndTargetCells: ko", MyDebug);
                return null;
            }

            Tools.Warn(debugStr + "Making Job", MyDebug);

            return JobMaker.MakeJob(def.jobDef, pawnCell, targetCell);
        }
        
    }
}