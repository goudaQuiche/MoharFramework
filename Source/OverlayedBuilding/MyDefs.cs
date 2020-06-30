using AlienRace;
using Verse;
using UnityEngine;
using RimWorld;

namespace OLB
{
    public static class MyDefs
    {
        public enum DisplayCondition
        {
            [Description("When fueled")]
            WhenFueled = 0,
            [Description("When powered")]
            WhenPowered = 1,
            [Description("when fueled and powered")]
            WhenFueledAndPowered = 2,
            [Description("When worker")]
            WhenWorker = 3,
            [Description("no condition")]
            NoCondition = 4,
            [Description("undefined")]
            Undefined = 5
        }
        public enum DisplayOrigin
        {
            [Description("building centered")]
            BuildingCenter = 0,
            [Description("interaction cell")]
            InteractionCell = 1,
            [Description("worker head")]
            WorkerHead = 2,
            [Description("undefined")]
            Undefined = 10,
        }

        public static bool ShouldSpawnMote(this MoteTracing tracer, CompDecorate comp)
        {
            switch (tracer.condition)
            {
                case DisplayCondition.NoCondition:
                    return true;

                case DisplayCondition.WhenFueled:
                    return comp.compFuel.IsNotEmpty();

                case DisplayCondition.WhenPowered:
                    return comp.compPower.HasPower();

                case DisplayCondition.WhenFueledAndPowered:
                    return comp.compFuel.IsNotEmpty() && comp.compPower.HasPower();

                case DisplayCondition.WhenWorker:
                    return comp.HasWorker;

                case DisplayCondition.Undefined:
                    return false;

                default:
                    return false;
            }

        }

        public static void GetDrawPos(DisplayOrigin origin, Building building, Pawn pawn, out IntVec3 cell, out Vector3 drawPos)
        {

            switch (origin)
            {
                case DisplayOrigin.BuildingCenter:
                    drawPos = building.TrueCenter();
                    cell = drawPos.ToIntVec3();
                    break;
                case DisplayOrigin.InteractionCell:
                    cell = building.InteractionCell;
                    drawPos = cell.ToVector3();
                    break;
                case DisplayOrigin.WorkerHead:
                    cell = pawn.Position;
                    drawPos = pawn.HeadPos();
                    break;
                default:
                    drawPos = building.TrueCenter();
                    cell = drawPos.ToIntVec3();
                    break;
            }
        }
    }
}
