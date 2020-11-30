using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace MoharJoy
{
/*    
    public class JobDriver_ThrowRocks : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            Toil toil = new Toil
            {
                initAction = delegate
                {
                    pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                },
                tickAction = delegate
                {
                    float extraJoyGainFactor = 1;
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = job.def.joyDuration
            };
            toil.FailOn(() => pawn.Position.Roofed(pawn.Map));
            toil.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
            yield return toil;
        }

        public override string GetReport()
        {
            return "Throwing rocks needs translate";
        }
        
    }
    */
}
