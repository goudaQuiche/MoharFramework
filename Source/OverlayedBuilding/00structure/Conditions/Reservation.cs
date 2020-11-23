using Verse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLB
{
    public static class ReservationCondition
    {
        public static bool ReservationValidation(this CompDecorate comp)
        {
            if (!comp.CurItem.RequiresReservationCheck)
            {
                Tools.Warn(comp.CurItem.label + " did not require a reservation check ; ok", comp.CurItem.debug);
                return true;
            }

            if (comp.CurItem.condition.ifWorker && !comp.HasWorker)
            {
                Tools.Warn(comp.CurItem.label + " has no worker; ko", comp.CurItem.debug);
                return false;
            }

            //if (!comp.IsOccupied)
            if(comp.CurItem.condition.ifWorkerOnInteractionCell && !comp.HasWorkerOnInteractionCell)
            {
                Tools.Warn(comp.CurItem.label + " has no worker on interaction cell ; ko", comp.CurItem.debug);
                return false;
            }

            if (comp.CurItem.condition.ifWorkerTouch && !comp.HasWorkerTouchingBuilding)
            {
                Tools.Warn(
                    comp.CurItem.label + " has no worker touching building; "+
                    comp.Worker.Position+"-" +comp.GetBuilding.Position+ 
                    " - " + comp.GetBuilding.OccupiedRect().AdjacentCells.Contains(comp.Worker.Position) +
                    //" - " + comp.Worker.Position.IsAdjacentToCardinalOrInside(comp.GetBuilding.OccupiedRect()) + 
                    " ko", comp.CurItem.debug);
                return false;
            }

            if (!comp.ReservationIsItemCompatible())
            {
                Tools.Warn(comp.CurItem.label + " is not compatible with reservation ; ko", comp.CurItem.debug);
                return false;
            }

            Tools.Warn(comp.CurItem.label + " reservation ok", comp.CurItem.debug);
            return true;
        }
    }
}
