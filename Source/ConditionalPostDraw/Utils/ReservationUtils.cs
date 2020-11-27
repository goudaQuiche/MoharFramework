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
            bool DoHaveReservation = comp.UpdateReservation() && comp.UpdateWorker();
            Tools.Warn(comp.parent.LabelShort + " >> reservation: " + DoHaveReservation + " worker: " + comp.Worker?.LabelShort, comp.MyDebug);
            return DoHaveReservation;
        }

        public static bool ReservationIsItemCompatible(this CompConditionalPostDraw comp)
        {
            if (!comp.IsReserved)
                return false;

            if (comp.FirstReservation == null)
                return false;

            ReservationManager.Reservation resItem = comp.FirstReservation;

            bool compatible = true;

            if (comp.CurCondition.HasIncludedJob)
                if (!(compatible &= comp.CurCondition.includeJob.Contains(resItem.Job.def)))
                    return false;

            if (comp.CurCondition.HasExcludedJob)
                if (!(compatible &= !comp.CurCondition.excludeJob.Contains(resItem.Job.def)))
                    return false;

            if (comp.CurCondition.HasIncludedRecipe && resItem.Job.RecipeDef != null)
                if (!(compatible &= comp.CurCondition.includeRecipe.Contains(resItem.Job.RecipeDef)))
                    return false;

            if (comp.CurCondition.HasExcludedRecipe && resItem.Job.RecipeDef != null)
                if (!(compatible &= !comp.CurCondition.excludeRecipe.Contains(resItem.Job.RecipeDef)))
                    return false;

            return compatible;
        }


        public static void MaybeUpdateReservations(this CompConditionalPostDraw comp)
        {
            if (!comp.RequiresReservationUpdate || !comp.IsTimeToUpdate)
                return;

            comp.UpdateReservationAndWorker();
        }
    }
}
