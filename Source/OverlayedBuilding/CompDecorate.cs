using System;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;

// OLB stands for OverLayed Building
namespace OLB
{
    public class CompDecorate : ThingComp
    {
        public CompProperties_Decorate Props => (CompProperties_Decorate)props;
        Building building;

        public Pawn worker = null;
        public CompPowerTrader compPower = null;
        public CompRefuelable compFuel = null;

        IEnumerable<ReservationManager.Reservation> reservations;
        bool NeedToWatchForWorker = false;

        private MoteTracing[] moteTracer;

        bool myDebug => Props.debug;
        List<MoteDecoration> moteDeco => Props.moteDecorations;
        int moteNum => moteDeco.Count;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!(parent is Building))
            {
                Tools.Warn("this is a comp is meant for a building, this will fail", myDebug);
            }
            if (moteNum == 0)
            {
                Tools.Warn("There is no mote definition, this will fail", myDebug);
            }

            building = (Building)parent;

            compFuel = building.TryGetComp<CompRefuelable>();
            compPower = building.TryGetComp<CompPowerTrader>();

            moteTracer = new MoteTracing[moteNum];
            for (int i=0;i< moteNum; i++)
            {
                moteTracer[i] = new MoteTracing(Props.moteDecorations[i], myDebug);
            }

            // needs to be after moteTracers init or will fail
            NeedToWatchForWorker = AnyOfDecorationWatchForWorker;

            Tools.Warn(moteNum + " mote Def found; need NeedToWatchForWorker: " + NeedToWatchForWorker, myDebug);
        }

        bool UpdateReservation()
        {
            reservations = building.Map.reservationManager.ReservationsReadOnly.Where(
                r =>
                r.Target == new LocalTargetInfo(parent) &&
                r.Job.def != JobDefOf.Maintain && r.Job.def != JobDefOf.Deconstruct && r.Job.def != JobDefOf.Repair &&
                r.Faction == Faction.OfPlayer
            );

            return !reservations.EnumerableNullOrEmpty();
        }

        bool IsOccupied()
        {
            return !reservations.EnumerableNullOrEmpty() && reservations.FirstOrDefault().Claimant.Position == building.InteractionCell;
        }

        void UpdateWorker()
        {
            worker = IsOccupied() ? reservations.FirstOrDefault().Claimant : null;
        }

        public bool HasWorker
        {
            get
            {
                return worker != null;
            }
        }

        public bool AnyOfDecorationWatchForWorker
        {
            get
            {
                if (moteTracer.NullOrEmpty())
                    return false;

                return (moteTracer.Any(md => md.condition == MyDefs.DisplayCondition.WhenWorker));
            }

        }

        public override void CompTick()
        {
            base.CompTick();

            if (NeedToWatchForWorker && (Find.TickManager.TicksGame % Props.workerReservationUpdateFrequency == 0))
            {
                bool DoHaveReservation = UpdateReservation();
                UpdateWorker();
                Tools.Warn(building.LabelShort + " reservation: " + DoHaveReservation + " worker: " + worker?.LabelShort, myDebug);
            }

            for (int i = 0; i < moteNum; i++)
            {
                MoteDecoration curMoteDef = Props.moteDecorations[i];
                MoteTracing curMoteTrace = moteTracer[i];

                string debugStr = "[" + i + "/" + moteNum + "]";
                if (curMoteDef == null)
                {
                    Tools.Warn(debugStr + " Null curMoteDef", myDebug);
                    continue;
                }
                if (curMoteTrace == null)
                {
                    Tools.Warn(debugStr + " Null curMoteTrace", myDebug);
                    continue;
                }

                string moteName = curMoteDef.moteDef.defName;

                if  ( !curMoteDef.multipleMotesCoexisting && curMoteTrace.mote != null && curMoteTrace.mote.Spawned)
                {
                    //Tools.Warn(debugStr + " Found living mote for " + moteName, myDebug);
                    continue;
                }

                if (curMoteTrace.graceTime > 0)
                {
                    //Tools.Warn(debugStr + " graceTime not over " + curMoteTrace.graceTime, myDebug);
                    curMoteTrace.graceTime--;
                    continue;
                }

                if (!curMoteTrace.ShouldSpawnMote(this))
                {
                    //Tools.Warn(debugStr + " should not be displayed " + moteName + " - " + Tools.DescriptionAttr(curMoteTrace.condition), myDebug);
                    continue;
                }
                else
                {
                    Tools.Warn(debugStr + " should be displayed " + moteName + " - " + Tools.DescriptionAttr(curMoteTrace.condition), myDebug);
                }


                Tools.Warn(debugStr + " trying to spawn a mote for " + moteName + " on " + Tools.DescriptionAttr(curMoteTrace.origin), myDebug);

                curMoteTrace.mote = GfxEffects.SpawnMote(curMoteDef, curMoteTrace, building, worker);
                curMoteTrace.graceTime = curMoteDef.graceTime;

            }

        }
    }
}