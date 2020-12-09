using Verse;
using System.Collections.Generic;
using RimWorld;

namespace MoharCustomHAR
{
    public class JobParameters
    {
        public List<JobSpecificParameters> hideIfJob;
        public List<JobSpecificParameters> drawIfJob;
        public bool drawIfNullJob = true;

        public bool HasHideJobs => !hideIfJob.NullOrEmpty();
        public bool HasDrawJobs => !drawIfJob.NullOrEmpty();
    }
    public class JobSpecificParameters
    {
        public JobDef job;
        public PostureOrMoving postureOrMoving;

        public bool HasPostureOrMoving => postureOrMoving != null;
    }
    public class PostureOrMoving
    {
        public List<PawnPosture> postures;
        public bool moving = true;

        public bool IsOkWithPostureAndMoving(Pawn p)
        {
            return postures.Contains(p.GetPosture()) && moving == p.pather.MovingNow;
        }
    }
}
