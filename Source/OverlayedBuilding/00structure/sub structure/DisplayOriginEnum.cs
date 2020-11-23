using Verse;
using UnityEngine;
using RimWorld;

namespace OLB
{
    public static class DisplayOrigin
    {
        public enum Origin
        {
            [Description("building centered")]
            BuildingCenter = 0,
            [Description("interaction cell")]
            InteractionCell = 1,
            [Description("between worker and building")]
            BetweenWorkerAndBuilding = 2,
            [Description("worker cell")]
            WorkerCell = 3,
            [Description("worker head")]
            WorkerHead = 4,
            [Description("undefined")]
            Undefined = 10,
        }

        public static void GetDrawPos(Origin origin, Building building, Pawn pawn, out IntVec3 cell, out Vector3 drawPos)
        {
            drawPos = building.TrueCenter();
            cell = drawPos.ToIntVec3();

            switch (origin)
            {
                case Origin.BuildingCenter:
                    drawPos = building.TrueCenter();
                    cell = drawPos.ToIntVec3();
                    return;
                case Origin.InteractionCell:
                    cell = building.InteractionCell;
                    drawPos = cell.ToVector3();
                    return;
                case Origin.BetweenWorkerAndBuilding:
                    cell = new IntVec3(
                        (building.InteractionCell.x + pawn.Position.x) / 2,
                        0,
                        (building.InteractionCell.z + pawn.Position.z) / 2
                    );
                    drawPos = (building.DrawPos+pawn.DrawPos)/2;
                    return;
                case Origin.WorkerCell:
                    cell = pawn.Position;
                    drawPos = pawn.DrawPos;
                    return;
                case Origin.WorkerHead:
                    if (pawn == null) break;
                    cell = pawn.Position;
                    drawPos = pawn.HeadPos();
                    return;
                default:
                    drawPos = building.TrueCenter();
                    cell = drawPos.ToIntVec3();
                    break;
            }
        }
    }
}
