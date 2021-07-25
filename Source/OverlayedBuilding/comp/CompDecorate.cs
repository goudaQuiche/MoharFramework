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
        public Building GetBuilding => (Building)parent;

        // Condition Fuel && Power
        public CompRefuelable CompFuel => GetBuilding.TryGetComp<CompRefuelable>() ?? null;
        public CompPowerTrader CompPower => GetBuilding.TryGetComp<CompPowerTrader>() ?? null;
        
        public bool HasFuelComp => CompFuel != null;
        public bool HasPowerComp => CompPower != null;

        public bool IsFueled => HasFuelComp && CompFuel.Fuel > 0;
        public bool HasPowerOn => HasPowerComp && CompPower.PowerOn;

        public bool RequiresFuelCheck => CurItem.RequiresFuelCheck;
        public bool RequiresPowerCheck => CurItem.RequiresPowerCheck;
        public bool RequiresFuelAndPowerCheck => CurItem.RequiresFuelAndPowerCheck;

        // Condition reservation & worker
        public IEnumerable<ReservationManager.Reservation> reservations;
        public Pawn Worker = null;
        public bool HasWorker => Worker != null;
        public bool IsReserved => !reservations.EnumerableNullOrEmpty();
        public ReservationManager.Reservation FirstReservation => reservations.FirstOrDefault() ?? null;
        public bool HasNonMovingWorker => IsReserved && HasWorker && !Worker.pather.MovingNow;

        public bool HasWorkerOnInteractionCell => HasNonMovingWorker && Worker.Position == GetBuilding.InteractionCell;
        public bool HasWorkerTouchingBuilding => HasNonMovingWorker && GetBuilding.OccupiedRect().AdjacentCells.Contains(Worker.Position);

        // general
        public bool IsReservationUpdateRequired => ItemList.Any(md => md.RequiresReservationUpdate);
            //for the item
        public bool RequiresWorkerCheck => CurItem.RequiresReservationUpdate;
            // for the comp
        public bool RequiresReservationUpdate = false;
        // time
        public bool IsTimeToUpdate => Find.TickManager.TicksGame % Props.workerReservationUpdateFrequency == 0;

        // Condition selected
        public bool IsSelected => Find.Selector.IsSelected(parent);
        public bool RequiresSelectionCheck => CurItem.RequiresSelectionCheck;

        // Mote item
        public List<MoteDecoration> ItemList => Props.mote;
        public int MoteNum => ItemList.Count;
        public bool EmptyParameters => MoteNum == 0;

        public int MoteIndex = 0;
        public bool ValidIndex => (MoteIndex >= 0) && (MoteIndex < MoteNum);
        public MoteDecoration CurItem => ValidIndex ? Props.mote[MoteIndex] : null;
        public bool NullCurItem => CurItem == null;
        public bool NonNullCurItem => !NullCurItem;

        public List<MoteTracer> LivingMotes = new List<MoteTracer>();
        public bool HasLivingMotes => !LivingMotes.NullOrEmpty();
        public bool HasEmptyTracer => !HasLivingMotes;

        public bool MyDebug => Props.debug;
        public bool DebugCheck => MyDebug && Props.verboseLevel >= 1;
        public bool DebugOutsideLoop => MyDebug && Props.verboseLevel >= 2;
        public bool DebugInsideLoop => MyDebug && Props.verboseLevel >= 3;

        public bool DoNothing = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
                this.CheckGeneral();
            RequiresReservationUpdate = IsReservationUpdateRequired;
            if (RequiresReservationUpdate)
                this.UpdateReservationAndWorker();

        }

        public override void CompTick()
        {
            if (DoNothing)
                return;

            base.CompTick();

            string debugStr = MyDebug ? parent.Label + " CompTick - " : "";

            this.MaybeUpdateReservations();

            //Tools.Warn(debugStr + "LivingMotes Loop", DebugOutsideLoop);
            for (int i = LivingMotes.Count - 1; i >= 0; i--)
            {
                MoteTracer MT = LivingMotes[i];
                if (MT.MoteIsDead())
                    LivingMotes.RemoveAt(i);
                MT.DecreaseGraceTicks();
            }

            //Tools.Warn(debugStr + "LivingMotes not coexisting", DebugOutsideLoop);
            if (this.NonCoexistingMoteInTracer())
                return;

            //Tools.Warn(debugStr + "Looping around " + MoteNum + " mote params", DebugOutsideLoop);
            for (MoteIndex = 0; MoteIndex < MoteNum; MoteIndex++)
            {
                string debugLoopStr = "[" + (MoteIndex+1) + "/" + MoteNum + "]";

                if (NullCurItem) {
                    Tools.Warn(debugStr + debugLoopStr + " curItem is null, skipped", DebugInsideLoop);
                    continue;
                }
                else
                    Tools.Warn(debugStr + debugLoopStr + " curItem is real, going on ", DebugInsideLoop && CurItem.debug);
                

                if (CurItem.IsInvalid)
                {
                    Tools.Warn(debugStr + debugLoopStr + " is invalid, skipped", DebugInsideLoop && CurItem.debug);
                    continue;
                }else Tools.Warn(debugStr + debugLoopStr + " is valid, going on", DebugInsideLoop && CurItem.debug);

                string moteName = MyDebug ? CurItem.moteDef.defName : ""; ;

                Tools.Warn(debugStr + debugLoopStr + " condition validating: " + moteName , DebugInsideLoop && CurItem.debug);
                if (!this.AllConditionsValidation())
                    continue;
                else
                {
                    Tools.Warn(debugStr + " should be displayed " + moteName, DebugInsideLoop && CurItem.debug);
                }

                LivingMotes.Add(
                    new MoteTracer(
                        CurItem.label,
                        GfxEffects.SpawnMote(CurItem, GetBuilding, Worker),
                        CurItem.graceTicks,
                        CurItem.coexistsWithSame,
                        CurItem.coexistsWithOther
                    )
                );
            }

            //Tools.Warn("motes end Loop", DebugOutsideLoop);

        }
    }
}