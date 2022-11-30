using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;
using UnityEngine;

namespace OLB
{
    public static class ReservationUtils
    {
        public static Vector2 GetOffset(this Offset offset, Rot4 rot)
        {
            switch (rot.AsInt)
            {
                case 0:
                    return offset.north;
                case 1:
                    return offset.east;
                case 2:
                    return offset.south;
                case 3:
                    return offset.west;
                default:
                    return offset.north;
            }
        }

        public static bool UpdateReservation(this CompDecorate comp)
        {
            comp.reservations = comp.parent.Map.reservationManager.ReservationsReadOnly.Where(
                r =>
                r.Target == new LocalTargetInfo(comp.parent) &&
                r.Faction == Faction.OfPlayer
            );

            return comp.IsReserved;
        }

        public static bool UpdateWorker(this CompDecorate comp)
        {
            comp.Worker = comp.IsReserved ? comp.FirstReservation.Claimant : null;
            return comp.HasWorker;
        }

        public static bool UpdateReservationAndWorker(this CompDecorate comp)
        {
            bool DoHaveReservation = comp.UpdateReservation() && comp.UpdateWorker();
            Tools.Warn(comp.parent.LabelShort + " >> reservation: " + DoHaveReservation + " worker: " + comp.Worker?.LabelShort, comp.MyDebug);
            return DoHaveReservation;
        }

        public static bool ReservationIsItemCompatible(this CompDecorate comp)
        {
            if (!comp.IsReserved)
                return false;

            if (comp.FirstReservation == null)
                return false;

            ReservationManager.Reservation resItem = comp.FirstReservation;

            bool compatible = true;

            if (comp.CurItem.condition.HasIncludedJob)
                if (!(compatible &= comp.CurItem.condition.includeJob.Contains(resItem.Job.def)))
                    return false;

            if (comp.CurItem.condition.HasExcludedJob)
                if (!(compatible &= !comp.CurItem.condition.excludeJob.Contains(resItem.Job.def)))
                    return false;

            if (comp.CurItem.condition.HasIncludedRecipe && resItem.Job.RecipeDef != null)
                if (!(compatible &= comp.CurItem.condition.includeRecipe.Contains(resItem.Job.RecipeDef)))
                    return false;

            if (comp.CurItem.condition.HasExcludedRecipe && resItem.Job.RecipeDef != null)
                if (!(compatible &= !comp.CurItem.condition.excludeRecipe.Contains(resItem.Job.RecipeDef)))
                    return false;

            return compatible;
        }


        public static void MaybeUpdateReservations(this CompDecorate comp)
        {
            if (!comp.RequiresReservationUpdate || !comp.IsTimeToUpdate)
                return;

            comp.UpdateReservationAndWorker();
        }
}
}
