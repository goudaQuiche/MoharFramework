using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

namespace ConPoDra
{
    public static class ReservationUtils
    {
        public static bool UpdateReservation(this CompConditionalPostDraw comp)
        {
            comp.reservations = comp.parent.Map.reservationManager.ReservationsReadOnly.Where(
                r =>
                r.Target == new LocalTargetInfo(comp.parent) &&
                r.Faction == Faction.OfPlayer
            );

            return comp.IsReserved;
        }

        public static bool UpdateWorker(this CompConditionalPostDraw comp)
        {
            comp.Worker = comp.IsReserved ? comp.FirstReservation.Claimant : null;
            return comp.HasWorker;
        }

        public static bool UpdateReservationAndWorker(this CompConditionalPostDraw comp)
        {
            bool DoHaveReservation = comp.UpdateReservation();
            comp.UpdateWorker();
            Tools.Warn(comp.parent.LabelShort + " >> reservation: " + DoHaveReservation + " worker: " + comp.Worker?.LabelShort, comp.MyDebug);
            return DoHaveReservation;
        }

        public static bool ReservationIsItemCompatible(this CompConditionalPostDraw comp)
        {
            if (!comp.CurCondition.HasWorkCondition || !comp.IsReserved || comp.FirstReservation == null)
                return false;

            ReservationManager.Reservation resItem = comp.FirstReservation;

            if (comp.CurCondition.ifWork.HasIncludedJob)
                if (!comp.CurCondition.ifWork.includeJob.Contains(resItem.Job.def))
                    return false;

            if (comp.CurCondition.ifWork.HasExcludedJob)
                if (!comp.CurCondition.ifWork.excludeJob.Contains(resItem.Job.def))
                    return false;

            if (comp.CurCondition.ifWork.HasIncludedRecipe && resItem.Job.RecipeDef != null)
                if (comp.CurCondition.ifWork.includeRecipe.Contains(resItem.Job.RecipeDef))
                    return false;

            if (comp.CurCondition.ifWork.HasExcludedRecipe && resItem.Job.RecipeDef != null)
                if (!comp.CurCondition.ifWork.excludeRecipe.Contains(resItem.Job.RecipeDef))
                    return false;

            return true;
        }


        public static void MaybeUpdateReservations(this CompConditionalPostDraw comp)
        {
            if (!comp.RequiresReservationUpdate || !comp.IsTimeToUpdate)
                return;

            comp.UpdateReservationAndWorker();
        }
    }
}
