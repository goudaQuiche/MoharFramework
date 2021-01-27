using UnityEngine;
using Verse;

namespace MoharHediffs
{
    public static class TrailUtils
    {
        public static float GetMoteRotation(this HediffComp_TrailLeaver comp, Vector3 drawPos, out Vector3 normalized)
        {
            normalized = Vector3.zero;

            if (!comp.Props.dynamicRotation && !comp.Props.UsesFootPrints)
                return 0;

            float dynaRot = comp.GetDynamicRotation(drawPos, out normalized);
            float rot = comp.Props.dynamicRotation ? dynaRot : 0;
            rot += comp.Props.HasRotationOffset ? comp.Props.rotationOffset : 0;

            //if (comp.MyDebug) Log.Warning("GetMoteRotation normalized" + normalized);

            return rot % 360;
        }

        public static float GetDynamicRotation(this HediffComp_TrailLeaver comp, Vector3 drawPos, out Vector3 normalized)
        {
            normalized = (drawPos - comp.lastMotePos).normalized;
            return normalized.AngleFlat();
        }

        public static Vector3 GetFootPrintOffset(this HediffComp_TrailLeaver comp, Vector3 normalized)
        {
            if (!comp.Props.UsesFootPrints)
                return Vector3.zero;

            float angle = comp.lastFootprintRight ? 90 : (-90);
            Vector3 b = normalized.RotatedBy(angle) * comp.Props.footprint.distanceBetweenFeet * Mathf.Sqrt(comp.Pawn.BodySize);

            comp.lastFootprintRight = !comp.lastFootprintRight;

            //if (comp.MyDebug)Log.Warning($"{comp.Props.footprint.offset} {b}");

            return comp.Props.footprint.offset + b;
        }

        public static void RecordMotePos(this HediffComp_TrailLeaver comp, Vector3 drawPos)
        {
            if (!comp.Props.dynamicRotation)
                return;

            comp.lastMotePos = drawPos;
        }
    }
}
